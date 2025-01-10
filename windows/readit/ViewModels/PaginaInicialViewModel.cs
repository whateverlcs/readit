using Caliburn.Micro;

namespace readit.ViewModels
{
    public class PaginaInicialViewModel : Screen
    {
        private bool _exibirMenuAdministrador = Global.UsuarioLogado.Administrador;

        public bool ExibirMenuAdministrador
        {
            get { return _exibirMenuAdministrador; }
            set
            {
                _exibirMenuAdministrador = value;
                NotifyOfPropertyChange(() => ExibirMenuAdministrador);
            }
        }

        public void SelecionarCadastro()
        {
            _ = ActiveView.OpenItemMain(new SelecaoCadastroViewModel());
        }

        public void PaginaInicial()
        {
            _ = ActiveView.OpenItemMain(new PaginaInicialViewModel());
        }

        public void EditarPerfil()
        {
            _ = ActiveView.OpenItemMain(new EditarPerfilViewModel());
        }
    }
}