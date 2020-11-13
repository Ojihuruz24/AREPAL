using Newtonsoft.Json;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using ProyectoGrado.Conection;
using ProyectoGrado.Dialog.Views;
using ProyectoGrado.Events;
using ProyectoGrado.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ProyectoGrado.Dialog.ViewModels
{
    public class DialogConfigBDViewModel : BindableBase
    {

        private bool _isAvanced;
        private Visibility _visibilityAdvanced = Visibility.Collapsed;
        private string _nameServer;
        private string _dataBase;
        private readonly IEventAggregator _eventAggregator;

        public bool IsAvanced
        {
            get { return _isAvanced; }
            set
            {
                SetProperty(ref _isAvanced, value);
                if (VisibilityAdvanced == Visibility.Visible)
                {
                    VisibilityAdvanced = Visibility.Collapsed;
                }
                else
                {
                    VisibilityAdvanced = Visibility.Visible;
                }
            }
        }

        private bool _isTrueSeguridad = true;

        public bool IsTrueSeguridad
        {
            get { return _isTrueSeguridad; }
            set
            {
                SetProperty(ref _isTrueSeguridad, value);
            }
        }

        public Visibility VisibilityAdvanced
        {
            get { return _visibilityAdvanced; }
            set
            {
                SetProperty(ref _visibilityAdvanced, value);
            }
        }

        public string NameServer
        {
            get { return _nameServer; }
            set
            {
                SetProperty(ref _nameServer, value);
                AceptarCommand.RaiseCanExecuteChanged();
            }
        }

        public string DataBase
        {
            get { return _dataBase; }
            set
            {
                SetProperty(ref _dataBase, value);
                AceptarCommand.RaiseCanExecuteChanged();
            }
        }

        public DelegateCommand AceptarCommand { set; get; }
        public DelegateCommand CancelCommand { set; get; }

        public DialogConfigBDViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            AceptarCommand = new DelegateCommand(Aceptar, CanAceptar);
            CancelCommand = new DelegateCommand(Cancel, CanCancel);
            ReadConfiguration();
        }

        private void ReadConfiguration()
        {
            string path = LoginViewModel.PathConection;
            using (StreamReader jsonStream = File.OpenText(path))
            {
                var json = jsonStream.ReadToEnd();
                Parameter parameters = JsonConvert.DeserializeObject<Parameter>(json);

                NameServer = parameters.ServerName;
                DataBase = parameters.DataBase;
                IsTrueSeguridad = parameters.Security;
            }
        }

        private bool CanCancel()
        {
            return true;
        }

        private void Cancel()
        {
            Clear();
        }

        private void Clear()
        {
            NameServer = string.Empty;
            DataBase = string.Empty;
            IsTrueSeguridad = true;
            IsAvanced = false;
        }

        private bool CanAceptar()
        {
            if (!string.IsNullOrWhiteSpace(NameServer) &&
                !string.IsNullOrWhiteSpace(DataBase))
            {
                return true;
            }
            return false;
        }

        private void Aceptar()
        {
            var parameter = new Parameter
            {
                ServerName = NameServer,
                DataBase = DataBase,
                Security = IsTrueSeguridad
            };

            string json = JsonConvert.SerializeObject(parameter);
            File.WriteAllText(LoginViewModel.PathConection, json);

            _eventAggregator.GetEvent<ParameterDataBaseEvent>().Publish(parameter);

        }
    }
}
