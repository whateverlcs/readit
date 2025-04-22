using Caliburn.Micro;
using Readit.Core.Desktop.Domain;
using Readit.Core.Desktop.Services;
using Readit.Core.Domain;
using Readit.Core.Enums;
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
    public class ListagemObrasViewModel : Screen
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

        #region Listagem Obras

        /// <summary>
        /// Defina quantos itens serão exibidos por página.
        /// </summary>
        private const int ItemsPerPage = 14;

        private int _currentPage = 1;

        /// <summary>
        /// Comando para a navegação dos detalhes da obra
        /// </summary>
        public ICommand NavigateToDetailsCommand { get; set; }

        public ICommand NextPageCommand { get; set; }

        public ICommand PreviousPageCommand { get; set; }

        public List<PostagensObrasDesktop> ListaObras { get; set; }

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

        public int TotalPages => (int)Math.Ceiling((double)ListaObrasFiltrada.Count / ItemsPerPage);

        #endregion Listagem Obras

        #region Filtro

        private string _searchQuery;

        /// <summary>
        /// Variável que será utilizada para filtrar as obras de acordo com o que o usuário digitou
        /// </summary>
        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                _searchQuery = value;
                NotifyOfPropertyChange(() => SearchQuery);
                FiltrarCapitulos(SearchQuery, GeneroSelecionado, StatusSelecionado, TipoSelecionado, OrdenacaoSelecionada);
            }
        }

        private List<PostagensObrasDesktop> _listaObrasFiltrada;

        public List<PostagensObrasDesktop> ListaObrasFiltrada
        {
            get => _listaObrasFiltrada;
            set
            {
                _listaObrasFiltrada = value;
                NotifyOfPropertyChange(() => ListaObrasFiltrada);
            }
        }

        private ObservableCollection<Generos> _listaGeneros = new ObservableCollection<Generos>();

        public ObservableCollection<Generos> ListaGeneros
        {
            get { return _listaGeneros; }
            set
            {
                _listaGeneros = value;
                NotifyOfPropertyChange(() => ListaGeneros);
            }
        }

        private Generos _generoSelecionado;

        public Generos GeneroSelecionado
        {
            get { return _generoSelecionado; }
            set
            {
                _generoSelecionado = value;
                NotifyOfPropertyChange(() => GeneroSelecionado);
                FiltrarCapitulos(SearchQuery, GeneroSelecionado, StatusSelecionado, TipoSelecionado, OrdenacaoSelecionada);
            }
        }

        private Status _statusSelecionado;

        public Status StatusSelecionado
        {
            get { return _statusSelecionado; }
            set
            {
                _statusSelecionado = value;
                NotifyOfPropertyChange(() => StatusSelecionado);
                FiltrarCapitulos(SearchQuery, GeneroSelecionado, StatusSelecionado, TipoSelecionado, OrdenacaoSelecionada);
            }
        }

        private ObservableCollection<Status> _listaStatus = new ObservableCollection<Status>();

        public ObservableCollection<Status> ListaStatus
        {
            get { return _listaStatus; }
            set
            {
                _listaStatus = value;
                NotifyOfPropertyChange(() => ListaStatus);
            }
        }

        private Tipos _tipoSelecionado;

        public Tipos TipoSelecionado
        {
            get { return _tipoSelecionado; }
            set
            {
                _tipoSelecionado = value;
                NotifyOfPropertyChange(() => TipoSelecionado);
                FiltrarCapitulos(SearchQuery, GeneroSelecionado, StatusSelecionado, TipoSelecionado, OrdenacaoSelecionada);
            }
        }

        private ObservableCollection<Tipos> _listaTipos = new ObservableCollection<Tipos>();

        public ObservableCollection<Tipos> ListaTipos
        {
            get { return _listaTipos; }
            set
            {
                _listaTipos = value;
                NotifyOfPropertyChange(() => ListaTipos);
            }
        }

        private Ordenacao _ordenacaoSelecionada;

        public Ordenacao OrdenacaoSelecionada
        {
            get { return _ordenacaoSelecionada; }
            set
            {
                _ordenacaoSelecionada = value;
                NotifyOfPropertyChange(() => OrdenacaoSelecionada);
                FiltrarCapitulos(SearchQuery, GeneroSelecionado, StatusSelecionado, TipoSelecionado, OrdenacaoSelecionada);
            }
        }

        private ObservableCollection<Ordenacao> _listaOrdenacao = new ObservableCollection<Ordenacao>();

        public ObservableCollection<Ordenacao> ListaOrdenacao
        {
            get { return _listaOrdenacao; }
            set
            {
                _listaOrdenacao = value;
                NotifyOfPropertyChange(() => ListaOrdenacao);
            }
        }

        #endregion Filtro

        private readonly IUsuarioService _usuarioService;
        private readonly IObraRepository _obraRepository;
        private readonly IGeneroRepository _generoRepository;
        private readonly IArquivoService _arquivoService;
        private readonly IObraDesktopService _obraService;
        private readonly string _nomeGenero;

        public ListagemObrasViewModel(IUsuarioService usuarioService, IObraRepository obraRepository, IGeneroRepository generoRepository, IArquivoService arquivoService, IObraDesktopService obraService, string nomeGenero)
        {
            _usuarioService = usuarioService;
            _obraRepository = obraRepository;
            _generoRepository = generoRepository;
            _arquivoService = arquivoService;
            _obraService = obraService;
            _exibirMenuAdministrador = _usuarioService.UsuarioLogado.Administrador;
            _texts = _arquivoService.ExtrairDadosFrasesLoading();
            _nomeGenero = nomeGenero;

            AplicarLoading(true);

            Task.Run(() => CarregarDadosListagemAsync());

            NavigateToDetailsCommand = new RelayCommandHelper<PostagensObrasDesktop>(NavigateToDetails);
            NextPageCommand = new RelayCommandHelper<object>(_ => NextPage());
            PreviousPageCommand = new RelayCommandHelper<object>(_ => PreviousPage());
        }

        public async Task CarregarDadosListagemAsync()
        {
            await PopularListagemObras().ConfigureAwait(false);
            await PopularGeneros().ConfigureAwait(false);

            PopularTipos();
            PopularStatus();
            PopularOrdenacao();

            CanGoToNextPage = CurrentPage < TotalPages;
            CanGoToPreviousPage = CurrentPage > 1;

            UpdatePaginatedList();

            AplicarLoading(false);
        }

        public async Task PopularListagemObras()
        {
            var dadosObras = await _obraRepository.BuscarListagemObrasAsync().ConfigureAwait(false);

            var postagens = _obraService.FormatarDadosListagemObras(dadosObras);

            ListaObras = ListaObrasFiltrada = new List<PostagensObrasDesktop>(postagens);
        }

        private void FiltrarCapitulos(string searchQuery, Generos generoSelecionado, Status statusSelecionado, Tipos tipoSelecionado, Ordenacao ordenacaoSelecionada)
        {
            var obrasFiltradas = ListaObras.AsQueryable();

            if (generoSelecionado != null)
                obrasFiltradas = obrasFiltradas.Where(x => x.Genres.Contains(generoSelecionado.Nome));

            if (statusSelecionado != null)
                obrasFiltradas = obrasFiltradas.Where(x => x.Status.Equals(statusSelecionado.Nome));

            if (tipoSelecionado != null)
                obrasFiltradas = obrasFiltradas.Where(x => x.Tipo.Equals(tipoSelecionado.Nome));

            if (ordenacaoSelecionada != null)
            {
                obrasFiltradas = ordenacaoSelecionada.Id switch
                {
                    var ordenacao when ordenacao == (int)EnumObra.TipoOrdenacao.OrdemAlfabetica => obrasFiltradas.OrderBy(x => x.Title),
                    var ordenacao when ordenacao == (int)EnumObra.TipoOrdenacao.OrdemAlfabeticaReversa => obrasFiltradas.OrderByDescending(x => x.Title),
                    var ordenacao when ordenacao == (int)EnumObra.TipoOrdenacao.OrdemObrasRecemAtualizadas => obrasFiltradas.OrderByDescending(x => x.DataAtualizacao),
                    var ordenacao when ordenacao == (int)EnumObra.TipoOrdenacao.OrdemPublicacao => obrasFiltradas.OrderBy(x => x.DataPublicacao),
                    var ordenacao when ordenacao == (int)EnumObra.TipoOrdenacao.OrdemMaisPopulares => obrasFiltradas.OrderByDescending(x => x.Rating),
                    _ => obrasFiltradas
                };
            }

            ListaObrasFiltrada = string.IsNullOrEmpty(searchQuery)
                ? obrasFiltradas.ToList()
                : obrasFiltradas.Where(c => c.Title.ToLower().Contains(searchQuery.ToLower())).ToList();

            CurrentPage = 1;
            CanGoToNextPage = CurrentPage < TotalPages;
            CanGoToPreviousPage = CurrentPage > 1;

            UpdatePaginatedList();
        }

        private void NavigateToDetails(PostagensObrasDesktop item)
        {
            _ = ActiveView.OpenItemMain<DetalhamentoObraViewModel>(item.TitleOriginal);
        }

        private void UpdatePaginatedList()
        {
            PaginatedList.Clear();
            var itemsToDisplay = ListaObrasFiltrada
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

        public async Task PopularGeneros()
        {
            var generos = (await _generoRepository.BuscarGenerosPorObraAsync(null).ConfigureAwait(false)).OrderBy(x => x.Nome);

            ListaGeneros = new(generos);

            if (!string.IsNullOrEmpty(_nomeGenero))
            {
                GeneroSelecionado = ListaGeneros.Where(x => x.Nome.Equals(_nomeGenero)).FirstOrDefault();
            }
        }

        public void PopularStatus()
        {
            ListaStatus = [
                    new(){ Id = (int)EnumObra.StatusObra.EmAndamento, Nome = "Em Andamento" },
                    new(){ Id = (int)EnumObra.StatusObra.EmHiato, Nome = "Em Hiato" },
                    new(){ Id = (int)EnumObra.StatusObra.Finalizado, Nome = "Finalizado" },
                    new(){ Id = (int)EnumObra.StatusObra.Cancelado, Nome = "Cancelado" },
                    new(){ Id = (int)EnumObra.StatusObra.Dropado, Nome = "Dropado" }
            ];
        }

        public void PopularTipos()
        {
            ListaTipos = [
                new(){ Id = (int)EnumObra.TipoObra.Manhwa, Nome = "Manhwa" },
                new(){ Id = (int)EnumObra.TipoObra.Donghua, Nome = "Donghua" },
                new(){ Id = (int)EnumObra.TipoObra.Manga, Nome = "Manga" }
            ];
        }

        public void PopularOrdenacao()
        {
            ListaOrdenacao = [
                new(){ Id = (int)EnumObra.TipoOrdenacao.OrdemAlfabetica, Nome = "Ordem Alfabética" },
                new(){ Id = (int)EnumObra.TipoOrdenacao.OrdemAlfabeticaReversa, Nome = "Ordem Alfabética Reversa" },
                new(){ Id = (int)EnumObra.TipoOrdenacao.OrdemMaisPopulares, Nome = "Ordem de Popularidade" },
                new(){ Id = (int)EnumObra.TipoOrdenacao.OrdemObrasRecemAtualizadas, Nome = "Ordem de Atualização" },
                new(){ Id = (int)EnumObra.TipoOrdenacao.OrdemPublicacao, Nome = "Ordem de Publicação" }
            ];
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