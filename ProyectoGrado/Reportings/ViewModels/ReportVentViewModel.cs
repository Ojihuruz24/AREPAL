using CommonServiceLocator;
using Prism.Events;
using Prism.Mvvm;
using ProyectoGrado.Events;
using ProyectoGrado.Models;
using ProyectoGrado.Services;
using ProyectoGrado.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoGrado.Reportings.ViewModels
{
    public class ReportVentViewModel : BindableBase
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IVentasService _ventasService;
        private string _description;
        private List<Venta> _prodcutsSale;
        private int _quantity;
        private int _subTotal;
        private int _idVenta;
        private int _saleUnit;

        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description, value); }
        }

        public DateTime DataTimeNow { get; set; } = DateTime.Now;


        public int SaleUnit
        {
            get { return _saleUnit; }
            set { SetProperty(ref _saleUnit, value); }
        }

        public int IdVenta
        {
            get { return _idVenta; }
            set { SetProperty(ref _idVenta, value); }
        }

        public int Quantity
        {
            get { return _quantity; }
            set { SetProperty(ref _quantity, value); }
        }

        public int SubTotal
        {
            get { return _subTotal; }
            set { SetProperty(ref _subTotal, value); }
        }

        public List<Venta> ProductsSale
        {
            get { return _prodcutsSale; }
            set { SetProperty(ref _prodcutsSale, value); }
        }

        public string NameClient { get; set; } = "";
        public string Cedula { get; set; } = "";
        public string Tel { get; set; }
        public int Total { get; set; }

        public ReportVentViewModel(IVentasService ventasService)
        {
           _ventasService = ventasService;
            ProductsSale = new List<Venta>();
            OnDatum();
        }

        private void OnDatum()
        {
            NameClient = _ventasService.NameClient;
            Tel = _ventasService.Tel;
            Total = _ventasService.Total;
            ProductsSale = new List<Venta>(_ventasService.Ventas);
            IdVenta = _ventasService.IdVenta;
            Cedula = _ventasService.Cedula;

        }
    }
}
