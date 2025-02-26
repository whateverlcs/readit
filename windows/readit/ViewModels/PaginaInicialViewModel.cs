using Caliburn.Micro;
using readit.Controls;
using readit.Data;
using readit.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

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

        public Visibility ExibirSecoes
        {
            get { return _exibirSecoes; }
            set
            {
                _exibirSecoes = value;
                NotifyOfPropertyChange(() => ExibirSecoes);
            }
        }

        private List<string> _texts;
        private int _index, _textIndex;
        private bool _removing;
        private DispatcherTimer _timer;

        private string _animatedText;

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

        public BindableCollection<SlideshowItem> Items { get; set; }

        private SlideshowItem _currentItem;

        public SlideshowItem CurrentItem
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

        private const int ItemsPerPage = 14; // Defina quantos itens serão exibidos por página.

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

        public BindableCollection<PostagensObras> ListaPostagens { get; set; }

        public BindableCollection<PostagensObras> PaginatedList { get; private set; } = new BindableCollection<PostagensObras>();

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

        public ObservableCollection<DestaquesItem> RankingItems { get; set; }

        public ObservableCollection<DestaquesItem> RankingItemsFiltered { get; set; } = new ObservableCollection<DestaquesItem>();

        public string SelectedFilter
        {
            get => _selectedFilter;
            set
            {
                _selectedFilter = value;
                NotifyOfPropertyChange(() => SelectedFilter);
            }
        }

        public RelayCommand<string> FilterCommand { get; set; }

        public ICommand GenreClickCommand { get; set; }

        /// <summary>
        /// Comando para a navegação dos detalhes da obra através dos destaques
        /// </summary>
        public ICommand NavigateToDetailsHighlightCommand { get; set; }

        #endregion Destaques

        private ControlPrincipal cp = new ControlPrincipal();

        private ControlLogs clog = new ControlLogs();

        private DBConnection db = new DBConnection();

        public PaginaInicialViewModel()
        {
            AplicarLoading(true);

            Thread thread = new(CarregarDadosPaginaInicialThread) { IsBackground = true };
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();

            #region Slideshow

            NavigateToCommand = new RelayCommand<SlideshowItem>(NavigateTo);
            GenreClickSlideShowCommand = new RelayCommand<string>(OnGenreClick);

            #endregion Slideshow

            #region Ultimas Atualizações

            NavigateToDetailsCommand = new RelayCommand<PostagensObras>(NavigateToDetails);
            NavigateToChapterCommand = new RelayCommand<ChapterInfo>(NavigateToChapter);
            NextPageCommand = new RelayCommand<object>(_ => NextPage());
            PreviousPageCommand = new RelayCommand<object>(_ => PreviousPage());

            #endregion Ultimas Atualizações

            #region Destaques

            FilterCommand = new RelayCommand<string>(ApplyFilter);
            GenreClickCommand = new RelayCommand<string>(OnGenreClick);
            NavigateToDetailsHighlightCommand = new RelayCommand<DestaquesItem>(NavigateToDetailsHighlight);

            #endregion Destaques
        }

        #region Slideshow

        private void OnSlideshowTimerTick(object sender, EventArgs e)
        {
            // Avança para o próximo item ou retorna ao primeiro
            CurrentIndex = (CurrentIndex + 1) % Items.Count;
        }

        public void NavigateTo(SlideshowItem item)
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
            _ = ActiveView.OpenItemMain(new DetalhamentoObraViewModel(CurrentItem.Title));
        }

        public void PopularSlideShow()
        {
            var slideshowItems = db.BuscarObrasSlideShow();

            foreach (var item in slideshowItems)
            {
                item.BackgroundImage = cp.ByteArrayToImage(item.BackgroundImageByte);
                item.FocusedImage = cp.ByteArrayToImage(item.FocusedImageByte);
                item.Description = item.Description.Length > 211 ? item.Description.Substring(0, 211).Trim() + "..." : item.Description.Trim();
            }

            Items = new BindableCollection<SlideshowItem>(slideshowItems);
            NotifyOfPropertyChange(() => Items);
            NotifyOfPropertyChange(() => CurrentItem);
        }

        #endregion Slideshow

        #region Ultimas Atualizações

        public void PopularUltimasAtualizacoes()
        {
            var postagens = cp.FormatarDadosUltimasAtualizacoes(db.BuscarObrasUltimasAtualizacoes());
            ListaPostagens = new BindableCollection<PostagensObras>(postagens);
        }

        private void NavigateToDetails(PostagensObras item)
        {
            _ = ActiveView.OpenItemMain(new DetalhamentoObraViewModel(item.Title));
        }

        private void NavigateToChapter(ChapterInfo chapterInfo)
        {
            // Lógica para navegação baseada no capítulo
            System.Diagnostics.Debug.WriteLine($"Navegando para o {chapterInfo.Chapter}");
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

        private void ApplyFilter(string filter)
        {
            SelectedFilter = filter;

            if (RankingItemsFiltered.Count == 0)
            {
                RankingItemsFiltered = new ObservableCollection<DestaquesItem>(cp.FormatarDadosObrasEmDestaques());
            }

            RankingItems = new ObservableCollection<DestaquesItem>(RankingItemsFiltered.Where(x => x.Filter == filter));
            NotifyOfPropertyChange(() => RankingItems);
        }

        private void OnGenreClick(string genre)
        {
            // Lógica ao clicar em um gênero
            string err = "";
        }

        private void NavigateToDetailsHighlight(DestaquesItem item)
        {
            _ = ActiveView.OpenItemMain(new DetalhamentoObraViewModel(item.Title));
        }

        #endregion Destaques

        public void CarregarDadosPaginaInicialThread()
        {
            #region SlideShow

            PopularSlideShow();

            Application.Current.Dispatcher.BeginInvoke(() =>
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

            PopularUltimasAtualizacoes();

            CanGoToNextPage = CurrentPage < TotalPages;
            CanGoToPreviousPage = CurrentPage > 1;

            UpdatePaginatedList();

            #endregion Últimas Atualizações

            #region Destaques

            SelectedFilter = "Semanal";

            ApplyFilter(SelectedFilter);

            #endregion Destaques

            AplicarLoading(false);
        }

        public void AplicarLoading(bool loading)
        {
            if (loading)
            {
                Loading = true;
                ExibirSecoes = Visibility.Collapsed;

                _texts = cp.ExtrairDadosFrasesLoading();
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

        public void BookmarksUsuario()
        {
            _ = ActiveView.OpenItemMain(new BookmarksViewModel());
        }

        public void ListagemObras()
        {
            _ = ActiveView.OpenItemMain(new ListagemObrasViewModel());
        }
    }
}