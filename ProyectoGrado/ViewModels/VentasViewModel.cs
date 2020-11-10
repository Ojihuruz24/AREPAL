using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using ProyectoGrado.Dialog;
using ProyectoGrado.Dialog.ViewModels;
using ProyectoGrado.Dialog.Views;
using ProyectoGrado.Events;
using ProyectoGrado.Models;
using ProyectoGrado.Reportings;
using ProyectoGrado.Reportings.ViewModels;
using ProyectoGrado.Services;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;

namespace ProyectoGrado.ViewModels
{
    public class VentasViewModel : BindableBase
    {
        private string _code;
        private string _product;
        private int _quantity = 0;
        private int _price;
        private string _client;
        private string _number;
        private string _clientName;
        private int _total;
        private ObservableCollection<Venta> _ventas;
        private readonly IEventAggregator _eventAggregator;
        private readonly IVentasService _ventasService;

        public int ProductValue { get; set; }

        public ObservableCollection<Venta> Ventas
        {
            get { return _ventas; }
            set 
            {
                SetProperty(ref _ventas, value);
                PrintCommand.RaiseCanExecuteChanged(); 
            }
        }

        public string NameProduct { get; set; }

        public string Code
        {
            get { return _code; }
            set { SetProperty(ref _code, value); }
        }

        public string Product
        {
            get { return _product; }
            set
            {
                SetProperty(ref _product, value);
            }
        }

        public int Quantity
        {
            get { return _quantity; }
            set
            {
                    SetProperty(ref _quantity, value);
                    Price = ProductValue * value;
                    AddProductCommand.RaiseCanExecuteChanged();
            }
        }

        public int Price
        {
            get { return _price; }
            set { SetProperty(ref _price, value); }
        }

        public string Client
        {
            get { return _client; }
            set { SetProperty(ref _client, value); }
        }

        public string Number
        {
            get { return _number; }
            set { SetProperty(ref _number, value); }
        }

        public string ClientName
        {
            get { return _clientName; }
            set
            { 
                SetProperty(ref _clientName, value);
                PrintCommand.RaiseCanExecuteChanged();
            }
        }

        public int Total
        {
            get { return _total; }
            set { SetProperty(ref _total, value); }
        }

        public DelegateCommand AddProductCommand { get; }
        public DelegateCommand PrintCommand { get; }
        public DelegateCommand CancelProductCommand { get; }
        public DelegateCommand SearchCodeCommand { get; }
        public DelegateCommand AddClientCommand { get; }

        public VentasViewModel(IEventAggregator eventAggregator, IVentasService ventasService)
        {
            _eventAggregator = eventAggregator;
            _ventasService = ventasService;
            AddProductCommand = new DelegateCommand(AddProduct, CanAddProduct);
            PrintCommand = new DelegateCommand(Print, CanPrint);
            CancelProductCommand = new DelegateCommand(CancelProduct, CanCancelProduct);
            SearchCodeCommand = new DelegateCommand(SearchCode, CanSearchCode);
            AddClientCommand = new DelegateCommand(AddClient, CanAddClient);
            Ventas = new ObservableCollection<Venta>();
            _eventAggregator.GetEvent<ProductSelectedEvent>().Subscribe(OnProductSelected);
            _eventAggregator.GetEvent<ClientSelectedEvent>().Subscribe(OnClientSelected);
        }

        private void OnClientSelected(DataRowView client)
        {
            Client = $"{client.Row[0]}";
            ClientName = $"{client.Row[1]}";
            Number = $"{client.Row[3]}";
        }

        private bool CanAddClient()
        {
            return true;
        }

        private void AddClient()
        {
            DialogClienteView dialogClienteView = new DialogClienteView();
            dialogClienteView.DataContext = new DialogClienteViewModel(_eventAggregator);
            dialogClienteView.ShowDialog();
        }

        private void OnProductSelected(DataRowView product)
        {
            Code = product.Row[0].ToString();
            Product = product.Row[1].ToString();
            NameProduct = product.Row[2].ToString();
            ProductValue = int.Parse(product.Row[3].ToString());
        }

        private bool CanSearchCode()
        {
            return true;
        }

        private void SearchCode()
        {
            DialogProductoView dialogProductoView = new DialogProductoView();
            dialogProductoView.DataContext = new DialogProductoViewModel(_eventAggregator);
            dialogProductoView.ShowDialog();
        }

        private bool CanCancelProduct()
        {
            return true;
        }

        private void CancelProduct()
        {
            Ventas.Clear();
            Total = 0;
            Clear();
        }

        private bool CanPrint()
        {
            if (!string.IsNullOrEmpty(ClientName) && Ventas.Count > 0)
            {
                return true;
            }
            return false;
        }

