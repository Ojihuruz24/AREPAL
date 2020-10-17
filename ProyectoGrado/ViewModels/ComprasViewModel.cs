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
using System.Windows.Controls;

namespace ProyectoGrado.ViewModels
{
    public class ComprasViewModel : BindableBase
    {
        private DataTable _tableArticle;
        private ObservableCollection<Articulo> _articles;
        private ObservableCollection<Articulo> _providers;
        private int _price;
        private string _quantity;
        private int _total;
        private ObservableCollection<Articulo> _vouncher;
        private string _numComprobante;
        private Articulo _article;

        public DataTable TableArticle
        {
            get => _tableArticle;
            set => SetProperty(ref _tableArticle, value);
        }

        public Articulo Article
        {
            get { return _article; }
            set { SetProperty(ref _article, value); }
        }

        public string Quantity
        {
            get { return _quantity; }
            set { SetProperty(ref _quantity, value); }
        }

        public int Price
        {
            get { return _price; }
            set { SetProperty(ref _price, value); }
        }

        public int Total
        {
            get { return _total; }
            set { SetProperty(ref _total, value); }
        }


        public string NumComprobante
        {
            get { return _numComprobante; }
            set { SetProperty(ref _numComprobante, value); }
        }

        public Articulo Provider { get; set; }
        public Articulo Vouncher { get; set; }

        public ObservableCollection<Articulo> Vounchers
        {
            get { return _vouncher; }
            set { SetProperty(ref _vouncher, value); }
        }

        public ObservableCollection<Articulo> Providers
        {
            get { return _providers; }
            set
            {
                SetProperty(ref _providers, value);
            }
        }

        public ObservableCollection<Articulo> Articles
        {
            get { return _articles; }
            set
            {
                SetProperty(ref _articles, value);
            }
        }

        public DelegateCommand AddCommand { get; set; }
        public DelegateCommand CancelCommand { get; set; }


        public ComprasViewModel()
        {
            Vounchers = new ObservableCollection<Articulo>();


            AddCommand = new DelegateCommand(Add, CanAdd);
            CancelCommand = new DelegateCommand(Cancel, CanCancel);
            ConectionTables();
        }


        private bool CanCancel()
        {
            return true;
        }

        private void Cancel()
        {
        }

        private bool CanAdd()
        {
            return true;
        }

        private void Add()
        {
            int _idCompra = 0;
            using (var conn = new SqlConnection(LoginViewModel.ConectionBD))
            {
                SqlCommand cmd = null;

                var isExist = ValidationDatosDetalleCompra(conn);

                if (!isExist)
                {
                    try
                    {
                        #region Tabla COMPRA
                        string compra = "INSERT INTO COMPRA" +
                            " (COMPROBANTE , NUM_COMPROBANTE , DESCRIPCION , FECHA, ID_PROVEEDOR, ID_USUARIO)" +
                            " VALUES (@COMPROBANTE, @NUM_COMPROBANTE, @DESCRIPCION, @FECHA, @ID_PROVEEDOR, @ID_USUARIO) " +
                            "SELECT SCOPE_IDENTITY()";

                        cmd = new SqlCommand(compra, conn);

                        cmd.Parameters.AddWithValue("@COMPROBANTE", Vouncher.Name);
                        cmd.Parameters.AddWithValue("@NUM_COMPROBANTE", NumComprobante);
                        cmd.Parameters.AddWithValue("@DESCRIPCION", "Compra");
                        cmd.Parameters.AddWithValue("@FECHA", DateTime.Now);
                        cmd.Parameters.AddWithValue("@ID_PROVEEDOR", Provider.Codigo);
                        cmd.Parameters.AddWithValue("@ID_USUARIO", LoginViewModel.UserBD);

                        _idCompra = Convert.ToInt32(cmd.ExecuteScalar());

                        cmd.Parameters.Clear();

                        #endregion
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show("Error a la hora de insertar la compra", ex.Message);
                    }

                    try
                    {
                        #region tabla DETALLE COMPRA
                        string detalleCompra = $"INSERT INTO DETALLE_COMPRA " +
                                    " (STOCK, CANTIDAD, MEDIDA, PRECIO, ID_ARTICULO, ID_COMPRA)" +
                           " VALUES ( @STOCK, @CANTIDAD, @MEDIDA, @PRECIO, @ID_ARTICULO, @ID_COMPRA)";

                        cmd = new SqlCommand(detalleCompra, conn);
                        cmd.Parameters.AddWithValue("@STOCK", Quantity);
                        cmd.Parameters.AddWithValue("@CANTIDAD", Quantity);
                        cmd.Parameters.AddWithValue("@MEDIDA", "Kilogramos");
                        cmd.Parameters.AddWithValue("@PRECIO", Price);
                        cmd.Parameters.AddWithValue("@ID_ARTICULO", Article.Codigo);
                        cmd.Parameters.AddWithValue("@ID_COMPRA", _idCompra);

                        cmd.ExecuteNonQuery();

                        cmd.Parameters.Clear();
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show("Error a la hora de insertar el detalle de la compra", ex.Message);
                    }

                }
                else
                {
                    #region  ARTICULO

                    try
                    {
                        var stock = Stock(conn);

                        UpdateStock(conn, stock);
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show("Error a la hora de actualizar el stock", ex.Message);
                    }


                    #endregion
                }

                DetailsTable();
            }
        }

