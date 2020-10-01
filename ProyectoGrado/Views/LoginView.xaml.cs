using ProyectoGrado.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace ProyectoGrado.Views
{
    public partial class LoginView : Window
    {
        public LoginView()
        {
            InitializeComponent();
        }

        private void OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            ((LoginViewModel)DataContext).Password = ((PasswordBox)sender).SecurePassword;
        }
    }
}
