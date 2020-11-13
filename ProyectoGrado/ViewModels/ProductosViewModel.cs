using Prism.Mvvm;
using ProyectoGrado.Utility.Validations;
using System.Data;
using System.Data.SqlClient;

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
                if (ValidationesInput.IsNumber(value, "Codigo invalido"))
                {
                    SetProperty(ref _searchProduct, value);
                    Search(value);
                }


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
                Products.DefaultView.RowFilter = $"CODIGO = {SearchProduct}";
                return;
            }

            ConectionTable();
        }

        private void ConectionTable()
        {
            DataTable dt = new DataTable();
            using (var conn = new SqlConnection(LoginViewModel.ConectionBD))
            {
                string query = "SELECT " +
                    " PRODUCTO.ID," +
                    " PRODUCTO.CODIGO," +
                    " PRODUCTO.NOMBRE ," +
                    " PRODUCTO.PRECIO, " +
                    " CATEGORIA.NOMBRE AS CATEGORIA" +
                    " FROM CATEGORIA " +
                    "INNER JOIN PRODUCTO ON CATEGORIA.ID = PRODUCTO.ID_CATEGORIA";

                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                Products = dt;
            }
        }
    }
}
