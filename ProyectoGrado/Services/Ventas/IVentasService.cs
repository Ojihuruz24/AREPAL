using ProyectoGrado.Models;
using System.Collections.Generic;

namespace ProyectoGrado.Services
{
    public interface IVentasService
    {
        string Client { get; set; }
        int Total { get; set; }
        List<Venta> Ventas { get; set; }
    }
}