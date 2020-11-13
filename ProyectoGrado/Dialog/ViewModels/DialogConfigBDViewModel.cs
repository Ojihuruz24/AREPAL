using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using ProyectoGrado.Conection;
using ProyectoGrado.Events;
using System;
using System.Collections.Generic;
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
            }
        }

        public string DataBase
        {
            get { return _dataBase; }
            set
            {
                SetProperty(ref _dataBase, value);
            }
        }

        public DelegateCommand AceptarCommand { set; get; }
        public DelegateCommand CancelCommand { set; get; }

        public DialogConfigBDViewModel(IEventAggregator eventAggregator)
        {
            AceptarCommand = new DelegateCommand(Aceptar, CanAceptar);
            CancelCommand = new DelegateCommand(Cancel, CanCancel);
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<ParameterDataBaseEvent>().Subscribe(OnParameterSelected);
        }

        private void OnParameterSelected(Parameter parameter)
        {
            NameServer = parameter.ServerName;
            DataBase = parameter.DataBase;
        }

        private bool CanCancel()
        {
            return true;
        }

        private void Cancel()
        {

        }

        private bool CanAceptar()
        {
            return true;
        }

        private void Aceptar()
        {
            
        }
    }
}
