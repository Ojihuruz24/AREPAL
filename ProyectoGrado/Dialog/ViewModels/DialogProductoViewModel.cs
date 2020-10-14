using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using ProyectoGrado.Events;
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
        private readonly IEventAggregator _eventAggregator;

        public DataRowView ProductSelected
        {
            get { return _productSelected; }
            set { SetProperty(ref _productSelected, value); }
        }

        public DelegateCommand SendCommand { get; }

        

        public DialogProductoViewModel(IEventAggregator eventAggregator)
        {
            ConectionTable();
            SendCommand = new DelegateCommand(Send, CanSend);
            _eventAggregator = eventAggregator;

        }

        private bool CanSend()
        {
            return true;
        }

        private void Send()
        {
            _eventAggregator.GetEvent<ProductSelectedEvent>().Publish(ProductSelected);
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
                string query = "SELECT PRODUCTO.ID, PRODUCTO.CODIGO, PRODUCTO.NOMBRE ,PRODUCTO.PRECIO, CATEGORIA.NOMBRE AS CATEGORIA" +
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
