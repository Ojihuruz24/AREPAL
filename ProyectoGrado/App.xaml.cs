using MahApps.Metro.Controls.Dialogs;
using Prism.Events;
using Prism.Ioc;
using Prism.Unity;
using ProyectoGrado.Services;
using ProyectoGrado.Services.DataBase;
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
            containerRegistry.Register<BackupService>();
            //containerRegistry.RegisterInstance(DialogCoordinator.Instance);
        }

        protected override Window CreateShell()
        {
            return Container.Resolve<LoginView>();
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
            var loginViewModel = new LoginViewModel(onCompleted, Container.Resolve<IEventAggregator>());
            login.DataContext = loginViewModel;
            login.ShowDialog();
            if (!result)
            {
                Current.Shutdown();
                return;
            }

            var win = new HomeView();
            var homeViewModel = new HomeViewModel(Container.Resolve<BackupService>());
            win.DataContext = homeViewModel;
            win.ShowDialog();

            //base.InitializeModules();
        }
    }
}
