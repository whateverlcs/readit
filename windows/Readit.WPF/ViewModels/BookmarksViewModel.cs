using Caliburn.Micro;
using Readit.Core.Desktop.Domain;
using Readit.Core.Desktop.Services;
using Readit.Core.Repositories;
using Readit.Core.Services;
using Readit.Infra.Desktop.Helpers;
using Readit.WPF.Infrastructure;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Readit.WPF.ViewModels
{
    public class BookmarksViewModel : Screen
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

        private bool _loading;

        public bool Loading
        {
            get { return _loading; }
            set
            {
                _loading = value;
                NotifyOfPropertyChange(() => Loading);
            }
        }

        #region Loading

        private Visibility _exibirSecoes;

        /// <summary>
        /// Exibe ou deixa invisivel seções do front-end de acordo com a regra de negócio
        /// </summary>
        public Visibility ExibirSecoes
        {
            get { return _exibirSecoes; }
            set
            {
                _exibirSecoes = value;
                NotifyOfPropertyChange(() => ExibirSecoes);
            }
        }

        /// <summary>
        /// Lista de textos que irão aparecer no loading.
        /// </summary>
        private List<string> _texts;

        /// <summary>
        /// Variáveis de apoio na lógica
        /// </summary>
        private int _index, _textIndex;

        /// <summary>
        /// Variável de apoio na lógica
        /// </summary>
        private bool _removing;

        /// <summary>
        /// Timer que irá adicionando ou removendo letra por letra.
        /// </summary>
        private DispatcherTimer _timer;

        private string _animatedText;

        /// <summary>
        /// Texto atual que está sendo exibido.
        /// </summary>
        public string AnimatedText
        {
            get => _animatedText;
            set
            {
                _animatedText = value;
                NotifyOfPropertyChange(() => AnimatedText);
            }
        }

        #endregion Loading

        #region Bookmarks

        /// <summary>
        /// Define quantos itens serão exibidos por página.
        /// </summary>
        private const int ItemsPerPage = 14;

        /// <summary>
        /// Página atual
        /// </summary>
        private int _currentPage = 1;

        /// <summary>
        /// Comando para a navegação dos detalhes da obra
        /// </summary>
        public ICommand NavigateToDetailsCommand { get; set; }

        public ICommand NextPageCommand { get; set; }

        public ICommand PreviousPageCommand { get; set; }

        public BindableCollection<PostagensObrasDesktop> ListaBookmarks { get; set; }

        public BindableCollection<PostagensObrasDesktop> PaginatedList { get; private set; } = new BindableCollection<PostagensObrasDesktop>();

        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                if (_currentPage != value)
                {
                    _currentPage = value;
                    NotifyOfPropertyChange(() => CurrentPage);
                    NotifyOfPropertyChange(() => CanGoToNextPage); // Atualiza o estado do botão Próximo
                    NotifyOfPropertyChange(() => CanGoToPreviousPage); // Atualiza o estado do botão Anterior
                    UpdatePaginatedList();
                }
            }
        }

        private bool _canGoToNextPage;

        public bool CanGoToNextPage
        {
            get => _canGoToNextPage;
            set
            {
                if (_canGoToNextPage != value)
                {
                    _canGoToNextPage = value;
                    NotifyOfPropertyChange(() => CanGoToNextPage);
                }
            }
        }

        private bool _canGoToPreviousPage;

        public bool CanGoToPreviousPage
        {
            get => _canGoToPreviousPage;
            set
            {
                if (_canGoToPreviousPage != value)
                {
                    _canGoToPreviousPage = value;
                    NotifyOfPropertyChange(() => CanGoToPreviousPage);
                }
            }
        }

        public int TotalPages => (int)Math.Ceiling((double)ListaBookmarks.Count / ItemsPerPage);

        #endregion Bookmarks

        private readonly IUsuarioService _usuarioService;
        private readonly IObraRepository _obraRepository;
        private readonly IObraDesktopService _obraService;
        private readonly IArquivoService _arquivoService;

        public BookmarksViewModel(IUsuarioService usuarioService, IObraRepository obraRepository, IObraDesktopService obraService, IArquivoService arquivoService)
        {
            _usuarioService = usuarioService;
            _obraRepository = obraRepository;
            _obraService = obraService;
            _arquivoService = arquivoService;
            _exibirMenuAdministrador = _usuarioService.UsuarioLogado.Administrador;
            _texts = _arquivoService.ExtrairDadosFrasesLoading();

            AplicarLoading(true);

            Task.Run(() => CarregarDadosBookmarksAsync()).ConfigureAwait(false);

            NavigateToDetailsCommand = new RelayCommandHelper<PostagensObrasDesktop>(NavigateToDetails);
            NextPageCommand = new RelayCommandHelper<object>(_ => NextPage());
            PreviousPageCommand = new RelayCommandHelper<object>(_ => PreviousPage());
        }

        public async Task CarregarDadosBookmarksAsync()
        {
            await PopularBookmarksUsuario().ConfigureAwait(false);

            CanGoToNextPage = CurrentPage < TotalPages;
            CanGoToPreviousPage = CurrentPage > 1;

            UpdatePaginatedList();

            AplicarLoading(false);
        }

        public async Task PopularBookmarksUsuario()
        {
            var postagens = _obraService.FormatarDadosBookmarks(await _obraRepository.BuscarObrasBookmarksAsync().ConfigureAwait(false));
            ListaBookmarks = new BindableCollection<PostagensObrasDesktop>(postagens);
        }

        private void NavigateToDetails(PostagensObrasDesktop item)
        {
            _ = ActiveView.OpenItemMain<DetalhamentoObraViewModel>(item.TitleOriginal);
        }

        private void UpdatePaginatedList()
        {
            PaginatedList.Clear();
            var itemsToDisplay = ListaBookmarks
                .Skip((CurrentPage - 1) * ItemsPerPage)
                .Take(ItemsPerPage)
                .ToList();

            PaginatedList.AddRange(itemsToDisplay);
        }

        private void NextPage()
        {
            if (CanGoToNextPage)
            {
                CurrentPage++;
            }
            CanGoToNextPage = CurrentPage < TotalPages;
            CanGoToPreviousPage = CurrentPage > 1;
        }

        private void PreviousPage()
        {
            if (CanGoToPreviousPage)
            {
                CurrentPage--;
            }
            CanGoToNextPage = CurrentPage < TotalPages;
            CanGoToPreviousPage = CurrentPage > 1;
        }

        public void AplicarLoading(bool loading)
        {
            if (loading)
            {
                Loading = true;
                ExibirSecoes = Visibility.Collapsed;

                _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(20) };
                _timer.Tick += (s, e) =>
                {
                    var currentText = _texts[_textIndex];
                    AnimatedText = _removing ? currentText[..(--_index)] : currentText[..(++_index)];

                    if (_index == currentText.Length) _removing = true;
                    if (_index == 0)
                    {
                        _removing = false;
                        _textIndex = (_textIndex + 1) % _texts.Count;
                    }
                };
                _timer.Start();
            }
            else
            {
                try
                {
                    _timer.Stop();
                    Loading = false;
                    ExibirSecoes = Visibility.Visible;
                }
                catch { }
            }
        }

        public void SelecionarCadastro() => _ = ActiveView.OpenItemMain<SelecaoCadastroViewModel>();

        public void PaginaInicial() => _ = ActiveView.OpenItemMain<PaginaInicialViewModel>();

        public void EditarPerfil() => _ = ActiveView.OpenItemMain<EditarPerfilViewModel>();

        public void BookmarksUsuario() => _ = ActiveView.OpenItemMain<BookmarksViewModel>();

        public void ListagemObras() => _ = ActiveView.OpenItemMain<ListagemObrasViewModel>("");

        public async Task RealizarLogoff()
        {
            _usuarioService.CancelarConsultas();

            await Application.Current.Dispatcher.InvokeAsync(async () =>
            {
                _usuarioService.UsuarioLogado = null;
                _usuarioService.ListaCapitulosSelecionados.Clear();

                var windowManager = DependencyResolver.GetService<IWindowManager>();
                var shellViewModel = DependencyResolver.GetService<ShellViewModel>();

                await windowManager.ShowWindowAsync(shellViewModel);

                Application.Current.MainWindow = Application.Current.Windows.OfType<Window>()
                    .FirstOrDefault(w => w.DataContext is ShellMainViewModel);

                Application.Current.MainWindow?.Close();
            });
        }
    }
}