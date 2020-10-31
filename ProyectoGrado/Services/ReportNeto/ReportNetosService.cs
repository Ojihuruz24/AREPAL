using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoGrado.Services.ReportNeto
{
    public class ReportNetosService : IReportNetosService
    {
        private string _dateInitial = DateTime.Now.ToString();
        private string _dateEnd = DateTime.Now.ToString();

        public string DateInitial
        {
            get => _dateInitial;
            set
            {
                _dateInitial = value;
            }
        }
        public string DateEnd
        {
            get => _dateEnd;
            set
            {
                _dateEnd = value;
            }
        }
    }
}
