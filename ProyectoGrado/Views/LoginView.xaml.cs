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

        private void TextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            int ascci = Convert.ToInt32(Convert.ToChar(e.Text));
            if (ascci >= 48 && ascci <= 57)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }
    }
}
