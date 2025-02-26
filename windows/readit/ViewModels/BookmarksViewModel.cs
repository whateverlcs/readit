using Caliburn.Micro;
using readit.Controls;
using readit.Data;
using readit.Models;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace readit.ViewModels
{
    public class BookmarksViewModel : Screen
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

        #region Bookmarks

        private const int ItemsPerPage = 14; // Defina quantos itens serão exibidos por página.

        private int _currentPage = 1;

        /// <summary>
        /// Comando para a navegação dos detalhes da obra
        /// </summary>
        public ICommand NavigateToDetailsCommand { get; set; }

        public ICommand NextPageCommand { get; set; }

        public ICommand PreviousPageCommand { get; set; }

        public BindableCollection<PostagensObras> ListaBookmarks { get; set; }

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

        public int TotalPages => (int)Math.Ceiling((double)ListaBookmarks.Count / ItemsPerPage);

        #endregion Bookmarks

        private ControlPrincipal cp = new ControlPrincipal();

        private ControlLogs clog = new ControlLogs();

        private DBConnection db = new DBConnection();

        public BookmarksViewModel()
        {
            AplicarLoading(true);

            Thread thread = new(CarregarDadosBookmarksThread) { IsBackground = true };
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();

            NavigateToDetailsCommand = new RelayCommand<PostagensObras>(NavigateToDetails);
            NextPageCommand = new RelayCommand<object>(_ => NextPage());
            PreviousPageCommand = new RelayCommand<object>(_ => PreviousPage());
        }

        public void CarregarDadosBookmarksThread()
        {
            PopularBookmarksUsuario();

            CanGoToNextPage = CurrentPage < TotalPages;
            CanGoToPreviousPage = CurrentPage > 1;

            UpdatePaginatedList();

            AplicarLoading(false);
        }

        public void PopularBookmarksUsuario()
        {
            var postagens = cp.FormatarDadosBookmarks(db.BuscarObrasBookmarks());
            ListaBookmarks = new BindableCollection<PostagensObras>(postagens);
        }

        private void NavigateToDetails(PostagensObras item)
        {
            _ = ActiveView.OpenItemMain(new DetalhamentoObraViewModel(item.Title));
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