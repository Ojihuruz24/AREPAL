using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;
using ProyectoGrado.Conection;
using ProyectoGrado.Services.DataBase;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows;


namespace ProyectoGrado.ViewModels
{
    public class HomeViewModel : BindableBase
    {
        private Parameter parameter;
        private Visibility _permisson;
        private readonly BackupService _backupService;
       

        public Visibility Permisson
        {
            get { return _permisson; }
            set { SetProperty(ref _permisson, value); }
        }

        public DelegateCommand BackupRestoreCommand { get; }


        public HomeViewModel(BackupService backupService)
        {
            validation();
            BackupRestoreCommand = new DelegateCommand(() => BackupRestore(), CanBackupRestore);
            _backupService = backupService;
        }

        private bool CanBackupRestore()
        {
            return true;
        }

        private void BackupRestore()
        {
            _backupService.BackupDatabase("AREPAL");

        }

        private void validation()
        {
            using (var conn = new SqlConnection(LoginViewModel.ConectionBD))
            {
                var query = $"SELECT * FROM USUARIO WHERE ID = {LoginViewModel.UserBD}";

                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataAdapter da = new SqlDataAdapter(cmd);

                SqlDataAdapter sqlData = new SqlDataAdapter(cmd);
                DataTable dataTable = new DataTable();
                sqlData.Fill(dataTable);

                if (dataTable.Rows.Count == 1)
                {
                    if (dataTable.Rows[0][3].ToString().ToLower() == "vendedor")
                    {
                        Permisson = Visibility.Collapsed;
                    }
                }
            }
        }
    }
}
