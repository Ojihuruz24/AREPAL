using ProyectoGrado.Models;
using System.Collections.Generic;

namespace ProyectoGrado.Services
{
    public interface IVentasService
    {
        string Client { get; set; }
        string Code { get; set; }
        int Price { get; set; }
        string Product { get; set; }
        int ProductValue { get; set; }
        int Quantity { get; set; }
        int Total { get; set; }
        List<Venta> Ventas { get; set; }
    }
}