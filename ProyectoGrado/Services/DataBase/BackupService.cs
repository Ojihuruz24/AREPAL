using ProyectoGrado.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoGrado.Services.DataBase
{
    public class BackupService
    {
        private string _backupFolderFullPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\BACKUP";


        public BackupService()
        {

        }

        public void BackupDatabase(string databaseName)
        {
            IsExistPath(_backupFolderFullPath);

            string filePath = BuildBackupPathWithFilename(databaseName);

            using (var connection = new SqlConnection(LoginViewModel.ConectionBD))
            {
                var query = $"BACKUP DATABASE [{databaseName}] TO DISK='{filePath}'";

                using (var command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        private string BuildBackupPathWithFilename(string databaseName)
        {
            string filename = string.Format("{0}-{1}.bak", databaseName, DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));

            return Path.Combine(_backupFolderFullPath, filename);
        }

        private void IsExistPath(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

    }

}
