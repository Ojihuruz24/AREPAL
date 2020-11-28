using ControlzEx.Theming;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using ProyectoGrado.Conection;
using ProyectoGrado.Dialog.ViewModels;
using ProyectoGrado.Dialog.Views;
using ProyectoGrado.Events;
using ProyectoGrado.Services;
using ProyectoGrado.Utility.Validations;
using ProyectoGrado.Views;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Security;
using System.Threading.Tasks;
using System.Windows;

namespace ProyectoGrado.ViewModels
{
    public class LoginViewModel : BindableBase
    {
        private string _user;
        private SecureString _password;
        private readonly Action<bool> _onCompleted;
        private readonly IEventAggregator _eventAggregator;
        private Parameter parameter;
        public static string ConectionBD = @"server=(Localdb)\PROYECTO; database=AREPAL ; integrated security = true";
        public static string UserBD = "";
        public static string PathConection = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + @"\Arepal\BD\Conection.json";
        public static string PathBackup = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + @"\AREPAL\BD\AREPAL.bak";

        #region Conection

        private void ConectionParameter()
        {
            if (!File.Exists(PathConection))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(PathConection));
                var arepal = new Parameter
                {
                    ServerName = @"(Localdb)\PROYECTO",
                    DataBase = "AREPAL",
                    Security = true
                };
                string json = JsonConvert.SerializeObject(arepal);
                File.WriteAllText(PathConection, json);
            }

