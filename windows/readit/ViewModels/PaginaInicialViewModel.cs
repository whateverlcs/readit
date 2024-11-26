using Caliburn.Micro;

namespace readit.ViewModels
{
    public class PaginaInicialViewModel : Screen
    {
        private bool _exibirMenuPaginaObra;

        public bool ExibirMenuPaginaObra
        {
            get { return _exibirMenuPaginaObra; }
            set
            {
                _exibirMenuPaginaObra = value;
                NotifyOfPropertyChange(() => ExibirMenuPaginaObra);
            }
        }
    }
}