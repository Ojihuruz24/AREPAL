using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using ProyectoGrado.Dialog.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProyectoGrado.Dialog.Probando
{
    /// <summary>
    /// Interaction logic for DialogMahapp.xaml
    /// </summary>
    public partial class DialogMahappView : MetroWindow
    {

        private readonly DialogMahappViewModel _viewModel;

        public DialogMahappView()
        {
            // Set the DataContext for your View
            _viewModel = new DialogMahappViewModel(DialogCoordinator.Instance);

            DialogParticipation.SetRegister(this, null);
            DataContext = _viewModel;

            this.InitializeComponent();

        }


    }
}
