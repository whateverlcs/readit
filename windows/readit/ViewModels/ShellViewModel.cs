using Caliburn.Micro;
using readit.Controls;
using readit.Data;

namespace readit.ViewModels
{
    public class ShellViewModel : Conductor<IScreen>.Collection.OneActive
    {
        private ControlPrincipal cp = new ControlPrincipal();
        private DBConnection db = new DBConnection();

        public ShellViewModel()
        {
            ActiveView.Parent = this;

            cp.CriarPastaControle();

            db.RealizarConexaoDB();

            if (!db.TestarConexaoDB())
            {
                _ = ActiveView.OpenItem(new ErroViewModel("../Images/alert.png", "Não foi possível realizar a conexão com o banco de dados."));
                return;
            }

            _ = ActiveView.OpenItem(new LoginViewModel());
        }
    }
}