            using (StreamReader jsonStream = File.OpenText(PathConection))
            {
                var json = jsonStream.ReadToEnd();
                parameter = JsonConvert.DeserializeObject<Parameter>(json);
                ConectionBD = $"server= {parameter.ServerName}; database={parameter.DataBase}; integrated security ={parameter.Security}";
            }

        }

        #endregion

        public string User
        {
            get { return _user; }
            set
            {
                if (ValidationesInput.IsNumber(value, "Cédula Incorrecta"))
                {
                    SetProperty(ref _user, value);
                }
            }
        }

        public SecureString Password
        {
            get { return _password; }
            set
            {
                SetProperty(ref _password, value.Copy());
                _password.MakeReadOnly();
            }
        }

        public DelegateCommand LoginUserCommand { get; }
        public DelegateCommand ShowToolsCommand { get; }

        public LoginViewModel(Action<bool> onCompleted, IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            ConectionParameter();
            LoginUserCommand = new DelegateCommand(() => LoginUser(), CanLoginUser)
                .ObservesProperty(() => Password)
                .ObservesProperty(() => User);
            ShowToolsCommand = new DelegateCommand(() => ShowTools(), CanShowTools);

            _onCompleted = onCompleted;
            _eventAggregator.GetEvent<ParameterDataBaseEvent>().Subscribe(OnParameterDataBase);
        }

        private void OnParameterDataBase(Parameter parameter)
        {
            ConectionBD = $"server= {parameter.ServerName}; database={parameter.DataBase}; integrated security ={parameter.Security}";
        }

        private async void ShowTools()
        {
            var dialog = DialogCoordinator.Instance;
            var setings = new LoginDialogSettings()
            {
                UsernameWatermark = "Login",
                PasswordWatermark = "Pasword",
                RememberCheckBoxChecked = true
            };

            var resultDialog = await dialog.ShowLoginAsync(this, "CONFIGURACION", "LOGIN", setings);

            if (string.IsNullOrWhiteSpace(resultDialog.Username) || string.IsNullOrWhiteSpace(resultDialog.Password))
            {
                return;
            }

            await FormConfigDataBase(resultDialog);
        }

        private async Task FormConfigDataBase(LoginDialogData resultDialog)
        {
            try
            {
                using (var conection = new SqlConnection(ConectionBD))
                {
                    conection.Open();

                    var result = await Validation(resultDialog.Username, resultDialog.SecurePassword, conection, false);

                    if (result.Rows.Count == 1)
                    {
                        if (result.Rows[0][3].ToString().ToLower() == "admin")
                        {
                            DialogConfigBDView view = new DialogConfigBDView();
                            _eventAggregator.GetEvent<ParameterDataBaseEvent>().Publish(parameter);
                            view.ShowDialog();
                        }
                        else
                        {
                            await MenssageErrorConectionUser("AUTENTICACIÓN", "NO CUMPLES CON LOS PERMISOS REQUERIDOS");
                        }
                    }
                }
            }
            catch
            {
                await MenssageErrorConectionUser("CONEXION", "Problemas de conexion");
            }
        }

        private bool CanShowTools()
        {
            return true;
        }

        private async void LoginUser()
        {
            if (!OpenConectionBD())
            {
                await MenssageErrorConectionUser("ERROR", "PROBLEMA CON LA CONEXIÓN A LA BASE DE DATOS");
            }
        }

        private bool CanLoginUser()
        {
            if (!string.IsNullOrWhiteSpace(User))
            {
                if (Password != null)
                {
                    return true;
                }
            }
            return false;
        }

        private bool OpenConectionBD()
        {
            try
            {
                using (var conection = new SqlConnection(ConectionBD))
                {
                    conection.Open();

                    _ = Validation(User, Password, conection, true);

                    return true;
                }
            }
            catch (Exception ex)
            {
                MenssageErrorConectionUser("ALERTA", "PROBLEMAS CON LA CONEXION");
            }

            return false;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="user">Se ingresa el nombre del usuario</param>
        /// <param name="pass">Se ingrea la constraseña del usuario</param>
        /// <param name="conection">Se ingresa la conexion </param>
        /// <param name="login">True es para login y FALSE es para configuraciones</param>
        /// <returns></returns>
        private async Task<DataTable> Validation(string user, SecureString pass, SqlConnection conection, bool login)
        {
            DataTable dataTable = new DataTable();
            try
            {
                var validation = $"SELECT * FROM USUARIO WHERE ID = @user AND PASSWORD = @pass";
                using (var consult = new SqlCommand(validation, conection))
                {
                    var credential = new NetworkCredential(user, pass);
                    consult.Parameters.AddWithValue("user", user);
                    consult.Parameters.AddWithValue("pass", credential.Password);
                    SqlDataAdapter sqlData = new SqlDataAdapter(consult);
                    sqlData.Fill(dataTable);

                    if (dataTable.Rows.Count == 1)
                    {

                        if (dataTable.Rows[0][0].ToString() == user && dataTable.Rows[0][2].ToString() == credential.Password)
                        {
                            UserBD = dataTable.Rows[0][0].ToString();
                            if (login)
                            {
                                _onCompleted(true);
                            }
                            return dataTable;
                        }
                    }
                    else
                    {
                        await MenssageErrorConectionUser("AUTENTICACIÓN", "Usuario o contraseña incorrecta");
                    }
                }

                return dataTable;
            }
            catch (Exception ex)
            {
                await MenssageErrorConectionUser("AUTENTICACIÓN", "Problemas de conexion");
                conection.Close();
            }
            finally
            {
                conection.Close();
            }
            return dataTable;
        }

        private async Task MenssageErrorConectionUser(string title, string mensaje)
        {
            var dialog = DialogCoordinator.Instance;
            var settings = new MetroDialogSettings()
            {
                ColorScheme = MetroDialogColorScheme.Theme,
                AnimateHide = true
            };
            await dialog.ShowMessageAsync(this, title, mensaje, MessageDialogStyle.AffirmativeAndNegative, settings);
        }

        private async Task MenssageLoadConection(string title, string mensaje)
        {
            var dialog = DialogCoordinator.Instance;
            var settings = new MetroDialogSettings()
            {
                ColorScheme = MetroDialogColorScheme.Theme,
            };

            await dialog.ShowProgressAsync(this, title, mensaje);
        }
    }
}
