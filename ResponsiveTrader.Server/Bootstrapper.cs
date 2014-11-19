using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Practices.Prism.UnityExtensions;
using Microsoft.Practices.Unity;
using ResponsiveTrader.Server.Sender;
using ResponsiveTrader.Server.ViewModels;

namespace ResponsiveTrader.Server
{
    public class Bootstrapper : UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return this.Container.Resolve<MainWindow>();
        }

        protected override void InitializeShell()
        {
            base.InitializeShell();
            App.Current.MainWindow = (Window)this.Shell;
            App.Current.MainWindow.Show();
        }


        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();
            Container.RegisterType<IShellViewModel, ShellViewModel>(new TransientLifetimeManager());
            Container.RegisterType<ISender, ResponsiveTrader.Server.Sender.Sender>(new ContainerControlledLifetimeManager());
        }
    }
}
