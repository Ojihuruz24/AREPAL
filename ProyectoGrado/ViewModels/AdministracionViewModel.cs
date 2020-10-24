using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using ProyectoGrado.Dialog;
using ProyectoGrado.Dialog.ViewModels;
using ProyectoGrado.Events;
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
        private readonly IEventAggregator _eventAggregator;

        public AdministracionViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            CategoriaViewModel();
            ProductoViewModel();
            DetalleProductoViewModel();
            ArticleViewModel();
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

        private void ProductoViewModel()
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

        #region DETALLE_PRODUCTO

        private ObservableCollection<Articulo> _articlesDetailProduct;
        private DataTable _tableDetalleProducto;
        private string _nameProducto;
        private string _countDetailProduct;
        private string _codeProductDetailProduct;

        public string NameProducto
        {
            get { return _nameProducto; }
            set { SetProperty(ref _nameProducto, value); }
        }

        public string CodeProductDetailProduct
        {
            get { return _codeProductDetailProduct; }
            set { SetProperty(ref _codeProductDetailProduct, value); }
        }

        public string CountDetailProduct
        {
            get { return _countDetailProduct; }
            set { SetProperty(ref _countDetailProduct, value); }
        }

        public ObservableCollection<Articulo> ArticlesDetailProduct
        {
            get { return _articlesDetailProduct; }
            set
            {
                SetProperty(ref _articlesDetailProduct, value);
            }
        }

        public DataTable TableDetalleProducto
        {
            get => _tableDetalleProducto;
            set => SetProperty(ref _tableDetalleProducto, value);
        }

        public Articulo ArticleDetailProduct { get; set; }


        public DelegateCommand ArticleDetalleComboboxCommand { get; private set; }
        public DelegateCommand SearchCodeDetalleProductoCommand { get; private set; }
        public DelegateCommand AddDetailProductCommand { get; private set; }

        private void DetalleProductoViewModel()
        {
            ConsultDetalleProducto();
            ArticleDetalleComboboxCommand = new DelegateCommand(UpdateArticle, CanUpdateArticle);
            SearchCodeDetalleProductoCommand = new DelegateCommand(OpenProduct, CanOpenProduct);
            AddDetailProductCommand = new DelegateCommand(AddDetailProduct, CanAddDetailProduct);
            AddArticleDetalle();
            _eventAggregator.GetEvent<ProductSelectedEvent>().Subscribe(OnProductSelected);
        }

        private bool CanAddDetailProduct()
        {
            return true;
        }

        private void AddDetailProduct()
        {
            using (var conn = new SqlConnection(LoginViewModel.ConectionBD))
            {
                try
                {
                    string query = "INSERT INTO DETALLE_PRODUCTO (ID_PRODUCTO , ID_ARTICULO, CANTIDAD)" +
                        " VALUES (@ID_PRODUCTO, @ID_ARTICULO, @CANTIDAD)";
                    SqlCommand cmd = new SqlCommand(query, conn);

                    conn.Open();

                    cmd.Parameters.AddWithValue("@ID_PRODUCTO", CodeProductDetailProduct);
                    cmd.Parameters.AddWithValue("@ID_ARTICULO", ArticleDetailProduct.Codigo);
                    cmd.Parameters.AddWithValue("@CANTIDAD", CountDetailProduct);

                    cmd.ExecuteNonQuery();

                    conn.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error a la hora de insertar el cliente", ex.Message);
                }

                ConsultDetalleProducto();

                ClearDetailProduct();

            }
        }

        private void ClearDetailProduct()
        {
            CodeProductDetailProduct = string.Empty;
            NameProducto = string.Empty;
            ArticlesDetailProduct = new ObservableCollection<Articulo>();
            AddArticleDetalle();
            CountDetailProduct = string.Empty;

        }

        private void OnProductSelected(DataRowView product)
        {
            CodeProductDetailProduct = product.Row[0].ToString();
            NameProducto = product.Row[2].ToString();
        }

        private bool CanOpenProduct()
        {
            return true;
        }

        private void OpenProduct()
        {
            DialogProductoView dialogProductoView = new DialogProductoView();
            dialogProductoView.DataContext = new DialogProductoViewModel(_eventAggregator);
            dialogProductoView.ShowDialog();
        }

        private bool CanUpdateArticle()
        {
            return true;
        }

        private void UpdateArticle()
        {
            AddArticleDetalle();
        }

        private void AddArticleDetalle()
        {
            using (var conn = new SqlConnection(LoginViewModel.ConectionBD))
            {
                conn.Open();
                ArticlesDetailProduct = new ObservableCollection<Articulo>();
                string query = "SELECT * FROM ARTICULO ";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    ArticlesDetailProduct.Add(new Articulo
                    {
                        Codigo = dr[0].ToString(),
                        Name = dr[1].ToString()
                    });
                }

                conn.Close();
            }
        }

        private void ConsultDetalleProducto()
        {
            DataTable dt = new DataTable();
            using (var conn = new SqlConnection(LoginViewModel.ConectionBD))
            {
                string query = "SELECT " +
                    "DETALLE_PRODUCTO.ID , " +
                    "DETALLE_PRODUCTO.ID_PRODUCTO AS CODIGO," +
                    "PRODUCTO.NOMBRE AS PRODUCTO, " +
                    "ARTICULO.NOMBRE AS ARTICULO ," +
                    "DETALLE_PRODUCTO.CANTIDAD " +
                    "FROM DETALLE_PRODUCTO " +
                    "INNER JOIN PRODUCTO ON PRODUCTO.ID = DETALLE_PRODUCTO.ID_PRODUCTO " +
                    "INNER JOIN ARTICULO ON  ARTICULO.ID = DETALLE_PRODUCTO.ID_ARTICULO";

                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                TableDetalleProducto = dt;
            }

        }
        #endregion

        #region ARTICULO

        private ObservableCollection<string> _measureArticles;
        private DataTable _tableArticles;

        public ObservableCollection<string> MeasureArticles
        {
            get { return _measureArticles; }
            set { SetProperty(ref _measureArticles, value); }
        }

        public string MeasureArticle { get; set; }

        public string NameArticle { get; set; }

        public int QuantityArticle { get; set; }

        public DataTable TableArticles
        {
            get => _tableArticles;
            set => SetProperty(ref _tableArticles, value);
        }
        public DelegateCommand AddArticleCommand { get; private set; }

        private void ArticleViewModel()
        {
            AddArticleCommand = new DelegateCommand(AddArticle, CanAddArticle);
            ConsultArticles();
            AddArticles();
        }

        private bool CanAddArticle()
        {
            return true;
        }

        private void AddArticle()
        {
            using (var conn = new SqlConnection(LoginViewModel.ConectionBD))
            {
                try
                {
                    string query = "INSERT INTO ARTICULO (NOMBRE , MEDIDA, STOCK)" +
                        " VALUES (@NOMBRE, @MEDIDA, @STOCK)";
                    SqlCommand cmd = new SqlCommand(query, conn);

                    conn.Open();

                    cmd.Parameters.AddWithValue("@NOMBRE", NameArticle);
                    cmd.Parameters.AddWithValue("@MEDIDA", MeasureArticle);
                    cmd.Parameters.AddWithValue("@STOCK", QuantityArticle);

                    cmd.ExecuteNonQuery();

                    conn.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error a la hora de insertar el articulo : {ex.Message}", "ERROR");
                }

                ConsultArticles();

                ClearArticle();
            }
        }

        private void ClearArticle()
        {
            NameArticle = string.Empty;
            AddArticles();
            QuantityArticle = 0;
        }

        private void ConsultArticles()
        {
            using (var conn = new SqlConnection(LoginViewModel.ConectionBD))
            {
                DataTable dt = new DataTable();
                string query = "SELECT * FROM ARTICULO";

                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                TableArticles = dt;
            }
        }



        private void AddArticles()
        {
            MeasureArticles = new ObservableCollection<string>();
            MeasureArticles.Add("KILOGRAMOS");
            MeasureArticles.Add("ONZAS");
            MeasureArticles.Add("TONELADA");
            MeasureArticles.Add("COSTALES");
        }

        #endregion
    }
}

