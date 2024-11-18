using Caliburn.Micro;
using readit.ViewModels;

namespace readit
{
    public static class ActiveView
    {
        public static ShellViewModel Parent;

        public static async Task OpenItem(IScreen t)
        {
            await Parent.ActivateItemAsync(t);
        }
    }
}