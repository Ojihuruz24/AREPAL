﻿using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
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
using ProyectoGrado.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Forms;

namespace ProyectoGrado.ViewModels
{
    public class VentasViewModel : BindableBase
    {
        private string _code;
        private string _product;
        private int _quantity = 0;
        private int _price;
        private string _client;
        private int _total;
        private ObservableCollection<Venta> _ventas;
        private readonly IEventAggregator _eventAggregator;
        private readonly IVentasService _ventasService;

        public int ProductValue { get; set; }

        public ObservableCollection<Venta> Ventas
        {
            get { return _ventas; }
            set { SetProperty(ref _ventas, value); }
        }

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
            Ventas = new ObservableCollection<Venta>();
            AddProductCommand = new DelegateCommand(AddProduct, CanAddProduct);
            PrintCommand = new DelegateCommand(Print, CanPrint);
            CancelProductCommand = new DelegateCommand(CancelProduct, CanCancelProduct);
            SearchCodeCommand = new DelegateCommand(SearchCode, CanSearchCode);
            AddClientCommand = new DelegateCommand(AddClient, CanAddClient);
            _eventAggregator.GetEvent<ProductSelectedEvent>().Subscribe(OnProductSelected);
            _eventAggregator.GetEvent<ClientSelectedEvent>().Subscribe(OnClientSelected);
        }

        private void OnClientSelected(DataRowView client)
        {
            Client = $"{client.Row[1]}  { client.Row[2]}";
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
            ProductValue = int.Parse(product.Row[2].ToString());
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
            Clear();
        }

        private bool CanPrint()
        {
            return true;
        }

        private void Print()
        {
            _ventasService.Client = Client;
            _ventasService.Total = Total;
            _ventasService.Ventas = Ventas.ToList();

            ReportVentView reportVentView = new ReportVentView();
            reportVentView.DataContext = new ReportVentViewModel(_ventasService);
            reportVentView.ShowDialog();

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
            Ventas.Add(new Venta { IdProduct = Code, NameProduct = Product, Quantity = Quantity, SubTotal = Price, Client = Client});
            Total = Total + Price;
            Clear();
        }

        private void Clear()
        {
            Code = string.Empty;
            Product = string.Empty;
            Quantity = 0;
            Price = 0;
            ProductValue = 0;
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
