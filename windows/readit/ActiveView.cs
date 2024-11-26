using Caliburn.Micro;
using readit.ViewModels;

namespace readit
{
    public static class ActiveView
    {
        public static ShellViewModel Parent;
        public static ShellMainViewModel ParentMain;

        public static async Task OpenItem(IScreen t)
        {
            await Parent.ActivateItemAsync(t);
        }

        public static async Task OpenItemMain(IScreen t)
        {
            await ParentMain.ActivateItemAsync(t);
        }
    }
}