using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ProyectoGrado.ViewModels
{
    public class AdministracionViewModel : BindableBase
    {

        public AdministracionViewModel()
        {
            CategoriaViewModel();
            ProductoVoewModel();
        }

        #region CATEGORIA

        private DataTable _tableCategoria;
        private string nombreCategoria;

        public DataTable TableCategoria
        {
            get => _tableCategoria;
            set => SetProperty(ref _tableCategoria, value);
        }

        public string NombreCategoria
        {
            get => nombreCategoria;
            set => SetProperty(ref nombreCategoria, value);
        }

        public DelegateCommand AddCategoriaCommand { get; set; }
        public DelegateCommand CancelCategoriaCommand { get; set; }

        private void CategoriaViewModel()
        {
            AddCategoriaCommand = new DelegateCommand(AddCategoria, CanAddCategoria);
            CancelCategoriaCommand = new DelegateCommand(CancelCategoria, CanCancelCategoria);
            ConsultCategoria();
        }

        private bool CanCancelCategoria()
        {
            return true;
        }

        private void CancelCategoria()
        {
            ClearCategory();
        }

        private bool CanAddCategoria()
        {
            return true;
        }

        private void AddCategoria()
        {
            using (var conn = new SqlConnection(LoginViewModel.ConectionBD))
            {
                try
                {
                    string query = "INSERT INTO CATEGORIA (NOMBRE)" +
                        " VALUES (@NOMBRE)";
                    SqlCommand cmd = new SqlCommand(query, conn);

                    conn.Open();

                    cmd.Parameters.AddWithValue("@NOMBRE", NombreCategoria);

                    cmd.ExecuteNonQuery();

                    conn.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error a la hora de insertar la categoria", ex.Message);
                }

                ConsultCategoria();

                ClearCategory();
            }
        }

        private void ClearCategory()
        {
            NombreCategoria = string.Empty;
        }

        private void ConsultCategoria()
        {
            DataTable dt = new DataTable();
            using (var conn = new SqlConnection(LoginViewModel.ConectionBD))
            {
                string query = "select * from CATEGORIA";

                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                TableCategoria = dt;
            }
        }

        #endregion

        #region PRODUCTO

        private DataTable _tableProductos;
        private int _codeProducto;
        private string _nombreProducto;
        private int _precioVentaProducto;
        private ObservableCollection<Articulo> _categoriaProductos;

        public DataTable TableProductos
        {
            get => _tableProductos;
            set => SetProperty(ref _tableProductos, value);
        }

        public ObservableCollection<Articulo> CategoriaProductos
        {
            get { return _categoriaProductos; }
            set { SetProperty(ref _categoriaProductos, value); }
        }

        public Articulo CategoriaProducto { get; set; }

        public int CodeProducto
        {
            get => _codeProducto;
            set => SetProperty(ref _codeProducto, value);
        }

        public string NombreProducto
        {
            get => _nombreProducto;
            set => SetProperty(ref _nombreProducto, value);
        }

        public int PrecioVentaProducto
        {
            get => _precioVentaProducto;
            set => SetProperty(ref _precioVentaProducto, value);
        }

        public DelegateCommand AddProductoCommand { get; set; }
        public DelegateCommand CategoriaComboboxCommand { get; set; }

        private void ProductoVoewModel()
        {
            AddProductoCommand = new DelegateCommand(AddProducto, CanProducto);
            CategoriaComboboxCommand = new DelegateCommand(CategoriaCombobo, CanCategoriaCombobox);
            ConsultProducto();
            AddCategoriaProducto();
        }

        private bool CanCategoriaCombobox()
        {
            return true;
        }

        private void CategoriaCombobo()
        {
            AddCategoriaProducto();
        }

        private bool CanProducto()
        {
            return true;
        }

        private void AddProducto()
        {
            using (var conn = new SqlConnection(LoginViewModel.ConectionBD))
            {
                try
                {
                    string query = "INSERT INTO PRODUCTO (CODIGO, NOMBRE , PRECIO, ID_CATEGORIA)" +
                        " VALUES (@CODIGO, @NOMBRE, @PRECIO, @ID_CATEGORIA)";
                    SqlCommand cmd = new SqlCommand(query, conn);

                    conn.Open();

                    cmd.Parameters.AddWithValue("@CODIGO", CodeProducto);
                    cmd.Parameters.AddWithValue("@NOMBRE", NombreProducto);
                    cmd.Parameters.AddWithValue("@PRECIO", PrecioVentaProducto);
                    cmd.Parameters.AddWithValue("@ID_CATEGORIA", CategoriaProducto.Codigo);

                    cmd.ExecuteNonQuery();

                    conn.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error a la hora de insertar el producto", ex.Message);
                }

                ConsultProducto();


            }
        }

       public void AddCategoriaProducto()
        {
            using (var conn = new SqlConnection(LoginViewModel.ConectionBD))
            {
                conn.Open();
                CategoriaProductos = new ObservableCollection<Articulo>();
                string query = "SELECT * FROM CATEGORIA ";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    CategoriaProductos.Add(new Articulo
                    {
                        Codigo = dr[0].ToString(),
                        Name = dr[1].ToString()
                    });
                }

                conn.Close();
            }
        }



        private void ConsultProducto()
        {
            DataTable dt = new DataTable();
            using (var conn = new SqlConnection(LoginViewModel.ConectionBD))
            {
                string query = "SELECT PRODUCTO.ID, PRODUCTO.CODIGO, PRODUCTO.NOMBRE ,PRODUCTO.PRECIO, CATEGORIA.NOMBRE AS CATEGORIA" +
                     " FROM CATEGORIA " +
                     "INNER JOIN PRODUCTO ON CATEGORIA.ID = PRODUCTO.ID_CATEGORIA";

                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                TableProductos = dt;
            }
        }

        #endregion

    }
}