        private void Print()
        {
            int _idVenta = 0;
            using (var conn = new SqlConnection(LoginViewModel.ConectionBD))
            {
                SqlCommand cmd = null;
                try
                {
                    conn.Open();

                    #region Tabla venta
                    string venta = "INSERT INTO VENTA" +
                        " (ID_USUARIO , VALOR_TOTAL , ESTADO , FECHA)" +
                        " VALUES (@ID_USUARIO, @VALOR_TOTAL, @ESTADO, @FECHA) SELECT SCOPE_IDENTITY()";

                    cmd = new SqlCommand(venta, conn);

                    cmd.Parameters.AddWithValue("@ID_USUARIO", Client);
                    cmd.Parameters.AddWithValue("@VALOR_TOTAL", Total);
                    cmd.Parameters.AddWithValue("@ESTADO", "Pagado");
                    cmd.Parameters.AddWithValue("@FECHA", DateTime.Now);

                    _idVenta = Convert.ToInt32(cmd.ExecuteScalar());
                    cmd.Parameters.Clear();

                    #endregion

                    #region detalle venta
                    string detalle_venta;

                    foreach (var item in Ventas)
                    {
                        detalle_venta = "INSERT INTO DETALLE_VENTA" +
                        " (PRECIO, DESCUENTO, ID_PRODUCTO , CANTIDAD, ID_VENTA)" +
                        " VALUES (@PRECIO, @DESCUENTO, @ID_PRODUCTO, @CANTIDAD, @ID_VENTA) SELECT SCOPE_IDENTITY() ";
                        cmd = new SqlCommand(detalle_venta, conn);

                        cmd.Parameters.AddWithValue("@PRECIO", item.SubTotal);
                        cmd.Parameters.AddWithValue("@DESCUENTO", 0);
                        cmd.Parameters.AddWithValue("@ID_PRODUCTO", item.IdProduct);
                        cmd.Parameters.AddWithValue("@CANTIDAD", item.Quantity);
                        cmd.Parameters.AddWithValue("@ID_VENTA", _idVenta);

                        cmd.ExecuteNonQuery();
                    }

                    cmd.Parameters.Clear();
                    #endregion

                    #region Detalle Producto

                    foreach (var item in Ventas)
                    {
                        string PROBANDO = $"UPDATE ARTICULO SET ARTICULO.STOCK = ARTICULO.STOCK - (DETALLE_PRODUCTO.CANTIDAD * {item.Quantity} ) " +
                                     "FROM VENTA " +
                                     "INNER JOIN DETALLE_VENTA " +
                                     "INNER JOIN DETALLE_PRODUCTO " +
                                     "INNER JOIN ARTICULO " +
                                     "ON DETALLE_PRODUCTO.ID_ARTICULO = ARTICULO.ID " +
                                     "INNER JOIN PRODUCTO ON DETALLE_PRODUCTO.ID_PRODUCTO = PRODUCTO.ID " +
                                     "ON DETALLE_VENTA.ID_PRODUCTO = PRODUCTO.ID " +
                                     "ON VENTA.ID = DETALLE_VENTA.ID_VENTA " +
                                     $"WHERE VENTA.ID = {_idVenta} AND DETALLE_PRODUCTO.ID_PRODUCTO = {item.IdProduct} "; 

                    cmd = new SqlCommand(PROBANDO, conn);

                    cmd.ExecuteNonQuery();

                    cmd.Parameters.Clear();

                    }



                    #endregion


                    // Este es el insert para agregar a la tabla de VENTAS_CLIENTE para registrar la id de la venta y el id cliente
                    #region tabla Venta cliente
                    string venta_cliente = "INSERT INTO VENTAS_CLIENTE " +
                                " (ID_VENTA ,ID_CLIENTE) VALUES (@ID_VENTA ,@Client)";

                    cmd = new SqlCommand(venta_cliente, conn);
                    cmd.Parameters.AddWithValue("@ID_VENTA", _idVenta);
                    cmd.Parameters.AddWithValue("@Client", Client);

                    cmd.ExecuteNonQuery();

                    cmd.Parameters.Clear();
                    #endregion  

                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("Error a la hora de insertar el cliente", ex.Message);
                }
            }


            _ventasService.Cedula = Client;
            _ventasService.Cedula = Client;
            _ventasService.Total = Total;
            _ventasService.IdVenta = _idVenta;
            _ventasService.NameClient = ClientName;
            _ventasService.Tel = Number;
            _ventasService.Ventas = Ventas.ToList();

            ReportVentView reportVentView = new ReportVentView();
            reportVentView.DataContext = new ReportVentViewModel(_ventasService);
            reportVentView.ShowDialog();

            Clear();

            Ventas.Clear();
            Total = 0;

            #region Commemt
            //using (var conn = new SqlConnection(LoginViewModel.ConectionBD))
            //{
            //    try
            //    {
            //        conn.Open();

            //        foreach (var item in Ventas)
            //        {
            //            string query = "INSERT INTO VENTA (ID_PRODUCTO, NOMBRE , CANTIDAD, PRECIO, CLIENTE) VALUES (@Id_producto, @Name, @Quantity, @Price, @Client)";
            //            SqlCommand cmd = new SqlCommand(query, conn);

            //            cmd.Parameters.AddWithValue("@Id_producto", item.Code);
            //            cmd.Parameters.AddWithValue("@Name", item.Product);
            //            cmd.Parameters.AddWithValue("@Quantity", item.Quantity);
            //            cmd.Parameters.AddWithValue("@Price", item.Price);
            //            cmd.Parameters.AddWithValue("@Client", Client);
            //            cmd.ExecuteNonQuery();
            //        }
            //        conn.Close();
            //    }
            //    catch (Exception ex)
            //    {
            //        System.Windows.Forms.MessageBox.Show("Error al insertar la venta: ", ex.Message);
            //    }
            //    finally
            //    {
            //        PrintDocument document = new PrintDocument();
            //        PrinterSettings settings = new PrinterSettings();
            //        document.PrinterSettings = settings;
            //        document.PrintPage += Imprimir;
            //        document.Print();
            //    }
            //}

            //PrintDocument document = new PrintDocument();
            //PrinterSettings settings = new PrinterSettings();
            //document.PrinterSettings = settings;
            //document.PrintPage += Imprimir;
            //document.Print(); 
            #endregion
        }

