using ProyectoGrado.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

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
