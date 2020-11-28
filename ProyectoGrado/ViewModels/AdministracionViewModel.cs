using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using ProyectoGrado.Dialog;
using ProyectoGrado.Dialog.ViewModels;
using ProyectoGrado.Events;
using ProyectoGrado.Utility.Validations;
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

        private bool _isSelectedCategoria = true;
        private bool _isSelectedArticle;


        public bool IsSelectedArticle
        {
            get { return _isSelectedArticle; }
            set
            {
                _isSelectedArticle = value;
                ArticleViewModel();
            }
        }

        public bool IsSelectedCategoria
        {
            get { return _isSelectedCategoria; }
            set
            {
                _isSelectedCategoria = value;
                CategoriaViewModel();
            }
        }


        public AdministracionViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            CategoriaViewModel();
            ProductoViewModel();
            DetalleProductoViewModel();
            ArticleViewModel();
            UserViewModel();
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
            set
            {
                if (ValidationesInput.IsString(value, "Nombre de categoria invalido"))
                {
                    SetProperty(ref nombreCategoria, value);
                }
            }
        }

        public DelegateCommand AddCategoriaCommand { get; set; }
        public DelegateCommand CancelCategoriaCommand { get; set; }

        private void CategoriaViewModel()
        {
            AddCategoriaCommand = new DelegateCommand(AddCategoria, CanAddCategoria).ObservesProperty(() => NombreCategoria);
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
            if (!string.IsNullOrWhiteSpace(NombreCategoria))
            {
                return true;
            }
            return false;
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
        private string _codeProducto;
        private string _nombreProducto;
        private string _precioVentaProducto;
        private ObservableCollection<Articulo> _categoriaProductos;
        private Articulo categoriaProducto;

        public DataTable TableProductos
        {
            get => _tableProductos;
            set => SetProperty(ref _tableProductos, value);
        }

        public ObservableCollection<Articulo> CategoriaProductos
        {
            get { return _categoriaProductos; }
            set
            {
                SetProperty(ref _categoriaProductos, value);

            }
        }

        public Articulo CategoriaProducto
        {
            get => categoriaProducto;
            set
            {
                SetProperty(ref categoriaProducto, value);
            }
        }

        public string CodeProducto
        {
            get => _codeProducto;
            set
            {
                if (ValidationesInput.IsNumber(value, "Codigo invalido"))
                {
                    SetProperty(ref _codeProducto, value);
                }

            }
        }

        public string NombreProducto
        {
            get => _nombreProducto;
            set
            {
                if (ValidationesInput.IsString(value, "Nombre invalido"))
                {
                    SetProperty(ref _nombreProducto, value);
                }
            }
        }

        public string PrecioVentaProducto
        {
            get => _precioVentaProducto;
            set
            {
                if (ValidationesInput.IsNumber(value, "Precio invalido"))
                {
                    SetProperty(ref _precioVentaProducto, value);
                }
            }
        }

        public DelegateCommand AddProductoCommand { get; set; }
        public DelegateCommand CancelProductoCommand { get; set; }
        public DelegateCommand CategoriaComboboxCommand { get; set; }

        private void ProductoViewModel()
        {
            AddProductoCommand = new DelegateCommand(AddProducto, CanProducto)
                .ObservesProperty(() => CategoriaProducto)
                .ObservesProperty(() => CodeProducto)
                .ObservesProperty(() => NombreProducto)
                .ObservesProperty(() => PrecioVentaProducto);
            CancelProductoCommand = new DelegateCommand(CancelProducto, CanCancelProducto);
            CategoriaComboboxCommand = new DelegateCommand(CategoriaCombobo, CanCategoriaCombobox);
            ConsultProducto();
            AddCategoriaProducto();
        }

        private bool CanCancelProducto()
        {
            return true;
        }

        private void CancelProducto()
        {
            ClearProduct();
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
            if (!string.IsNullOrWhiteSpace(CodeProducto)
                && !string.IsNullOrWhiteSpace(NombreProducto)
                && !string.IsNullOrWhiteSpace(PrecioVentaProducto)
                && CategoriaProducto != null)
            {
                return true;
            }
            return false;
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

                ClearProduct();
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

        private void ClearProduct()
        {
            CodeProducto = string.Empty;
            NombreProducto = string.Empty;
            PrecioVentaProducto = string.Empty;
            AddCategoriaProducto();
        }

        #endregion

        #region DETALLE_PRODUCTO

        private ObservableCollection<Articulo> _articlesDetailProduct;
        private DataTable _tableDetalleProducto;
        private string _nameProducto;
        private string _countDetailProduct;
        private string _codeProductDetailProduct;
        private Articulo _articleDetailProduct;

        public string NameProducto
        {
            get { return _nameProducto; }
            set
            {
                SetProperty(ref _nameProducto, value);
            }
        }

        public string CodeProductDetailProduct
        {
            get { return _codeProductDetailProduct; }
            set
            {
                SetProperty(ref _codeProductDetailProduct, value);
            }
        }

        public string CountDetailProduct
        {
            get { return _countDetailProduct; }
            set
            {
                if (ValidationesInput.IsNumber(value, "Cantidad invalida"))
                {
                    SetProperty(ref _countDetailProduct, value);
                }
            }
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

        public Articulo ArticleDetailProduct
        {
            get => _articleDetailProduct;
            set
            {
                SetProperty(ref _articleDetailProduct, value);
            }
        }

        public DelegateCommand ArticleDetalleComboboxCommand { get; private set; }
        public DelegateCommand SearchCodeDetalleProductoCommand { get; private set; }
        public DelegateCommand AddDetailProductCommand { get; private set; }
        public DelegateCommand CancelDetailProductCommand { get; private set; }

        private void DetalleProductoViewModel()
        {
            ConsultDetalleProducto();
            ArticleDetalleComboboxCommand = new DelegateCommand(UpdateArticle, CanUpdateArticle);
            SearchCodeDetalleProductoCommand = new DelegateCommand(OpenProduct, CanOpenProduct);
            AddDetailProductCommand = new DelegateCommand(AddDetailProduct, CanAddDetailProduct)
                .ObservesProperty(() => NameProducto)
                .ObservesProperty(() => CodeProductDetailProduct)
                .ObservesProperty(() => CountDetailProduct)
                .ObservesProperty(() => ArticleDetailProduct);
            CancelDetailProductCommand = new DelegateCommand(CancelDetailProduct, CanAddCancelDetailProduct);
            AddArticleDetalle();
            _eventAggregator.GetEvent<ProductSelectedEvent>().Subscribe(OnProductSelected);
        }

        private bool CanAddCancelDetailProduct()
        {
            return true;
        }

        private void CancelDetailProduct()
        {
            ClearDetailProduct();
        }

        private bool CanAddDetailProduct()
        {
            if (!string.IsNullOrWhiteSpace(CodeProductDetailProduct)
                && !string.IsNullOrWhiteSpace(NameProducto)
                && !string.IsNullOrWhiteSpace(CountDetailProduct)
                && ArticleDetailProduct != null)
            {
                return true;
            }
            return false;
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
        private string nameArticle;
        private string _quantityArticle;
        private string _measureArticle;

        public ObservableCollection<string> MeasureArticles
        {
            get { return _measureArticles; }
            set { SetProperty(ref _measureArticles, value); }
        }

        public string NameArticle
        {
            get => nameArticle;
            set
            {
                if (ValidationesInput.IsString(value, "Nombre invalido"))
                {
                    SetProperty(ref nameArticle, value);
                }
            }
        }

        public string MeasureArticle
        {
            get => _measureArticle;
            set
            {
                SetProperty(ref _measureArticle, value);
            }
        }

        public string QuantityArticle
        {
            get => _quantityArticle;
            set
            {
                if (ValidationesInput.IsNumber(value, "Cantidad invalida"))
                {
                    SetProperty(ref _quantityArticle, value);
                }
            }

        }

        public DataTable TableArticles
        {
            get => _tableArticles;
            set => SetProperty(ref _tableArticles, value);
        }
        public DelegateCommand AddArticleCommand { get; private set; }
        public DelegateCommand CancelArticleCommand { get; private set; }

        private void ArticleViewModel()
        {
            AddArticleCommand = new DelegateCommand(AddArticle, CanAddArticle)
                .ObservesProperty(() => NameArticle)
                .ObservesProperty(() => MeasureArticle)
                .ObservesProperty(() => QuantityArticle)
                .ObservesProperty(() => MeasureArticles);
            CancelArticleCommand = new DelegateCommand(CancelArticle, CanCancelArticle);
            ConsultArticles();
            AddArticles();
        }

        private bool CanCancelArticle()
        {
            return true;
        }

        private void CancelArticle()
        {
            ClearArticle();
        }

        private bool CanAddArticle()
        {
            if (!string.IsNullOrWhiteSpace(NameArticle)
                && !string.IsNullOrWhiteSpace(MeasureArticle)
                && !string.IsNullOrWhiteSpace(QuantityArticle))
            {
                return true;
            }
            return false;
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
            QuantityArticle = string.Empty;
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

        #region USUARIO

        private string _documentUser;
        private string _nameUser;
        private string _passwordUser;
        private ObservableCollection<string> _rolUsers;
        private string _rolUser;
        private DataTable _tableUser;

        public DataTable TableUser
        {
            get { return _tableUser; }
            set { SetProperty(ref _tableUser, value); }
        }

        public string DocumentUser
        {
            get { return _documentUser; }
            set
            {
                if (ValidationesInput.IsNumber(value, "Documento invalido"))
                {
                    SetProperty(ref _documentUser, value);
                }
            }
        }

        public string NameUser
        {
            get { return _nameUser; }
            set
            {
                if (ValidationesInput.IsString(value, "Nombre invalido"))
                {
                    SetProperty(ref _nameUser, value);
                }
            }
        }

        public string PasswordUser
        {
            get { return _passwordUser; }
            set
            {
                SetProperty(ref _passwordUser, value);
            }
        }

        public ObservableCollection<string> RolUsers
        {
            get { return _rolUsers; }
            set { SetProperty(ref _rolUsers, value); }
        }

        public string RolUser
        {
            get { return _rolUser; }
            set
            {
                SetProperty(ref _rolUser, value);
            }
        }

        public DelegateCommand AddUserCommand { get; set; }
        public DelegateCommand CancelUserCommand { get; set; }

        private void UserViewModel()
        {
            AddUserCommand = new DelegateCommand(AddUser, CanAddUser)
                .ObservesProperty(() => DocumentUser)
                .ObservesProperty(() => NameUser)
                .ObservesProperty(() => PasswordUser)
                .ObservesProperty(() => RolUser);
            CancelUserCommand = new DelegateCommand(CancelUser, CanCancelUser);

            AddListRols();
            ConsultUser();
        }

        private bool CanCancelUser()
        {
            return true;
        }

        private void CancelUser()
        {
            ClearUser();
        }

        private bool CanAddUser()
        {
            if (!string.IsNullOrWhiteSpace(DocumentUser)
                && !string.IsNullOrWhiteSpace(NameUser)
                && !string.IsNullOrWhiteSpace(PasswordUser)
                && !string.IsNullOrWhiteSpace(RolUser))
            {
                return true;
            }
            return false;
        }

        private void AddUser()
        {
            using (var conn = new SqlConnection(LoginViewModel.ConectionBD))
            {
                try
                {
                    string query = "INSERT INTO USUARIO (ID, NOMBRE , PASSWORD, ROL)" +
                        " VALUES (@ID, @NOMBRE, @PASSWORD, @ROL)";
                    SqlCommand cmd = new SqlCommand(query, conn);

                    conn.Open();

                    cmd.Parameters.AddWithValue("@ID", DocumentUser);
                    cmd.Parameters.AddWithValue("@NOMBRE", NameUser);
                    cmd.Parameters.AddWithValue("@PASSWORD", PasswordUser);
                    cmd.Parameters.AddWithValue("@ROL", RolUser);

                    cmd.ExecuteNonQuery();

                    conn.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error a la hora de insertar el cliente", ex.Message);
                }

                ConsultUser();
                ClearUser();
            }
        }
        private void ClearUser()
        {
            DocumentUser = string.Empty;
            NameUser = string.Empty;
            PasswordUser = string.Empty;
            AddListRols();
        }

        private void AddListRols()
        {
            RolUsers = new ObservableCollection<string>();
            RolUsers.Add("ADMIN");
            RolUsers.Add("VENDEDOR");
        }

        private void ConsultUser()
        {
            using (var conn = new SqlConnection(LoginViewModel.ConectionBD))
            {
                DataTable dt = new DataTable();
                string query = "SELECT * FROM USUARIO";

                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                TableUser = dt;
            }
        }
        #endregion

    }
}

