using ControlzEx.Theming;
using MahApps.Metro.Controls;
using System.Windows;
namespace ProyectoGrado
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var reponder = MessageBox.Show("¿Desea salir de la aplicacion?", "EXIT", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (reponder == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void tema_Click(object sender, RoutedEventArgs e)
        {
            var currentTheme = ThemeManager.Current.DetectTheme();
            var inverseTheme = ThemeManager.Current.GetInverseTheme(currentTheme);
            ThemeManager.Current.ChangeTheme(Application.Current, inverseTheme);
        }

        private void Facebook_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.uniremington.edu.co/rionegro/");
        }

        private void Univeridad_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.uniremington.edu.co/rionegro/");
        }
    }
}
