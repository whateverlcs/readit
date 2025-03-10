using Caliburn.Micro;

namespace Readit.WPF.ViewModels
{
    public class ShellMainViewModel : Conductor<IScreen>.Collection.OneActive
    {
        public ShellMainViewModel()
        {
            ActiveView.ParentMain = this;
        }

        protected override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            await base.OnActivateAsync(cancellationToken);

            await ActiveView.OpenItemMain<PaginaInicialViewModel>();
        }
    }
}