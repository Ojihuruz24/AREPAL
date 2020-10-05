using Microsoft.Win32;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using ProyectoGrado.Dialog;
using ProyectoGrado.Dialog.ViewModels;
using ProyectoGrado.Events;
using ProyectoGrado.Models;
using ProyectoGrado.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public VentasViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            Ventas = new ObservableCollection<Venta>();
            AddProductCommand = new DelegateCommand(AddProduct, CanAddProduct);
            PrintCommand = new DelegateCommand(Print, CanPrint);
            CancelProductCommand = new DelegateCommand(CancelProduct, CanCancelProduct);
            SearchCodeCommand = new DelegateCommand(SearchCode, CanSearchCode);
            _eventAggregator.GetEvent<ProductSelectedEvent>().Subscribe(OnProductSelected);
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
            // aca es donde se guarda en la Base de datos
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
            Ventas.Add(new Venta { Code = Code, Product = Product, Quantity = Quantity, Price = Price, Client = Client });
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
}
