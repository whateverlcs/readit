using Caliburn.Micro;

namespace readit.ViewModels
{
    public class SelecaoCadastroViewModel : Screen
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

        public void CadastrarObra()
        {
            _ = ActiveView.OpenItemMain(new CadastroObraViewModel());
        }

        public void CadastrarCapitulo()
        {
            _ = ActiveView.OpenItemMain(new CadastroCapituloViewModel());
        }

        public void CadastrarGenero()
        {
            _ = ActiveView.OpenItemMain(new CadastroGeneroViewModel());
        }

        public void SelecionarCadastro()
        {
            _ = ActiveView.OpenItemMain(new SelecaoCadastroViewModel());
        }
    }
}