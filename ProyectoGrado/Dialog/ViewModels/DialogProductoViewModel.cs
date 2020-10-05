using Prism.Commands;
using Prism.Mvvm;
using ProyectoGrado.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoGrado.Dialog.ViewModels
{
    public class DialogProductoViewModel : BindableBase
    {

        private DataTable _products;
        private string _searchProduct;

        public DataTable Products
        {
            get { return _products; }
            set { SetProperty(ref _products, value); }
        }

        public string SearchProduct
        {
            get { return _searchProduct; }
            set
            {
                SetProperty(ref _searchProduct, value);
                Search(value);
            }
        }

        private DataRowView _productSelected;

        public DataRowView ProductSelected
        {
            get { return _productSelected; }
            set { SetProperty(ref _productSelected, value); }
        }


        public DelegateCommand SendCommand { get; }

        

        public DialogProductoViewModel()
        {
            ConectionTable();
            SendCommand = new DelegateCommand(Send, CanSend);
        }

        private bool CanSend()
        {
            return true;
        }

        private void Send()
        {
            var daa = ProductSelected.Row[0];
        }

        private void Search(string search)
        {
            if (!string.IsNullOrEmpty(search))
            {
                Products.DefaultView.RowFilter = $"ID_PRODUCTO = {SearchProduct}";
                return;
            }

            ConectionTable();
        }

        private void ConectionTable()
        {
            DataTable dt = new DataTable();
            using (var conn = new SqlConnection(LoginViewModel.ConectionBD))
            {
                string query = "select * from PRODUCTO";

                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                Products = dt;
            }

        }
    }
}
