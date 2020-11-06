using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace ProyectoGrado.Utility.Validations
{
    public static class ValidationesInput
    {

        public static bool IsNumber(string number, string mensaje)
        {
            Regex regex = new Regex("[^0-9]+");
            if (regex.IsMatch(number))
            {
                MessageBox.Show(mensaje, "VALIDACIÓN", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        public static bool IsNumber(int number, string mensaje)
        {
            Regex regex = new Regex("[^0-9]+");
            if (regex.IsMatch(number.ToString()))
            {
                MessageBox.Show(mensaje, "VALIDACIÓN", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        public static bool IsString(string name, string mensaje)
        {
            Regex regex = new Regex(@"[^a-zA-Z\s]");
            if (regex.IsMatch(name))
            {
                MessageBox.Show(mensaje, "VALIDACIÓN", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        public static bool IsDateRange(DateTime dateTimeOne, DateTime dateTimeTwo, string mensaje)
        {
                if (dateTimeOne > dateTimeTwo)
                {
                    MessageBox.Show(mensaje, "VALIDACIÓN", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return true;
                }
            return false;
        }
    }
}
