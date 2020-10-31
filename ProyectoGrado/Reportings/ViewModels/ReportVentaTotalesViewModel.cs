using ProyectoGrado.Models;
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
    public class ReportVentaTotalesViewModel
    {
        public DateTime FechaNow { get; set; }
        public string User { get; set; }

        public ReportVentaTotalesViewModel()
        {
            ConectionTableVenta();
            FechaNow = DateTime.Now;

        }

        public DataTable VentasNeto { get; set; }

        private void ConectionTableVenta()
        {
            DataTable dt = new DataTable();
            using (var conn = new SqlConnection(LoginViewModel.ConectionBD))
            {
                string query = "SELECT" +
                    " DETALLE_VENTA.ID AS CODIGO, " +
                    "DETALLE_VENTA.CANTIDAD," +
                    "PRODUCTO.NOMBRE AS PRODUCTO ," +
                    "PRODUCTO.PRECIO AS 'PRECIOUNIDAD' " +
                    ",DETALLE_VENTA.PRECIO AS 'TOTAL'," +
                    " VENTA.ESTADO, " +
                    " CATEGORIA.NOMBRE AS CATEGORIA, " +
                    "VENTA.FECHA " +
                    "FROM DETALLE_VENTA " +
                    "INNER JOIN VENTA ON VENTA.ID = DETALLE_VENTA.ID_VENTA " +
                    "INNER JOIN PRODUCTO ON PRODUCTO.ID = DETALLE_VENTA.ID_PRODUCTO " +
                    "INNER JOIN CATEGORIA ON CATEGORIA.ID = PRODUCTO.ID_CATEGORIA ORDER BY CODIGO ASC";

                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                VentasNeto = dt;
                User = LoginViewModel.UserBD;
            }
        }
    }
}
