using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
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
    public class ClientesViewModel : BindableBase
    {

        private DataTable _tableClient;
        private string _telUser;
        private string _documentUser;
        private string _nameUser;
        private string _surnameUser;
        private string _directionUser;
        private ICollectionView _collectionView;
        private string _searchClient;

        public DataTable TableClient
        {
            get => _tableClient;
            set => SetProperty(ref _tableClient, value);
        }
        public string DocumentUser
        {
            get => _documentUser;
            set => SetProperty(ref _documentUser, value);
        }     
        
        public string SearchClient
        {
            get => _searchClient;
            set => SetProperty(ref _searchClient, value);
        }

        public string NameUser
        {
            get => _nameUser;
            set => SetProperty(ref _nameUser, value);
        }

        public string SurnameUser
        {
            get => _surnameUser;
            set => SetProperty(ref _surnameUser, value);
        }

        public string DirectionUser
        {
            get => _directionUser;
            set => SetProperty(ref _directionUser, value);
        }

        public string TelUser
        {
            get => _telUser;
            set => SetProperty(ref _telUser, value);
        }

        public DelegateCommand AddCommand { get; set; }
        public DelegateCommand CancelCommand { get; set; }
        public DelegateCommand SearchCommand { get; }

        public ClientesViewModel()
        {
            AddCommand = new DelegateCommand(Add, CanAdd);
            CancelCommand = new DelegateCommand(Cancel, CanCancel);
            SearchCommand = new DelegateCommand(Search, CanSearch);
            ConectionTable();
        }

        private bool CanSearch()
        {
            return true;
        }

        private void Search()
        {
            DataTable dt = new DataTable();
            using (var conn = new SqlConnection(LoginViewModel.ConectionBD))
            {

                string query = "select * from CLIENTE WHERE DOCUMENTO = @Documento";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Documento", SearchClient);

                conn.Open();

                cmd.ExecuteNonQuery();

                conn.Close();
            }
        }

        private void InitializeFilter()
        {
            _collectionView = CollectionViewSource.GetDefaultView(TableClient.Columns[0]);
            _collectionView.Filter = o => o is string info && info.Contains(SearchClient.ToLower());
        }


        private bool CanCancel()
        {
            return true;
        }

        private void Cancel()
        {
            Clear();
        }

        private bool CanAdd()
        {
            return true;
        }

        private void Add()
        {
            using (var conn = new SqlConnection(LoginViewModel.ConectionBD))
            {
                try
                {
                    string query = "INSERT INTO CLIENTE (DOCUMENTO, NOMBRE , APELLIDO, DIRECCION, TELEFONO) VALUES (@DocumentUser, @NameUser, @SurnameUser, @DirectionUser, @TelUser)";
                    SqlCommand cmd = new SqlCommand(query, conn);

                    conn.Open();

                    cmd.Parameters.AddWithValue("@DocumentUser", DocumentUser);
                    cmd.Parameters.AddWithValue("@NameUser", NameUser);
                    cmd.Parameters.AddWithValue("@SurnameUser", SurnameUser);
                    cmd.Parameters.AddWithValue("@DirectionUser", DirectionUser);
                    cmd.Parameters.AddWithValue("@TelUser", TelUser);

                    cmd.ExecuteNonQuery();

                    conn.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error a la hora de insertar el cliente", ex.Message);
                }

                ConectionTable();

                Clear();
            }
        }

        private void Clear()
        {
            DocumentUser = string.Empty;
            NameUser = string.Empty;
            SurnameUser = string.Empty;
            DirectionUser = string.Empty;
            TelUser = string.Empty;
        }

        private void ConectionTable()
        {
            DataTable dt = new DataTable();
            using (var conn = new SqlConnection(LoginViewModel.ConectionBD))
            {

                string query = "select * from CLIENTE";

                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                TableClient = dt;
            }


        }

    }
}
