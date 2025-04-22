using Caliburn.Micro;
using Readit.Core.Desktop.Domain;
using Readit.Core.Desktop.Services;
using Readit.Core.Domain;
using Readit.Core.Repositories;
using Readit.Core.Services;
using Readit.Infra.Desktop.Helpers;
using Readit.WPF.Infrastructure;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Readit.WPF.ViewModels
{
    public class PaginaInicialViewModel : Screen
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

        #region Slideshow

        private DispatcherTimer _slideshowTimer;

        public BindableCollection<SlideshowItemDesktop> Items { get; set; }

        private SlideshowItemDesktop _currentItem;

        public SlideshowItemDesktop CurrentItem
        {
            get => _currentItem;
            set
            {
                _currentItem = value;
                NotifyOfPropertyChange(() => CurrentItem);
            }
        }

        private int _currentIndex;

        public int CurrentIndex
        {
            get => _currentIndex;
            set
            {
                if (value >= 0 && value < Items.Count)
                {
                    // Atualiza a propriedade IsActive de cada item
                    foreach (var item in Items)
                    {
                        item.IsActive = false;
                    }

                    _currentIndex = value;
                    Items[_currentIndex].IsActive = true;
                    CurrentItem = Items[_currentIndex];
                    NotifyOfPropertyChange(() => CurrentIndex);
                }
            }
        }

        /// <summary>
        /// Comando para navegação do SlideShow
        /// </summary>
        public ICommand NavigateToCommand { get; set; }

        public ICommand GenreClickSlideShowCommand { get; set; }

        #endregion Slideshow

        #region Últimas Atualizações

        /// <summary>
        /// Define quantos itens serão exibidos por página.
        /// </summary>
        private const int ItemsPerPage = 14;

        private int _currentPage = 1;

        /// <summary>
        /// Comando para a navegação dos detalhes da obra
        /// </summary>
        public ICommand NavigateToDetailsCommand { get; set; }

        /// <summary>
        /// Comando para a navegação para o capitulo da obra
        /// </summary>
        public ICommand NavigateToChapterCommand { get; set; }

        public ICommand NextPageCommand { get; set; }

        public ICommand PreviousPageCommand { get; set; }

        public BindableCollection<PostagensObrasDesktop> ListaPostagens { get; set; }

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

        public int TotalPages => (int)Math.Ceiling((double)ListaPostagens.Count / ItemsPerPage);

        #endregion Últimas Atualizações

        #region Destaques

        private string _selectedFilter;

        public ObservableCollection<DestaquesItemDesktop> RankingItems { get; set; }

        public ObservableCollection<DestaquesItemDesktop> RankingItemsFiltered { get; set; } = new ObservableCollection<DestaquesItemDesktop>();

        public string SelectedFilter
        {
            get => _selectedFilter;
            set
            {
                _selectedFilter = value;
                NotifyOfPropertyChange(() => SelectedFilter);
            }
        }

        public ICommand FilterCommand { get; set; }

        public ICommand GenreClickCommand { get; set; }

        /// <summary>
        /// Comando para a navegação dos detalhes da obra através dos destaques
        /// </summary>
        public ICommand NavigateToDetailsHighlightCommand { get; set; }

        #endregion Destaques

        private readonly IUsuarioService _usuarioService;
        private readonly IObraRepository _obraRepository;
        private readonly IArquivoService _arquivoService;
        private readonly IImagemService _imagemService;
        private readonly IObraDesktopService _obraService;

        public PaginaInicialViewModel(IUsuarioService usuarioService, IObraRepository obraRepository, IArquivoService arquivoService, IImagemService imagemService, IObraDesktopService obraService)
        {
            _usuarioService = usuarioService;
            _obraRepository = obraRepository;
            _arquivoService = arquivoService;
            _imagemService = imagemService;
            _obraService = obraService;
            _exibirMenuAdministrador = _usuarioService.UsuarioLogado.Administrador;
            _texts = _arquivoService.ExtrairDadosFrasesLoading();

            AplicarLoading(true);

            Task.Run(() => CarregarDadosPaginaInicialAsync());

            #region Slideshow

            NavigateToCommand = new RelayCommandHelper<SlideshowItemDesktop>(NavigateTo);
            GenreClickSlideShowCommand = new RelayCommandHelper<string>(OnGenreClick);

            #endregion Slideshow

            #region Ultimas Atualizações

            NavigateToDetailsCommand = new RelayCommandHelper<PostagensObrasDesktop>(NavigateToDetails);
            NavigateToChapterCommand = new RelayCommandHelper<ChapterInfo>(NavigateToChapter);
            NextPageCommand = new RelayCommandHelper<object>(_ => NextPage());
            PreviousPageCommand = new RelayCommandHelper<object>(_ => PreviousPage());

            #endregion Ultimas Atualizações

            #region Destaques

            FilterCommand = new AsyncRelayCommand<string>(ApplyFilter);
            GenreClickCommand = new RelayCommandHelper<string>(OnGenreClick);
            NavigateToDetailsHighlightCommand = new RelayCommandHelper<DestaquesItemDesktop>(NavigateToDetailsHighlight);

            #endregion Destaques
        }

        #region Slideshow

        private void OnSlideshowTimerTick(object sender, EventArgs e)
        {
            if (Items.Count > 0)
            {
                // Avança para o próximo item ou retorna ao primeiro
                CurrentIndex = (CurrentIndex + 1) % Items.Count;
            }
        }

        public void NavigateTo(SlideshowItemDesktop item)
        {
            _slideshowTimer.Stop(); // Para o timer temporariamente
            int index = Items.IndexOf(item);
            if (index >= 0)
            {
                CurrentIndex = index;
            }
            _slideshowTimer.Start(); // Reinicia o timer
        }

        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            try { _slideshowTimer.Stop(); } catch { } // Para o timer quando o ViewModel for desativado
            return base.OnDeactivateAsync(close, cancellationToken);
        }

        public void IniciarLeituraObra()
        {
            _ = ActiveView.OpenItemMain<DetalhamentoObraViewModel>(CurrentItem.Title);
        }

        public async Task PopularSlideShow()
        {
            var slideshowItems = _obraService.FormatarDadosSlideshow(await _obraRepository.BuscarObrasSlideShowAsync().ConfigureAwait(false));

            Items = new BindableCollection<SlideshowItemDesktop>(slideshowItems);
            NotifyOfPropertyChange(() => Items);
            NotifyOfPropertyChange(() => CurrentItem);
        }

        #endregion Slideshow

        #region Ultimas Atualizações

        public async Task PopularUltimasAtualizacoes()
        {
            var postagens = _obraService.FormatarDadosUltimasAtualizacoes(await _obraRepository.BuscarObrasUltimasAtualizacoesAsync().ConfigureAwait(false));
            ListaPostagens = new BindableCollection<PostagensObrasDesktop>(postagens);
        }

        private void NavigateToDetails(PostagensObrasDesktop item)
        {
            _ = ActiveView.OpenItemMain<DetalhamentoObraViewModel>(item.Title);
        }

        private void NavigateToChapter(ChapterInfo chapterInfo)
        {
            _ = ActiveView.OpenItemMain<LeituraCapituloViewModel>(chapterInfo);
        }

        private void UpdatePaginatedList()
        {
            PaginatedList.Clear();
            var itemsToDisplay = ListaPostagens
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

        #endregion Ultimas Atualizações

        #region Destaques

        public async Task ApplyFilter(string filter)
        {
            SelectedFilter = filter;

            if (RankingItemsFiltered.Count == 0)
            {
                RankingItemsFiltered = new ObservableCollection<DestaquesItemDesktop>(await _obraService.FormatarDadosObrasEmDestaques().ConfigureAwait(false));
            }

            RankingItems = new ObservableCollection<DestaquesItemDesktop>(RankingItemsFiltered.Where(x => x.Filter == filter).Take(10));
            NotifyOfPropertyChange(() => RankingItems);
        }

        private void OnGenreClick(string genre)
        {
            _ = ActiveView.OpenItemMain<ListagemObrasViewModel>(genre);
        }

        private void NavigateToDetailsHighlight(DestaquesItemDesktop item)
        {
            _ = ActiveView.OpenItemMain<DetalhamentoObraViewModel>(item.Title);
        }

        #endregion Destaques

        public async Task CarregarDadosPaginaInicialAsync()
        {
            #region SlideShow

            await Task.Run(() => PopularSlideShow()).ConfigureAwait(false);  // Chama o método de forma assíncrona

            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                CurrentIndex = 0; // Inicializa no primeiro item

                // Configuração do Timer
                _slideshowTimer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromSeconds(5)
                };
                _slideshowTimer.Tick += OnSlideshowTimerTick;
                _slideshowTimer.Start();
            });

            #endregion SlideShow

            #region Últimas Atualizações

            await Task.Run(() => PopularUltimasAtualizacoes()).ConfigureAwait(false);

            CanGoToNextPage = CurrentPage < TotalPages;
            CanGoToPreviousPage = CurrentPage > 1;

            UpdatePaginatedList();

            #endregion Últimas Atualizações

            #region Destaques

            SelectedFilter = "Semanal";

            await Task.Run(() => ApplyFilter(SelectedFilter)).ConfigureAwait(false);

            #endregion Destaques

            AplicarLoading(false);
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