using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ProyectoGrado.ViewModels
{
    public class HomeViewModel : BindableBase
    {

        private Visibility _permisson;

        public Visibility Permisson
        {
            get { return _permisson; }
            set { SetProperty(ref _permisson, value); }
        }

        public HomeViewModel()
        {
            validation();
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
