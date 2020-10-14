using ProyectoGrado.Models;
using System.Collections.Generic;

namespace ProyectoGrado.Services
{
    public interface IVentasService
    {
        string Cedula { get; set; }
        string Tel { get; set; }
        string NameClient { get; set; }
        int Total { get; set; }
        int IdVenta { get; set; }
        List<Venta> Ventas { get; set; }
    }
}