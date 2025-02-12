﻿using Prism.Commands;
using Prism.Mvvm;
using ProyectoGrado.Utility.Validations;
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
        private string _directionUser;
        private string _searchClient;

        public DataTable TableClient
        {
            get => _tableClient;
            set => SetProperty(ref _tableClient, value);
        }
        public string DocumentUser
        {
            get => _documentUser;
            set
            {
                if (ValidationesInput.IsNumber(value, "Documento Invalido"))
                {
                    SetProperty(ref _documentUser, value);
                }
            }
        }

        public string SearchClient
        {
            get => _searchClient;
            set
            {
                SetProperty(ref _searchClient, value);

                if (!string.IsNullOrEmpty(value))
                {
                    TableClient.DefaultView.RowFilter = string.Format($"ID = {SearchClient}");
                    return;
                }
                ConectionTable();
            }
        }

        public string NameUser
        {
            get => _nameUser;
            set
            {
                if (ValidationesInput.IsString(value, "Nombre Invalido"))
                {
                    SetProperty(ref _nameUser, value);
                }
            }
        }

        public string DirectionUser
        {
            get => _directionUser;
            set
            {
                SetProperty(ref _directionUser, value);
            }
        }

        public string TelUser
        {
            get => _telUser;
            set
            {
                if (ValidationesInput.IsNumber(value, "Teléfono Invalido"))
                {
                    SetProperty(ref _telUser, value);
                }
            }
        }

        public DelegateCommand AddCommand { get; set; }
        public DelegateCommand CancelCommand { get; set; }

        public ClientesViewModel()
        {
            AddCommand = new DelegateCommand(Add, CanAdd)
                .ObservesProperty(() => DocumentUser)
                .ObservesProperty(() => NameUser)
                .ObservesProperty(() => DirectionUser)
                .ObservesProperty(() => TelUser);
            CancelCommand = new DelegateCommand(Cancel, CanCancel);
            ConectionTable();
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
            if (string.IsNullOrWhiteSpace(DocumentUser) || string.IsNullOrWhiteSpace(NameUser)
                || string.IsNullOrWhiteSpace(DirectionUser) || string.IsNullOrWhiteSpace(TelUser))
            {
                return false;
            }
            return true;
        }

        private void Add()
        {
            using (var conn = new SqlConnection(LoginViewModel.ConectionBD))
            {
                try
                {
                    string query = "INSERT INTO CLIENTE (ID, NOMBRE , DIRECCION, TELEFONO)" +
                        " VALUES (@DocumentUser, @NameUser, @DirectionUser, @TelUser)";
                    SqlCommand cmd = new SqlCommand(query, conn);

                    conn.Open();

                    cmd.Parameters.AddWithValue("@DocumentUser", DocumentUser);
                    cmd.Parameters.AddWithValue("@NameUser", NameUser);
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
