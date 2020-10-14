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
    public class DialogClienteViewModel : BindableBase
    {

        private DataTable _clients;
        private string _searchClient;
        private DataRowView _clientSelected;
        private readonly IEventAggregator _eventAggregator;

        public DataTable Clients
        {
            get { return _clients; }
            set { SetProperty(ref _clients, value); }
        }

        public string SearchClient
        {
            get { return _searchClient; }
            set
            {
                SetProperty(ref _searchClient, value);
                Search(value);
            }
        }

        public DataRowView ClientSelected
        {
            get { return _clientSelected; }
            set { SetProperty(ref _clientSelected, value); }
        }

        public DelegateCommand SendCommand { get; }


        public DialogClienteViewModel(IEventAggregator eventAggregator)
        {
            ConectionTable();
            _eventAggregator = eventAggregator;
            SendCommand = new DelegateCommand(Send, CanSend);
        }

        private bool CanSend()
        {
            return true;
        }

        private void Send()
        {
            _eventAggregator.GetEvent<ClientSelectedEvent>().Publish(ClientSelected);
        }

        private void Search(string search)
        {
            if (!string.IsNullOrEmpty(search))
            {
                Clients.DefaultView.RowFilter = $"ID = {SearchClient}";
                return;
            }

            ConectionTable();
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
                Clients = dt;
            }

        }
    }
}
