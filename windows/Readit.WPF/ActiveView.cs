using Caliburn.Micro;
using Readit.WPF.Infrastructure;
using Readit.WPF.ViewModels;

namespace Readit.WPF
{
    public static class ActiveView
    {
        public static ShellViewModel Parent;
        public static ShellMainViewModel ParentMain;

        /// <summary>
        /// Realiza a abertura de um viewmodel através do ShellViewModel
        /// </summary>
        public static async Task OpenItem<T>(params object[] args) where T : IScreen
        {
            var viewModel = (T)DependencyResolver.CreateInstance(typeof(T), args);
            await Parent.ActivateItemAsync(viewModel);
        }

        /// <summary>
        /// Realiza a abertura de um viewmodel através do ShellMainViewModel
        /// </summary>
        public static async Task OpenItemMain<T>(params object[] args) where T : IScreen
        {
            var viewModel = (T)DependencyResolver.CreateInstance(typeof(T), args);
            await ParentMain.ActivateItemAsync(viewModel);
        }
    }
}