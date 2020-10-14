using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoGrado.ViewModels
{
    public class ComprasViewModel : BindableBase
    {
        private DataTable _saleTable;

        public DataTable SaleTable
        {
            get => _saleTable;
            set => SetProperty(ref _saleTable, value);
        }

        public DelegateCommand AddCommand { get; set; }
        public DelegateCommand CancelCommand { get; set; }

        public ComprasViewModel()
        {
            AddCommand = new DelegateCommand(Add, CanAdd);
            CancelCommand = new DelegateCommand(Cancel, CanCancel);
            ConectionTable();
        }

        private bool CanCancel()
        {
            return true;
        }

        private void Cancel()
        {
            throw new NotImplementedException();
        }

        private bool CanAdd()
        {
            return true;
        }

        private void Add()
        {
            throw new NotImplementedException();
        }

        private void ConectionTable()
        {
            DataTable dt = new DataTable();
            using (var conn = new SqlConnection(LoginViewModel.ConectionBD))
            {
                string query = "select * from ARTICULO";

                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                SaleTable = dt;
            }
        }


    }
}
