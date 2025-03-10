using Caliburn.Micro;
using Readit.Core.Domain;
using Readit.Core.Repositories;
using Readit.Core.Services;
using Readit.Infra.Helpers;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Readit.WPF.ViewModels
{
    public class DetalhamentoObraViewModel : Screen
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

        public string _nomeObra;

        #region Detalhes da Obra

        private DetalhesObra _dadosDetalhesObra;

        public DetalhesObra DadosDetalhesObra
        {
            get { return _dadosDetalhesObra; }
            set
            {
                _dadosDetalhesObra = value;
                NotifyOfPropertyChange(() => DadosDetalhesObra);
                AtualizarEstrelas();
            }
        }

        public ICommand GenreClickCommand { get; set; }

        public ICommand StarClickedCommand { get; set; }

        public ICommand ChapterClickedCommand { get; set; }

        public ObservableCollection<StarRating> StarRatings { get; set; }

        #endregion Detalhes da Obra

        #region Tooltip Message

        private string _msgInfo;

        public string MsgInfo
        {
            get { return _msgInfo; }
            set
            {
                _msgInfo = value;
                NotifyOfPropertyChange(() => MsgInfo);
            }
        }

        private string _corMsgInfo = "#000000";

        public string CorMsgInfo
        {
            get { return _corMsgInfo; }
            set
            {
                _corMsgInfo = value;
                NotifyOfPropertyChange(() => CorMsgInfo);
            }
        }

        private bool _exibirMensagem;

        public bool ExibirMensagem
        {
            get { return _exibirMensagem; }
            set
            {
                _exibirMensagem = value;
                NotifyOfPropertyChange(() => ExibirMensagem);
            }
        }

        #endregion Tooltip Message

        #region Capitulos Obra

        private string _searchQuery;

        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                _searchQuery = value;
                NotifyOfPropertyChange(() => SearchQuery);
                FiltrarCapitulos();
            }
        }

        private List<ChapterInfo> _filteredCapitulos;

        public List<ChapterInfo> FilteredCapitulos
        {
            get => _filteredCapitulos;
            set
            {
                _filteredCapitulos = value;
                NotifyOfPropertyChange(() => FilteredCapitulos);
            }
        }

        #endregion Capitulos Obra

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

        private readonly IUsuarioService _usuarioService;
        private readonly IAvaliacaoObraRepository _avaliacaoObraRepository;
        private readonly IVisualizacaoObraRepository _visualizacaoObraRepository;
        private readonly IBookmarkRepository _bookmarkRepository;
        private readonly IArquivoService _arquivoService;
        private readonly IObraService _obraService;

        public DetalhamentoObraViewModel(IUsuarioService usuarioService, IAvaliacaoObraRepository avaliacaoObraRepository, IVisualizacaoObraRepository visualizacaoObraRepository, IBookmarkRepository bookmarkRepository, IArquivoService arquivoService, IObraService obraService, string nomeObra)
        {
            _usuarioService = usuarioService;
            _avaliacaoObraRepository = avaliacaoObraRepository;
            _visualizacaoObraRepository = visualizacaoObraRepository;
            _bookmarkRepository = bookmarkRepository;
            _arquivoService = arquivoService;
            _obraService = obraService;
            _exibirMenuAdministrador = _usuarioService.UsuarioLogado.Administrador;
            _nomeObra = nomeObra;
            _texts = _arquivoService.ExtrairDadosFrasesLoading();

            AplicarLoading(true);

            Task.Run(() => CarregarDadosObraAsync()).ConfigureAwait(false);

            GenreClickCommand = new RelayCommandHelper<string>(OnGenreClick);
            StarClickedCommand = new AsyncRelayCommand<object>(StarClicked);
            ChapterClickedCommand = new RelayCommandHelper<ChapterInfo>(OnChapterClick);
        }

        public void AplicarLoading(bool loading)
        {
            if (loading)
            {
                Loading = true;
                ExibirSecoes = Visibility.Hidden;

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
                }
                catch { }

                Loading = false;
                ExibirSecoes = Visibility.Visible;
            }
        }

        public async Task CarregarDadosObraAsync()
        {
            await _visualizacaoObraRepository.AtualizarViewObraAsync(_nomeObra).ConfigureAwait(false);
            DadosDetalhesObra = await _obraService.FormatarDadosDetalhamentoObra(_nomeObra).ConfigureAwait(false);
            FilteredCapitulos = DadosDetalhesObra.ChapterInfos;

            AplicarLoading(false);
        }

        private void FiltrarCapitulos()
        {
            if (string.IsNullOrEmpty(SearchQuery))
            {
                FilteredCapitulos = DadosDetalhesObra.ChapterInfos;
            }
            else
            {
                FilteredCapitulos = DadosDetalhesObra.ChapterInfos
                    .Where(c => c.Chapter.ToLower().Contains(SearchQuery.ToLower()))
                    .ToList();
            }
        }

        public void InicializarEstrelas()
        {
            StarRatings = new ObservableCollection<StarRating>();
            for (int i = 0; i < 5; i++)
            {
                StarRatings.Add(new StarRating { StarIndex = i, IsFilled = false });
            }
        }

        public void AtualizarEstrelas()
        {
            if (DadosDetalhesObra == null) return;

            if (StarRatings == null)
            {
                InicializarEstrelas();
            }

            int fullStars = (int)Math.Round(DadosDetalhesObra.Rating); // Arredonda para preencher corretamente
            for (int i = 0; i < StarRatings.Count; i++)
            {
                StarRatings[i].IsFilled = i < fullStars;
            }
            NotifyOfPropertyChange(() => StarRatings);
        }

        private async Task StarClicked(object param)
        {
            if (param is int index && DadosDetalhesObra != null)
            {
                DadosDetalhesObra.Rating = index + 1; // Atualiza o Rating com base no clique do usuário
                AtualizarEstrelas();
                await _avaliacaoObraRepository.AtualizarRatingAsync(DadosDetalhesObra.ObraId, DadosDetalhesObra.Rating).ConfigureAwait(false);
                NotifyOfPropertyChange(() => DadosDetalhesObra);
            }
        }

        public void StarHovered(int index)
        {
            for (int i = 0; i < StarRatings.Count; i++)
            {
                StarRatings[i].IsHovered = i <= index;
            }
        }

        public void StarHoverExit()
        {
            foreach (var star in StarRatings)
            {
                star.IsHovered = false;
            }
        }

        private void OnGenreClick(string genre)
        {
            _ = ActiveView.OpenItemMain<ListagemObrasViewModel>(genre);
        }

        private void OnChapterClick(ChapterInfo chapter)
        {
            _ = ActiveView.OpenItemMain<LeituraCapituloViewModel>(chapter);
        }

        public void RealizarBookmarkObra()
        {
            Task.Run(() => RealizarBookmarkObraAsync()).ConfigureAwait(false);
        }

        public async Task RealizarBookmarkObraAsync()
        {
            var sucesso = await _bookmarkRepository.CadastrarRemoverBookmarkAsync(new BookmarksUsuario { UsuarioId = _usuarioService.UsuarioLogado.Id, ObraId = DadosDetalhesObra.ObraId }).ConfigureAwait(false);

            if (sucesso.Item1)
            {
                DadosDetalhesObra.Bookmark = sucesso.Item2 == "Adicionado";
                NotifyOfPropertyChange(() => DadosDetalhesObra);

                await ExibirMensagemFlashAsync("Sucesso", [$"{(DadosDetalhesObra.Bookmark ? "Adicionado aos" : "Removido dos")} Bookmarks com sucesso!"]);
            }
            else
            {
                await ExibirMensagemFlashAsync("Erro", ["Ocorreu um erro ao salvar/remover o bookmark."]);
            }
        }

        public async Task ExibirMensagemFlashAsync(string tipoMensagem, List<string> mensagens)
        {
            foreach (var mensagem in mensagens)
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    MsgInfo = mensagem;
                    CorMsgInfo = tipoMensagem switch
                    {
                        "Informação" => "#7c94fa",
                        "Sucesso" => "#70a757",
                        "Erro" => "#d24330",
                        _ => "#000000"
                    };
                    ExibirMensagem = true;
                });

                await Task.Delay(2000);

                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    ExibirMensagem = false;
                });
            }
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