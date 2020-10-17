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
    public class ProveedoresViewModel : BindableBase
    {
        private DataTable _tableProvider;

        public string DocumentUser { get; set; }
        public string RazonSocial { get; set; }
        public ObservableCollection<TipoDocuments> TypeDocuments { get; set; } = new ObservableCollection<TipoDocuments>();

        public TipoDocuments TypeDocument { get; set; }

        public DataTable TableProvider
        {
            get { return _tableProvider; }
            set { SetProperty(ref _tableProvider, value); }
        }

        public string NumberDocument { get; set; }
        public string TelUser { get; set; }
        private string _searchProvider;

        public string SearchProvider
        {
            get { return _searchProvider; }
            set
            {
                SetProperty(ref _searchProvider, value);
                Search(value);
            }
        }

        public DelegateCommand AddProviderCommand { get; set; }
        public DelegateCommand CancelProviderCommand { get; set; }

        public ProveedoresViewModel()
        {
            AddProviderCommand = new DelegateCommand(AddProvider, CanAddProvider);
            CancelProviderCommand = new DelegateCommand(CancelProvider, CanCancelProvider);

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
            return true;
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

        private void Search(string search)
        {
            if (!string.IsNullOrEmpty(search))
            {
                TableProvider.DefaultView.RowFilter = $"NUM_DOCUMENTO = {SearchProvider}";
                return;
            }
            AddTableProvider();
        }

        private void AddTableProvider()
        {
            using (var conn = new SqlConnection(LoginViewModel.ConectionBD))
            {
                try
                {
                    DataTable dt = new DataTable();
                    var sql = "SELECT * FROM PROVEEDOR";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                    TableProvider = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al traer los proveedores de la base de datos", ex.Message);
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
