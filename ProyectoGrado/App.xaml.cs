using Prism.Ioc;
using Prism.Unity;
using ProyectoGrado.Services;
using ProyectoGrado.Services.ReportNeto;
using ProyectoGrado.ViewModels;
using ProyectoGrado.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ProyectoGrado
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IVentasService, VentasService>();
            containerRegistry.RegisterSingleton<IReportNetosService, ReportNetosService>();
        }

        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void InitializeShell(Window shell)
        {
            var result = false;
            var login = Container.Resolve<LoginView>();
            Action<bool> onCompleted = b =>
            {
                result = b;
                if (result)
                {
                    login.Close();
                }
            };
            var loginViewModel = new LoginViewModel(onCompleted);
            login.DataContext = loginViewModel;
            login.ShowDialog();
            if (!result)
            {
                Current.Shutdown();
                return;
            }
            base.InitializeShell(shell);
        }
    }
}
