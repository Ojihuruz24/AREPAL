using System.Windows;
namespace ProyectoGrado
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
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
    }
}
