using Prism.Commands;
using Prism.Mvvm;
using ProyectoGrado.Reportings.ViewModels;
using ProyectoGrado.Reportings.Views;
using ProyectoGrado.Services.ReportNeto;
using ProyectoGrado.Utility.Validations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoGrado.ViewModels
{
    public class ReportViewModel : BindableBase
    {
        private bool isVenta = false;
        private bool _isCompra = false;
        private DateTime _dateInitial = DateTime.Now;
        private DateTime _dateEnd = DateTime.Now;
        private readonly IReportNetosService _reportNetosService;

        public DateTime InititalRange { get; set; }
        public DateTime Desde { get; set; } = DateTime.Now;


        public DateTime DateInitial
        {
            get => _dateInitial;
            set
            {
                SetProperty(ref _dateInitial, value);
            }
        }
        public DateTime DateEnd 
        {
            get => _dateEnd;
            set
            {
                SetProperty(ref _dateEnd, value);
            }
        }
        public bool IsVenta
        {
            get => isVenta;
            set
            {
                isVenta = value;
                PrintReportCommand.RaiseCanExecuteChanged();
            }
        }
        public bool IsCompra
        {
            get => _isCompra;
            set
            {
                _isCompra = value;
                PrintReportCommand.RaiseCanExecuteChanged();
            }
        }

        public DelegateCommand PrintReportCommand { get; set; }

        public ReportViewModel(IReportNetosService reportNetosService)
        {
            PrintReportCommand = new DelegateCommand(PrintReport, CanPrintReport);
            _reportNetosService = reportNetosService;
        }

        private bool CanPrintReport()
        {
            if ((IsVenta == true || IsCompra == true))
            {
                return true;
            }

            return false;
        }

        private void PrintReport()
        {
            if (ValidationesInput.IsDateRange(DateInitial, DateEnd, "Fecha inicial debe ser menor a la fecha final"))
            {
                return;
            }

            if (IsVenta)
            {
                PrintVenta();
            }
            else
            {
                PrintCompra();
            }
        }

        private void PrintCompra()
        {
            _reportNetosService.DateInitial = DateInitial.ToString("yyyy-MM-dd");
            _reportNetosService.DateEnd = DateEnd.ToString("yyyy-MM-dd");
            ReportComprasNetoView reportComprasNetoView = new ReportComprasNetoView();
            reportComprasNetoView.DataContext = new ReportComprasNetoViewModel(_reportNetosService);
            reportComprasNetoView.ShowDialog();
        }

        private void PrintVenta()
        {
            ReportVentaTotalesView reportVentaTotalesView = new ReportVentaTotalesView();
            reportVentaTotalesView.DataContext = new ReportVentaTotalesViewModel();
            reportVentaTotalesView.ShowDialog();
        }
    }
}
