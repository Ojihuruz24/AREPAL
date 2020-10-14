using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoGrado.Models
{
    public class Venta
    {
        public string IdProduct { get; set; }

        public string NameProduct { get; set; }

        public int Quantity { get; set; }

        public int SubTotasl { get; set; }

        public string Client { get; set; }

        public int SubTotal { get; set; }


    }
}
