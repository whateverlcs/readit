using Caliburn.Micro;
using Readit.Core.Domain;
using Readit.Core.Repositories;
using Readit.Core.Services;
using Readit.Infra.Helpers;
using Readit.WPF.Infrastructure;
using SharpCompress;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
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
                AtualizarEstrelas(0);
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

        #region Comentários

        private string _novoComentario;

        public string NovoComentario
        {
            get { return _novoComentario; }
            set
            {
                _novoComentario = value;
                NotifyOfPropertyChange(() => NovoComentario);
            }
        }

        private string _novaResposta;

        public string NovaResposta
        {
            get { return _novaResposta; }
            set
            {
                _novaResposta = value;
                NotifyOfPropertyChange(() => NovaResposta);
            }
        }

        private string _novaEdicaoComentario;

        public string NovaEdicaoComentario
        {
            get { return _novaEdicaoComentario; }
            set
            {
                _novaEdicaoComentario = value;
                NotifyOfPropertyChange(() => NovaEdicaoComentario);
            }
        }

        private string _novaEdicaoResposta;

        public string NovaEdicaoResposta
        {
            get { return _novaEdicaoResposta; }
            set
            {
                _novaEdicaoResposta = value;
                NotifyOfPropertyChange(() => NovaEdicaoResposta);
            }
        }

        private ObservableCollection<Comentarios> _comentarios;

        public ObservableCollection<Comentarios> Comentarios
        {
            get { return _comentarios; }
            set
            {
                _comentarios = value;
                NotifyOfPropertyChange(() => Comentarios);
            }
        }

        private bool _exibirComentarios;

        public bool ExibirComentarios
        {
            get { return _exibirComentarios; }
            set
            {
                _exibirComentarios = value;
                NotifyOfPropertyChange(() => ExibirComentarios);
            }
        }

        private bool _exibirBotaoComentarios = true;

        public bool ExibirBotaoComentarios
        {
            get { return _exibirBotaoComentarios; }
            set
            {
                _exibirBotaoComentarios = value;
                NotifyOfPropertyChange(() => ExibirBotaoComentarios);
            }
        }

        private ImageSource _usuarioImagem;

        public ImageSource UsuarioImagem
        {
            get { return _usuarioImagem; }
            set
            {
                _usuarioImagem = value;
                NotifyOfPropertyChange(() => UsuarioImagem);
            }
        }

        private bool _loadingComentarios;

        public bool LoadingComentarios
        {
            get { return _loadingComentarios; }
            set
            {
                _loadingComentarios = value;
                NotifyOfPropertyChange(() => LoadingComentarios);
            }
        }

        private bool _habilitarCampos = true;

        public bool HabilitarCampos
        {
            get { return _habilitarCampos; }
            set
            {
                _habilitarCampos = value;
                NotifyOfPropertyChange(() => HabilitarCampos);
            }
        }

        private int _comentariosCount;

        public int ComentariosCount
        {
            get { return _comentariosCount; }
            set
            {
                _comentariosCount = value;
                NotifyOfPropertyChange(() => ComentariosCount);
            }
        }

        public ICommand ComentarCommand { get; set; }
        public ICommand FiltroMelhoresCommand { get; set; }
        public ICommand FiltroRecentesCommand { get; set; }
        public ICommand FiltroAntigosCommand { get; set; }
        public ICommand LikeCommand { get; set; }
        public ICommand DislikeCommand { get; set; }
        public ICommand ExibirResponderComentarioCommand { get; set; }

        public ICommand ExibirEditarComentarioCommand { get; set; }
        public ICommand ExcluirComentarioCommand { get; set; }
        public ICommand CancelarEdicaoCommand { get; set; }
        public ICommand EditarComentarioCommand { get; set; }

        public ICommand ExibirEditarRespostaCommand { get; set; }
        public ICommand CancelarEdicaoRespostaCommand { get; set; }

        public ICommand ResponderComentarioCommand { get; set; }
        public ICommand CancelarComentarioCommand { get; set; }

        #endregion Comentários

        private readonly IUsuarioService _usuarioService;
        private readonly IAvaliacaoObraRepository _avaliacaoObraRepository;
        private readonly IVisualizacaoObraRepository _visualizacaoObraRepository;
        private readonly IBookmarkRepository _bookmarkRepository;
        private readonly IComentarioRepository _comentarioRepository;
        private readonly IArquivoService _arquivoService;
        private readonly IObraService _obraService;
        private readonly IComentarioService _comentarioService;
        private readonly IImagemService _imagemService;
        private readonly IUtilService _utilService;

        public DetalhamentoObraViewModel(IUsuarioService usuarioService, IAvaliacaoObraRepository avaliacaoObraRepository, IVisualizacaoObraRepository visualizacaoObraRepository, IBookmarkRepository bookmarkRepository, IComentarioRepository comentarioRepository, IArquivoService arquivoService, IObraService obraService, IComentarioService comentarioService, IImagemService imagemService, IUtilService utilService, string nomeObra)
        {
            _usuarioService = usuarioService;
            _avaliacaoObraRepository = avaliacaoObraRepository;
            _visualizacaoObraRepository = visualizacaoObraRepository;
            _bookmarkRepository = bookmarkRepository;
            _comentarioRepository = comentarioRepository;
            _arquivoService = arquivoService;
            _obraService = obraService;
            _comentarioService = comentarioService;
            _imagemService = imagemService;
            _utilService = utilService;
            _exibirMenuAdministrador = _usuarioService.UsuarioLogado.Administrador;
            _nomeObra = nomeObra;
            _texts = _arquivoService.ExtrairDadosFrasesLoading();

            AplicarLoading(true);

            #region Dados Obra

            Task.Run(() => CarregarDadosObraAsync()).ConfigureAwait(false);

            GenreClickCommand = new RelayCommandHelper<string>(OnGenreClick);
            StarClickedCommand = new AsyncRelayCommand<object>(StarClicked);
            ChapterClickedCommand = new RelayCommandHelper<ChapterInfo>(OnChapterClick);

            #endregion Dados Obra

            #region Comentários

            ComentarCommand = new AsyncRelayCommand<object>(RealizarComentario);

            FiltroMelhoresCommand = new RelayCommandHelper<object>(FiltroMelhores);
            FiltroRecentesCommand = new RelayCommandHelper<object>(FiltroRecentes);
            FiltroAntigosCommand = new RelayCommandHelper<object>(FiltroAntigos);

            LikeCommand = new AsyncRelayCommand<Comentarios>(CurtirComentario);
            DislikeCommand = new AsyncRelayCommand<Comentarios>(DislikarComentario);

            ExibirResponderComentarioCommand = new RelayCommandHelper<Comentarios>(ExibirCampoResponder);
            ResponderComentarioCommand = new AsyncRelayCommand<Comentarios>(ResponderComentario);

            CancelarComentarioCommand = new RelayCommandHelper<Comentarios>(CancelarComentario);

            ExibirEditarComentarioCommand = new RelayCommandHelper<Comentarios>(ExibirCampoEditarComentario);
            ExcluirComentarioCommand = new AsyncRelayCommand<Comentarios>(ExcluirComentario);
            CancelarEdicaoCommand = new RelayCommandHelper<Comentarios>(CancelarEdicaoComentarioCommand);
            EditarComentarioCommand = new AsyncRelayCommand<Comentarios>(EditarComentario);

            ExibirEditarRespostaCommand = new RelayCommandHelper<Comentarios>(ExibirCampoEditarResposta);
            CancelarEdicaoRespostaCommand = new RelayCommandHelper<Comentarios>(CancelarEdicaoRespostaComentarioCommand);

            #endregion Comentários
        }

        #region Detalhes Obra

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

        public void AtualizarEstrelas(double rating)
        {
            if (DadosDetalhesObra == null) return;

            if (StarRatings == null)
            {
                InicializarEstrelas();
            }

            int fullStars = (int)Math.Round(rating > 0 ? rating : DadosDetalhesObra.RatingUsuario); // Arredonda para preencher corretamente
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
                var ratingSelecionado = index + 1;

                AtualizarEstrelas(ratingSelecionado);
                await _avaliacaoObraRepository.AtualizarRatingAsync(DadosDetalhesObra.ObraId, ratingSelecionado).ConfigureAwait(false);
                DadosDetalhesObra.Rating = Math.Round(await _avaliacaoObraRepository.BuscarRatingObraAsync(DadosDetalhesObra.ObraId), 1);
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

        #endregion Detalhes Obra

        #region Comentários

        public void CarregarComentarios()
        {
            LoadingComentarios = true;
            HabilitarCampos = false;

            Task.Run(() => CarregarDadosComentariosObra()).ConfigureAwait(false);
        }

        public async Task CarregarDadosComentariosObra()
        {
            UsuarioImagem = _imagemService.ByteArrayToImage(_usuarioService.UsuarioLogado.ImageByte);
            Comentarios = new ObservableCollection<Comentarios>(await _comentarioService.FormatarDadosComentarios(DadosDetalhesObra.ObraId, null).ConfigureAwait(false));
            NotifyOfPropertyChange(() => Comentarios);

            ComentariosCount = Comentarios.Count + Comentarios.Sum(c => c.Respostas.Count);
            ExibirBotaoComentarios = false;
            ExibirComentarios = true;
            LoadingComentarios = false;
            HabilitarCampos = true;
        }

        public async Task RealizarComentario(object obj)
        {
            if (!string.IsNullOrEmpty(NovoComentario))
            {
                var novoComentario = new Comentarios
                {
                    UsuarioApelido = _usuarioService.UsuarioLogado.Apelido,
                    TempoDecorridoFormatado = "1 min(s) atrás",
                    TempoDecorrido = DateTime.Now,
                    ComentarioTexto = NovoComentario,
                    ContadorLikes = 0,
                    ContadorDislikes = 0,
                    IdObra = DadosDetalhesObra.ObraId,
                    IdCapitulo = null,
                    IdUsuario = _usuarioService.UsuarioLogado.Id,
                    ImagemPerfil = _imagemService.ByteArrayToImage(_usuarioService.UsuarioLogado.ImageByte),
                    IsUsuarioOuAdministrador = true
                };

                var sucesso = await _comentarioRepository.CadastrarComentarioAsync(novoComentario).ConfigureAwait(false);

                if (sucesso.Item1)
                {
                    novoComentario.Id = sucesso.Item2;

                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        Comentarios.Add(novoComentario);

                        ComentariosCount = Comentarios.Count + Comentarios.Sum(c => c.Respostas.Count);
                        NovoComentario = string.Empty;
                        NotifyOfPropertyChange(() => Comentarios);
                    });
                }
                else
                {
                    await ExibirMensagemFlashAsync("Erro", ["Ocorreu um erro ao realizar o comentário na obra."]);
                }
            }
        }

        public async Task ResponderComentario(Comentarios comentario)
        {
            if (!string.IsNullOrEmpty(NovaResposta))
            {
                var novaResposta = new Comentarios
                {
                    UsuarioApelido = _usuarioService.UsuarioLogado.Apelido,
                    TempoDecorridoFormatado = "1 min(s) atrás",
                    TempoDecorrido = DateTime.Now,
                    ComentarioTexto = NovaResposta,
                    ContadorLikes = 0,
                    ContadorDislikes = 0,
                    Pai = comentario,
                    IdObra = DadosDetalhesObra.ObraId,
                    IdCapitulo = null,
                    IdUsuario = _usuarioService.UsuarioLogado.Id,
                    ImagemPerfil = _imagemService.ByteArrayToImage(_usuarioService.UsuarioLogado.ImageByte),
                    IsUsuarioOuAdministrador = true
                };

                var sucesso = await _comentarioRepository.CadastrarComentarioAsync(novaResposta).ConfigureAwait(false);

                if (sucesso.Item1)
                {
                    novaResposta.Id = sucesso.Item2;

                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        comentario.AdicionarResposta(novaResposta);

                        ComentariosCount = Comentarios.Count + Comentarios.Sum(c => c.Respostas.Count);
                        NovaResposta = string.Empty;
                        comentario.IsRespostaVisivel = false;
                        NotifyOfPropertyChange(() => Comentarios);
                    });
                }
                else
                {
                    await ExibirMensagemFlashAsync("Erro", ["Ocorreu um erro ao realizar a comentário na obra."]);
                }
            }
        }

        public async Task EditarComentario(Comentarios comentario)
        {
            if (!string.IsNullOrEmpty(NovaEdicaoComentario) || !string.IsNullOrEmpty(NovaEdicaoResposta))
            {
                comentario.ComentarioTexto = string.IsNullOrEmpty(NovaEdicaoComentario) ? NovaEdicaoResposta : NovaEdicaoComentario;
                comentario.TempoUltimaAtualizacaoDecorrido = DateTime.Now;
                comentario.TempoDecorridoFormatado = _utilService.FormatarData(comentario.TempoUltimaAtualizacaoDecorrido);

                var sucesso = await _comentarioRepository.EditarComentarioAsync(comentario).ConfigureAwait(false);

                if (sucesso)
                {
                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        NovaEdicaoComentario = string.Empty;
                        NovaEdicaoResposta = string.Empty;
                        comentario.IsEdicaoComentarioVisivel = false;
                        comentario.IsEdicaoRespostaVisivel = false;
                        NotifyOfPropertyChange(() => Comentarios);
                    });
                }
                else
                {
                    await ExibirMensagemFlashAsync("Erro", ["Ocorreu um erro ao editar o comentário da obra."]);
                }
            }
        }

        public async Task ExcluirComentario(Comentarios comentario)
        {
            var sucesso = await _comentarioRepository.ExcluirComentarioAsync(comentario.Id).ConfigureAwait(false);

            if (sucesso)
            {
                await RealizarExclusaoComentario(comentario);
            }
            else
            {
                await ExibirMensagemFlashAsync("Erro", ["Ocorreu um erro ao realizar ao excluir o comentário."]);
            }
        }

        public async Task RealizarExclusaoComentario(Comentarios comentario)
        {
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                if (comentario.Pai != null)
                {
                    comentario.RemoverResposta(comentario);
                }
                else
                {
                    Comentarios.Remove(comentario);
                }

                ComentariosCount = Comentarios.Count + Comentarios.Sum(c => c.Respostas.Count);
                NotifyOfPropertyChange(() => Comentarios);
            });
        }

        public void CancelarComentario(Comentarios comentario)
        {
            comentario.IsRespostaVisivel = false;
            NovaResposta = string.Empty;
        }

        public void CancelarEdicaoComentarioCommand(Comentarios comentario)
        {
            comentario.IsEdicaoComentarioVisivel = false;
            NovaEdicaoComentario = string.Empty;
        }

        public void CancelarEdicaoRespostaComentarioCommand(Comentarios comentario)
        {
            comentario.IsEdicaoRespostaVisivel = false;
            NovaEdicaoResposta = string.Empty;
        }

        public async Task CurtirComentario(Comentarios comentario)
        {
            var podeRealizarLike = await _comentarioRepository.ConsultarLikesDeslikesUsuarioAsync(comentario, "Like").ConfigureAwait(false);
            var podeRealizarDislike = await _comentarioRepository.ConsultarLikesDeslikesUsuarioAsync(comentario, "Dislike").ConfigureAwait(false);

            if (podeRealizarLike)
            {
                if (!podeRealizarDislike)
                {
                    await _comentarioRepository.CadastrarRemoverAvaliacaoComentarioAsync(comentario, "Dislike", "Remover").ConfigureAwait(false);
                    comentario.ContadorDislikes--;
                }

                await _comentarioRepository.CadastrarRemoverAvaliacaoComentarioAsync(comentario, "Like", "Adicionar").ConfigureAwait(false);
                comentario.ContadorLikes++;
            }
            else
            {
                await _comentarioRepository.CadastrarRemoverAvaliacaoComentarioAsync(comentario, "Like", "Remover").ConfigureAwait(false);
                comentario.ContadorLikes--;
            }
        }

        public async Task DislikarComentario(Comentarios comentario)
        {
            var podeRealizarDislike = await _comentarioRepository.ConsultarLikesDeslikesUsuarioAsync(comentario, "Dislike").ConfigureAwait(false);
            var podeRealizarLike = await _comentarioRepository.ConsultarLikesDeslikesUsuarioAsync(comentario, "Like").ConfigureAwait(false);

            if (podeRealizarDislike)
            {
                if (!podeRealizarLike)
                {
                    await _comentarioRepository.CadastrarRemoverAvaliacaoComentarioAsync(comentario, "Like", "Remover").ConfigureAwait(false);
                    comentario.ContadorLikes--;
                }

                await _comentarioRepository.CadastrarRemoverAvaliacaoComentarioAsync(comentario, "Dislike", "Adicionar").ConfigureAwait(false);
                comentario.ContadorDislikes++;
            }
            else
            {
                await _comentarioRepository.CadastrarRemoverAvaliacaoComentarioAsync(comentario, "Dislike", "Remover").ConfigureAwait(false);
                comentario.ContadorDislikes--;
            }
        }

        public void ExibirCampoResponder(Comentarios comentario)
        {
            comentario.MostrarResposta();
            Comentarios.ForEach(x =>
            {
                if (x.Id != comentario.Id)
                {
                    x.IsRespostaVisivel = false;
                }

                x.IsEdicaoComentarioVisivel = false;
                x.IsEdicaoRespostaVisivel = false;

                x.Respostas.ForEach(y =>
                {
                    if (y.Id != comentario.Id)
                    {
                        y.IsRespostaVisivel = false;
                    }

                    y.IsEdicaoComentarioVisivel = false;
                    y.IsEdicaoRespostaVisivel = false;
                });
            });
        }

        public void ExibirCampoEditarComentario(Comentarios comentario)
        {
            NovaEdicaoComentario = comentario.ComentarioTexto;
            comentario.MostrarEdicao();
            Comentarios.ForEach(x =>
            {
                if (x.Id != comentario.Id)
                {
                    x.IsEdicaoComentarioVisivel = false;
                }

                x.IsRespostaVisivel = false;
                x.IsEdicaoRespostaVisivel = false;

                x.Respostas.ForEach(y =>
                {
                    if (y.Id != comentario.Id)
                    {
                        y.IsEdicaoComentarioVisivel = false;
                    }

                    y.IsRespostaVisivel = false;
                    y.IsEdicaoRespostaVisivel = false;
                });
            });
        }

        public void ExibirCampoEditarResposta(Comentarios comentario)
        {
            NovaEdicaoResposta = comentario.ComentarioTexto;
            comentario.MostrarEdicaoResposta();
            Comentarios.ForEach(x =>
            {
                if (x.Id != comentario.Id)
                {
                    x.IsEdicaoRespostaVisivel = false;
                }

                x.IsEdicaoComentarioVisivel = false;
                x.IsRespostaVisivel = false;

                x.Respostas.ForEach(y =>
                {
                    if (y.Id != comentario.Id)
                    {
                        y.IsEdicaoRespostaVisivel = false;
                    }

                    y.IsEdicaoComentarioVisivel = false;
                    y.IsRespostaVisivel = false;
                });
            });
        }

        public void FiltroMelhores(object obj)
        {
            var melhoresComentarios = Comentarios.OrderByDescending(c => c.ContadorLikes).ToList();
            Comentarios.Clear();
            foreach (var comentario in melhoresComentarios)
            {
                Comentarios.Add(comentario);
            }
        }

        public void FiltroRecentes(object obj)
        {
            var comentariosRecentes = Comentarios.OrderByDescending(c => c.TempoDecorrido).ToList();
            Comentarios.Clear();
            foreach (var comentario in comentariosRecentes)
            {
                Comentarios.Add(comentario);
            }
        }

        public void FiltroAntigos(object obj)
        {
            var comentariosAntigos = Comentarios.OrderBy(c => c.TempoDecorrido).ToList();
            Comentarios.Clear();
            foreach (var comentario in comentariosAntigos)
            {
                Comentarios.Add(comentario);
            }
        }

        #endregion Comentários

        public void AplicarLoading(bool loading)
        {
            if (loading)
            {
                Loading = true;
                ExibirSecoes = Visibility.Hidden;
                ExibirBotaoComentarios = false;

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
                ExibirBotaoComentarios = true;
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