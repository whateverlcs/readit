using Caliburn.Micro;
using Readit.Core.Services;

namespace Readit.WPF.ViewModels
{
    public class SelecaoCadastroViewModel : Screen
    {
        private bool _exibirMenuAdministrador;

        public bool ExibirMenuAdministrador
        {
            get { return _exibirMenuAdministrador; }
            set
            {
                _exibirMenuAdministrador = value;
                NotifyOfPropertyChange(() => ExibirMenuAdministrador);
            }
        }

        private readonly IUsuarioService _usuarioService;

        public SelecaoCadastroViewModel(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
            _exibirMenuAdministrador = _usuarioService.UsuarioLogado.Administrador;
        }

        public void CadastrarObra()
        {
            _ = ActiveView.OpenItemMain<CadastroObraViewModel>();
        }

        public void CadastrarCapitulo()
        {
            _ = ActiveView.OpenItemMain<CadastroCapituloViewModel>();
        }

        public void CadastrarGenero()
        {
            _ = ActiveView.OpenItemMain<CadastroGeneroViewModel>();
        }

        public void SelecionarCadastro()
        {
            _ = ActiveView.OpenItemMain<SelecaoCadastroViewModel>();
        }

        public void PaginaInicial()
        {
            _ = ActiveView.OpenItemMain<PaginaInicialViewModel>();
        }

        public void EditarPerfil()
        {
            _ = ActiveView.OpenItemMain<EditarPerfilViewModel>();
        }

        public void BookmarksUsuario()
        {
            _ = ActiveView.OpenItemMain<BookmarksViewModel>();
        }

        public void ListagemObras()
        {
            _ = ActiveView.OpenItemMain<ListagemObrasViewModel>();
        }
    }
}