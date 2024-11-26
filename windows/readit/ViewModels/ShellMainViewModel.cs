using Caliburn.Micro;

namespace readit.ViewModels
{
    public class ShellMainViewModel : Conductor<IScreen>.Collection.OneActive
    {
        public ShellMainViewModel()
        {
            ActiveView.ParentMain = this;
            _ = ActiveView.OpenItemMain(new PaginaInicialViewModel());
        }
    }
}