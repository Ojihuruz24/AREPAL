using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using ProyectoGrado.Views;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Security;
using System.Windows;

namespace ProyectoGrado.ViewModels
{
    class LoginViewModel : BindableBase
    {
        private string _user;
        private SecureString _password;

        public static string ConectionBD = @"server=(Localdb)\PROYECTO ; database=AREPAL ; integrated security = true";
        public static string UserBD = "";
        private readonly Action<bool> _onCompleted;

        public string User
        {
            get { return _user; }
            set { SetProperty(ref _user, value); }
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

        public LoginViewModel(Action<bool> onCompleted)
        {
            LoginUserCommand = new DelegateCommand(LoginUser, CanLoginUser);
            _onCompleted = onCompleted;
        }
        private void LoginUser()
        {
            if (!OpenConectionBD())
            {
                MessageBox.Show("Problemas con la conexion a la base de datos", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanLoginUser()
        {
            return true;
        }

        private bool OpenConectionBD()
        {
            using (var conection = new SqlConnection(ConectionBD))
            {
                conection.Open();

                Validation(User, Password, conection);

                return true;
            }
        }

        private void Validation(string user, SecureString pass, SqlConnection conection)
        {
            try
            {
                var validation = $"SELECT * FROM USUARIO WHERE ID = @user AND PASSWORD = @pass";
                using (var consult = new SqlCommand(validation, conection))
                {
                    var credential = new NetworkCredential(user, pass);
                    consult.Parameters.AddWithValue("user", user);
                    consult.Parameters.AddWithValue("pass", credential.Password);
                    SqlDataAdapter sqlData = new SqlDataAdapter(consult);
                    DataTable dataTable = new DataTable();
                    sqlData.Fill(dataTable);

                    if (dataTable.Rows.Count == 1)
                    {
                        
                        if (dataTable.Rows[0][0].ToString() == user && dataTable.Rows[0][2].ToString() == credential.Password)
                        {
                            UserBD = dataTable.Rows[0][0].ToString();
                            _onCompleted(true);

                        }
                    }
                    else
                    {
                        MessageBox.Show($"No se encontro datos");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error en la consulta, {ex.Message}");
                conection.Close();
            }
            finally
            {
                conection.Close();
            }
        }
    }
}
