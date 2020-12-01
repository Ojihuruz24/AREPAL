using Prism.Mvvm;
using ProyectoGrado.Models;
using ProyectoGrado.Utility.Validations;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Data;

namespace ProyectoGrado.ViewModels
{
    public class ProductosViewModel : BindableBase
    {
        private ObservableCollection<Producto> _products;
        private string _searchProduct = "";
        private ICollectionView _filteredSearch;

        public ObservableCollection<Producto> Products
        {
            get { return _products; }
            set { SetProperty(ref _products, value); }
        }

        public string SearchProduct
        {
            get { return _searchProduct; }
            set
            {
                if (_searchProduct != value)
                {
                    SetProperty(ref _searchProduct, value);
                    _filteredSearch.Refresh(); 
                }
            }
        }

        public ProductosViewModel()
        {
            Products = new ObservableCollection<Producto>();
            SetFilteredProvider();
            ConectionTable();
        }


        public void SetFilteredProvider()
        {
            _filteredSearch = CollectionViewSource.GetDefaultView(Products);
            _filteredSearch.Filter = Filter;
        }

        private bool Filter(object obj)
        {
            if (obj is Producto prodcuto)
            {
                var search = $"{prodcuto.Codigo} {prodcuto.Nombre} {prodcuto.Categoria}";

                return search.ToLower().Contains(SearchProduct.ToLower());
            }

            return false;
        }


        private void ConectionTable()
        {
            using (var conn = new SqlConnection(LoginViewModel.ConectionBD))
            {
                try
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
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    Products.Clear();

                    while (reader.Read())
                    {

                        Products.Add(new Producto
                        {
                            Codigo = reader.GetString(1),
                            Nombre = reader.GetString(2),
                            Precio = reader.GetInt32(3),
                            Categoria = reader.GetString(4)
                        });

                    }

                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                }
            }
        }
    }
}
