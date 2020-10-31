using System.Windows;
using System.Windows.Controls;

namespace ProyectoGrado.Reportings.Views
{
    /// <summary>
    /// Interaction logic for ReportVentaTotales.xaml
    /// </summary>
    public partial class ReportVentaTotalesView : Window
    {
        public ReportVentaTotalesView()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.IsEnabled = false;
                PrintDialog printDialog = new PrintDialog();
                if (printDialog.ShowDialog() == true)
                {
                    printDialog.PrintVisual(print, "invoice");
                }
            }
            finally
            {
                IsEnabled = true;

                Close();
            }
        }
    }
}
