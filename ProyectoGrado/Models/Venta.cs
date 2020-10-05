using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoGrado.Models
{
    public class Venta
    {
        private string _code;
        private string _product;
        private int _quantity;
        private int _price;
        private string _client;

        public string Code
        {
            get { return _code; }
            set { _code = value; }
        }

        public string Product
        {
            get { return _product; }
            set { _product = value; }
        }

        public int Quantity
        {
            get { return _quantity; }
            set { _quantity = value; }
        }

        public int Price
        {
            get { return _price; }
            set { _price = value; }
        }

        public string Client
        {
            get { return _client; }
            set { _client = value; }
        }
    }
}
