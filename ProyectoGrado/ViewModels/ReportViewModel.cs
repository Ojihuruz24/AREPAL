﻿using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoGrado.ViewModels
{
    public class ReportViewModel
    {
        private bool isVenta = false;
        private bool _isCompra = false;
        private DateTime _dateInitial = DateTime.Now;
        private DateTime _dateEnd = DateTime.Now;

        public DateTime DateInitial
        {
            get => _dateInitial;
            set
            {
                _dateInitial = value;
            }
        }
        public DateTime DateEnd
        {
            get => _dateEnd;
            set
            {
                _dateEnd = value;
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

        public ReportViewModel()
        {
            PrintReportCommand = new DelegateCommand(PrintReport, CanPrintReport);
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

        }

        private void PrintVenta()
        {

        }
    }
}