        private void Imprimir(object sender, PrintPageEventArgs e)
        {

            Console.WriteLine("{0,6} {1,20} {2,20} {3,20} {4,30}",
                              "Codigo", "Producto", "Cantidad", "Preico", "Cliente");

            foreach (var item in Ventas)
            {
                var datusua = ("{0,6} {1,20} {2,20} {3,20} {4,30}",
                                 item.IdProduct, item.NameProduct, item.Quantity, item.SubTotal, Client);
                Console.WriteLine(datusua);
            }



            Font font = new Font("Arial", 14);
            int ancho = 700;
            int y = 20;

            e.Graphics.DrawString("--------FACTURA AREPAL--------", font, Brushes.Black, new RectangleF(0, y += 20, ancho, 20));


            e.Graphics.DrawString($"Cliente: {Client}", font, Brushes.Black, new RectangleF(0, y += 20, ancho, 20));
            e.Graphics.DrawString(string.Format("{0,6} {1,10} {2,10} {3,18}",
                              "Codigo", "Producto", "Cantidad", "Preico"), font, Brushes.Black, new RectangleF(0, y += 20, ancho, 20));


            foreach (var venta in Ventas)
            {
                e.Graphics.DrawString(string.Format("{0,6} {1,15} {2,10} {3,18} ",
                                     venta.IdProduct, venta.NameProduct, venta.Quantity, venta.SubTotal), font, Brushes.Black, new RectangleF(0, y += 20, ancho, 20));
                //e.Graphics.DrawString($"{venta.Code} - {venta.Product} - {venta.Quantity} - {venta.Price} ", font, Brushes.Black, new RectangleF(0, y += 20, ancho, 20));
            }


            e.Graphics.DrawString($"------TOTAL A PAGAR: {Total}", font, Brushes.Black, new RectangleF(0, y += 30, ancho, 20));

            e.Graphics.DrawString($"------GRACIAS POR TU COMPRA-------", font, Brushes.Black, new RectangleF(0, y += 40, ancho, 20));

        }

        private bool CanAddProduct()
        {
            if (Price == 0)
            {
                return false;
            }
            return true;
        }

        private void AddProduct()
        {
            Ventas.Add(new Venta { IdProduct = Code, NameProduct = NameProduct, Quantity = Quantity, SubTotal = Price, Client = Client });
            Total = Total + Price;
            Clear();
            PrintCommand.RaiseCanExecuteChanged();
        }

        private void Clear()
        {
            Code = string.Empty;
            Product = string.Empty;
            Quantity = 0;
            Price = 0;
            ProductValue = 0;
            ClientName = string.Empty;
        }
    }

    public class PrintText
    {
        public PrintText(string text, Font font) : this(text, font, new StringFormat()) { }

        public PrintText(string text, Font font, StringFormat stringFormat)
        {
            Text = text;
            Font = font;
            StringFormat = stringFormat;
        }

        public string Text { get; set; }

        public Font Font { get; set; }

        /// <summary> Default is horizontal string formatting </summary>
        public StringFormat StringFormat { get; set; }
    }
}
