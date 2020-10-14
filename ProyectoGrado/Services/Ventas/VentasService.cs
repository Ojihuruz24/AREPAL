using ProyectoGrado.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoGrado.Services
{
    public class VentasService : IVentasService
    {

        public List<Venta> Ventas { get; set; }

        public int Price { get; set; }

        public string Cedula { get; set; }
        public string NameClient { get; set; }
        public string Tel { get; set; }

        public int Total { get; set; }
        public int IdVenta { get; set; }

        public VentasService()
        {

        }
    }
}