        private bool ValidationDatosDetalleCompra(SqlConnection conn)
        {
            conn.Open();

            var sum = $"SELECT ID FROM DETALLE_COMPRA WHERE ID_ARTICULO={Article.Codigo}";
            SqlCommand command = new SqlCommand(sum, conn);

            SqlDataReader registros = command.ExecuteReader();
            command.Parameters.Clear();
            if (registros.Read())
            {
                registros.Close();
                return true;
            }
            registros.Close();
            return false;
        }

        private void UpdateStock(SqlConnection conn, int stock)
        {
            SqlCommand comand = null;
            int stockValue = int.Parse(Quantity) + stock;


            var updateDetalleCompra = "UPDATE DETALLE_COMPRA SET STOCK=@STOCK WHERE ID_ARTICULO = @ID";
            comand = new SqlCommand(updateDetalleCompra, conn);

            comand.Parameters.AddWithValue("@STOCK", stockValue);
            comand.Parameters.AddWithValue("@ID", Article.Codigo);
            comand.ExecuteNonQuery();
            comand.Parameters.Clear();


            var UpdateArticle = "UPDATE ARTICULO SET STOCK = @STOCK  WHERE ID = @ID";

            comand = new SqlCommand(UpdateArticle, conn);

            comand.Parameters.AddWithValue("@STOCK", stockValue);
            comand.Parameters.AddWithValue("@ID", Article.Codigo);

            comand.ExecuteNonQuery();
            comand.Parameters.Clear();


        }

        private int Stock(SqlConnection conn)
        {
            int dato = 0;
            var sum = $"SELECT STOCK FROM ARTICULO WHERE ID={Article.Codigo}";
            SqlCommand command = new SqlCommand(sum, conn);
            SqlDataReader myReader = command.ExecuteReader();

            while (myReader.Read())
            {
                dato = (int)myReader["STOCK"];
                myReader.Close();
                return dato;
            }

            myReader.Close();
            return 0;
        }

        private void ConectionTables()
        {
            ArticleOpen();

            ProveedorOpen();

            DetailsTable();

            Vounchers.Add(new Articulo { Codigo = "0001", Name = "FACTURA" });
            Vounchers.Add(new Articulo { Codigo = "0002", Name = "RECIBO" });
            Vounchers.Add(new Articulo { Codigo = "0003", Name = "CONTADO" });
        }

        private void DetailsTable()
        {
            using (var conn = new SqlConnection(LoginViewModel.ConectionBD))
            {
                DataTable dt = new DataTable();
                string query = "SELECT DETALLE_COMPRA.ID, DETALLE_COMPRA.ID_ARTICULO, DETALLE_COMPRA.STOCK ,DETALLE_COMPRA.MEDIDA, ARTICULO.NOMBRE AS ARTICULO" +
                    " FROM ARTICULO " +
                    "INNER JOIN DETALLE_COMPRA ON ARTICULO.ID = DETALLE_COMPRA.ID_ARTICULO";

                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                TableArticle = dt;
            }
        }

        private void ProveedorOpen()
        {
            using (var conn = new SqlConnection(LoginViewModel.ConectionBD))
            {
                conn.Open();
                Providers = new ObservableCollection<Articulo>();
                string query = "SELECT * FROM PROVEEDOR ";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    Providers.Add(new Articulo
                    {
                        Codigo = dr[0].ToString(),
                        Name = dr[1].ToString()
                    });
                }

                conn.Close();
            }
        }

        private void ArticleOpen()
        {
            using (var conn = new SqlConnection(LoginViewModel.ConectionBD))
            {
                conn.Open();
                Articles = new ObservableCollection<Articulo>();
                string query = "SELECT * FROM ARTICULO ";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    Articles.Add(new Articulo
                    {
                        Codigo = dr[0].ToString(),
                        Name = dr[1].ToString()
                    });
                }

                conn.Close();
            }
        }
    }

    public class Articulo
    {
        public string Codigo { get; set; }
        public string Name { get; set; }

    }
}
