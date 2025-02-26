using Caliburn.Micro;
using readit.Controls;
using readit.Data;
using readit.Enums;
using readit.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace readit.ViewModels
{
    public class ListagemObrasViewModel : Screen
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

        #region Listagem Obras

        private const int ItemsPerPage = 14; // Defina quantos itens serão exibidos por página.

        private int _currentPage = 1;

        /// <summary>
        /// Comando para a navegação dos detalhes da obra
        /// </summary>
        public ICommand NavigateToDetailsCommand { get; set; }

        public ICommand NextPageCommand { get; set; }

        public ICommand PreviousPageCommand { get; set; }

        public List<PostagensObras> ListaObras { get; set; }

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

        public int TotalPages => (int)Math.Ceiling((double)ListaObrasFiltrada.Count / ItemsPerPage);

        #endregion Listagem Obras

        #region Filtro

        private string _searchQuery;

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

        private List<PostagensObras> _listaObrasFiltrada;

        public List<PostagensObras> ListaObrasFiltrada
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

        private ControlPrincipal cp = new ControlPrincipal();

        private ControlLogs clog = new ControlLogs();

        private DBConnection db = new DBConnection();

        public ListagemObrasViewModel()
        {
            AplicarLoading(true);

            Thread thread = new(CarregarDadosListagemThread) { IsBackground = true };
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();

            NavigateToDetailsCommand = new RelayCommand<PostagensObras>(NavigateToDetails);
            NextPageCommand = new RelayCommand<object>(_ => NextPage());
            PreviousPageCommand = new RelayCommand<object>(_ => PreviousPage());
        }

        public void CarregarDadosListagemThread()
        {
            PopularListagemObras();
            PopularGeneros();
            PopularTipos();
            PopularStatus();
            PopularOrdenacao();

            CanGoToNextPage = CurrentPage < TotalPages;
            CanGoToPreviousPage = CurrentPage > 1;

            UpdatePaginatedList();

            AplicarLoading(false);
        }

        public void PopularListagemObras()
        {
            var postagens = cp.FormatarDadosListagemObras(db.BuscarListagemObras());
            ListaObras = ListaObrasFiltrada = new List<PostagensObras>(postagens);
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

        private void NavigateToDetails(PostagensObras item)
        {
            _ = ActiveView.OpenItemMain(new DetalhamentoObraViewModel(item.Title));
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

        public void PopularGeneros()
        {
            var generos = db.BuscarGenerosPorObra(null).OrderBy(x => x.Nome);

            ListaGeneros = new(generos);
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