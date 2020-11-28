using Prism.Commands;
using Prism.Mvvm;
using ProyectoGrado.Models;
using ProyectoGrado.Utility.Validations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace ProyectoGrado.ViewModels
{
    public class ProveedoresViewModel : BindableBase
    {
        private ObservableCollection<Proveedor> _tableProvider;
        private ICollectionView _filteredSearch;
        private string _searchProvider = "";
        private string _razonSocial;
        private string _numberDocument;
        private string _telUser;
        private TipoDocuments typeDocument;
        private ObservableCollection<TipoDocuments> typeDocuments;

        public string RazonSocial
        {
            get => _razonSocial;

            set
            {
                if (ValidationesInput.IsString(value, "Razon social invalida"))
                {
                    SetProperty(ref _razonSocial, value);
                }
            }
        }

        public string TelUser
        {
            get => _telUser;
            set
            {
                if (ValidationesInput.IsNumber(value, "Télefono invalido"))
                {
                    SetProperty(ref _telUser, value);
                }
            }
        }

        public ObservableCollection<TipoDocuments> TypeDocuments
        {
            get
            {
                return typeDocuments;
            }

            set
            {
                SetProperty(ref typeDocuments, value);
            }
        }

        public TipoDocuments TypeDocument
        {
            get => typeDocument;
            set
            {
                SetProperty(ref typeDocument, value);
            }
        }

        public ObservableCollection<Proveedor> TableProvider
        {
            get { return _tableProvider; }
            set { SetProperty(ref _tableProvider, value); }
        }

        public string NumberDocument
        {
            get => _numberDocument;
            set
            {

                if (ValidationesInput.IsNumber(value, "Documento invalido"))
                {
                    SetProperty(ref _numberDocument, value);
                }
            }
        }

        public string SearchProvider
        {
            get { return _searchProvider; }
            set
            {
                if (_searchProvider != value)
                {
                    SetProperty(ref _searchProvider, value);
                    _filteredSearch.Refresh();
                }
            }
        }

        public DelegateCommand AddProviderCommand { get; set; }
        public DelegateCommand CancelProviderCommand { get; set; }

        public ProveedoresViewModel()
        {
            TableProvider = new ObservableCollection<Proveedor>();
            SetFilteredProvider();
            AddProviderCommand = new DelegateCommand(AddProvider, CanAddProvider)
                .ObservesProperty(() => RazonSocial)
                .ObservesProperty(() => TelUser)
                .ObservesProperty(() => TypeDocument)
                .ObservesProperty(() => NumberDocument);
            CancelProviderCommand = new DelegateCommand(CancelProvider, CanCancelProvider);
            TypeDocuments = new ObservableCollection<TipoDocuments>();
            AddTypesDocument();
            AddTableProvider();
        }

        private bool CanCancelProvider()
        {
            return true;
        }

        private void CancelProvider()
        {
            Clear();
        }

        private bool CanAddProvider()
        {
            if (!string.IsNullOrWhiteSpace(TelUser)
                && !string.IsNullOrWhiteSpace(NumberDocument)
                && !string.IsNullOrWhiteSpace(RazonSocial)
                && TypeDocument != null)
            {
                return true;
            }
            return false;
        }

        public void SetFilteredProvider()
        {
            _filteredSearch = CollectionViewSource.GetDefaultView(TableProvider);
            _filteredSearch.Filter = Filter;
        }

        private bool Filter(object obj)
        {
            if (obj is Proveedor provider)
            {
                return provider.NumberDocument.ToLower().Contains(SearchProvider.ToLower());
            }

            return false;
        }

        private void AddProvider()
        {
            using (var conn = new SqlConnection(LoginViewModel.ConectionBD))
            {
                try
                {
                    string query = "INSERT INTO PROVEEDOR (RAZON_SOCIAL,TIPO_DOCUMENTO, NUM_DOCUMENTO, TELEFONO)" +
                               " VALUES (@RAZON_SOCIAL, @TIPO_DOCUMENTO, @NUM_DOCUMENTO, @TELEFONO)";
                    SqlCommand sql = new SqlCommand(query, conn);

                    conn.Open();

                    sql.Parameters.AddWithValue("@RAZON_SOCIAL", RazonSocial);
                    sql.Parameters.AddWithValue("@TIPO_DOCUMENTO", TypeDocument.Tipe);
                    sql.Parameters.AddWithValue("@NUM_DOCUMENTO", NumberDocument);
                    sql.Parameters.AddWithValue("@TELEFONO", TelUser);

                    sql.ExecuteNonQuery();
                    sql.Parameters.Clear();

                    conn.Close();

                    AddTableProvider();

                    Clear();

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al agregar el proveedor a la base de datos", ex.Message);
                }
            }
        }

        private void Clear()
        {
            RazonSocial = string.Empty;
            TypeDocuments = new ObservableCollection<TipoDocuments>();
            AddTypesDocument();
            NumberDocument = string.Empty;
            TelUser = string.Empty;
        }


        private void AddTableProvider()
        {
            using (var conn = new SqlConnection(LoginViewModel.ConectionBD))
            {
                try
                {
                    var sql = "SELECT * FROM PROVEEDOR";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    TableProvider.Clear();

                    while (reader.Read())
                    {
                        TableProvider.Add(new Proveedor
                        {
                            Name = reader.GetString(1),
                            TypeDocument = reader.GetString(2),
                            NumberDocument = reader.GetString(3),
                            Tel = reader.GetString(4),
                        });
                    }

                    conn.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al traer los proveedores de la base de datos", ex.Message);
                    conn.Close();
                }
            }
        }

        private void AddTypesDocument()
        {
            TypeDocuments.Add(new TipoDocuments { Tipe = "CC" });
            TypeDocuments.Add(new TipoDocuments { Tipe = "DNI" });
            TypeDocuments.Add(new TipoDocuments { Tipe = "NIT" });
            TypeDocuments.Add(new TipoDocuments { Tipe = "RFC" });
            TypeDocuments.Add(new TipoDocuments { Tipe = "RUC" });
            TypeDocuments.Add(new TipoDocuments { Tipe = "OTR" });
        }
    }

    public class TipoDocuments
    {
        public string Tipe { get; set; }
    }
}
