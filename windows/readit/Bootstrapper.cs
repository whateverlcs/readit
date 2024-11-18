using Caliburn.Micro;
using readit.ViewModels;
using System.Windows;

namespace readit
{
    public class Bootstrapper : BootstrapperBase
    {
        public Bootstrapper()
        {
            Initialize();
        }

        protected override async void OnStartup(object sender, StartupEventArgs e)
        {
            await DisplayRootViewForAsync<ShellViewModel>();
        }
    }
}