using Caliburn.Micro;
using readit.Data;

namespace readit.ViewModels
{
    public class ShellViewModel : Conductor<IScreen>.Collection.OneActive
    {
        private DBConnection db = new DBConnection();

        public ShellViewModel()
        {
            db.RealizarConexaoDB();

            if (!db.TestarConexaoDB())
            {
                _ = ActiveView.OpenItem(new ErroViewModel("../Images/alert.png", "Não foi possível realizar a conexão com o banco de dados."));
                return;
            }

            ActiveView.Parent = this;
            _ = ActiveView.OpenItem(new CadastroViewModel());
        }
    }
}