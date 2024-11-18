using Caliburn.Micro;

namespace readit.ViewModels
{
    public class ShellViewModel : Conductor<IScreen>.Collection.OneActive
    {
        public ShellViewModel()
        {
            ActiveView.Parent = this;
            _ = ActiveView.OpenItem(new LoginViewModel());
        }
    }
}