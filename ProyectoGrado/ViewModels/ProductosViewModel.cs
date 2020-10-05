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
    public class ProductosViewModel : BindableBase
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

        public ProductosViewModel()
        {
            ConectionTable();
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
