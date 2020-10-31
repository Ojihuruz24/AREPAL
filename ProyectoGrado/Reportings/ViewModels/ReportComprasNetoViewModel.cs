using Prism.Mvvm;
using ProyectoGrado.Services.ReportNeto;
using ProyectoGrado.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoGrado.Reportings.ViewModels
{
    public class ReportComprasNetoViewModel : BindableBase
    {
        private DataTable _comprasNeto;
        private readonly string _dateInitial;
        private readonly string _dateEnd;

        public DateTime FechaNow { get; set; }
        public string User { get; set; }

        public ReportComprasNetoViewModel(IReportNetosService reportNetosService)
        {
            _dateInitial = reportNetosService.DateInitial;
            _dateEnd = reportNetosService.DateEnd;
            ConectionTableCompra();
            FechaNow = DateTime.Now;
       
        }

        public DataTable ComprasNeto 
        { 
            get => _comprasNeto; 
            set =>SetProperty(ref _comprasNeto,value); 
        }

        private void ConectionTableCompra()
        {
            DataTable dt = new DataTable();
            using (var conn = new SqlConnection(LoginViewModel.ConectionBD))
            {
                string query = "SELECT " +
                    "DETALLE_COMPRA.CANTIDAD," +
                    "ARTICULO.NOMBRE AS ARTICULO," +
                    "DETALLE_COMPRA.PRECIO," +
                    "COMPRA.COMPROBANTE AS COMPROBANTE," +
                    "COMPRA.NUM_COMPROBANTE AS 'NUMERO_COMPROBANTE'," +
                    "PROVEEDOR.RAZON_SOCIAL AS PROVEEDOR," +
                    "COMPRA.FECHA " +
                    "FROM DETALLE_COMPRA " +
                    "INNER JOIN COMPRA ON COMPRA.ID = DETALLE_COMPRA.ID_COMPRA " +
                    "INNER JOIN PROVEEDOR ON PROVEEDOR.ID = COMPRA.ID_PROVEEDOR " +
                    "INNER JOIN ARTICULO ON ARTICULO.ID = DETALLE_COMPRA.ID_ARTICULO " +
                    $"WHERE COMPRA.FECHA BETWEEN '{_dateInitial}' AND '{_dateEnd}' " +
                    "ORDER BY COMPRA.FECHA";

                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                ComprasNeto = dt;
                User = LoginViewModel.UserBD;
            }
        }
    }
}
