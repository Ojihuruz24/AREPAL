using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoGrado.Dialog.ViewModels
{
    public class DialogMahappViewModel 
    {
        // The DialogCoordinator
        private IDialogCoordinator dialogCoordinator;

        public DialogMahappViewModel(IDialogCoordinator instance)
        {
            this.dialogCoordinator = instance;
        }

        public bool QuitConfirmationEnabled { get; internal set; }

        // Simple method which can be used on a Button
        public async void FooMessage()
        {
            await this.dialogCoordinator.ShowMessageAsync(this, "MIRANDO SI ME FUNCIONA", "PRUEBA");
        }

        public async void FooProgress()
        {
            // Show...
            ProgressDialogController controller = await this.dialogCoordinator.ShowProgressAsync(this, "Wait", "Waiting for the Answer to the Ultimate Question of Life, The Universe, and Everything...");

            controller.SetIndeterminate();

            // Do your work... 

            // Close...
            await controller.CloseAsync();
        }


    }
}
