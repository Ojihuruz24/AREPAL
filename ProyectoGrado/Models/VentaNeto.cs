using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoGrado.Models
{
    public class VentaNeto
    {
        public int Codigo { get; set; }
        public int Cantidad { get; set; }
        public string Producto { get; set; }
        public double PrecioUnidad { get; set; }
        public double PrecioTotal { get; set; }
        public string Estado { get; set; }
        public string Categoria { get; set; }
        public DateTime Fecha { get; set; }
    }
}
