using Caliburn.Micro;
using Readit.Core.Domain;
using Readit.Core.Repositories;
using Readit.Core.Services;
using Readit.Infra.Helpers;
using Readit.WPF.Infrastructure;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Readit.WPF.ViewModels
{
    public class LeituraCapituloViewModel : Screen
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

        #region Leitura Capitulo

        private string _tituloCapitulo;

        public string TituloCapitulo
        {
            get => _tituloCapitulo;
            set
            {
                _tituloCapitulo = value;
                NotifyOfPropertyChange(() => TituloCapitulo);
            }
        }

        private List<CapitulosObra> _capitulos;

        public List<CapitulosObra> Capitulos
        {
            get => _capitulos;
            set
            {
                _capitulos = value;
                NotifyOfPropertyChange(() => Capitulos);
            }
        }

        private bool isUpdatingCapituloSelecionado = false;

        private CapitulosObra _capituloSelecionado;

        public CapitulosObra CapituloSelecionado
        {
            get => _capituloSelecionado;
            set
            {
                _capituloSelecionado = value;
                NotifyOfPropertyChange(() => CapituloSelecionado);

                if (isUpdatingCapituloSelecionado) return;

                AplicarLoading(true);
                _ = ConfigurarMudancaPaginaThread(null, true);
            }
        }

        private CapitulosObra DadosCapituloAtual;

        private List<PaginasCapitulo> _paginas;

        public List<PaginasCapitulo> Paginas
        {
            get => _paginas;
            set
            {
                _paginas = value;
                NotifyOfPropertyChange(() => Paginas);
            }
        }

        private bool _exibirBotaoVoltarTopo;

        public bool ExibirBotaoVoltarTopo
        {
            get => _exibirBotaoVoltarTopo;
            set
            {
                _exibirBotaoVoltarTopo = value;
                NotifyOfPropertyChange(() => ExibirBotaoVoltarTopo);
            }
        }

        private bool _capituloAnteriorHabilitado;

        public bool CapituloAnteriorHabilitado
        {
            get => _capituloAnteriorHabilitado;
            set
            {
                _capituloAnteriorHabilitado = value;
                NotifyOfPropertyChange(() => CapituloAnteriorHabilitado);
            }
        }

        private bool _proximoCapituloHabilitado;

        public bool ProximoCapituloHabilitado
        {
            get => _proximoCapituloHabilitado;
            set
            {
                _proximoCapituloHabilitado = value;
                NotifyOfPropertyChange(() => ProximoCapituloHabilitado);
            }
        }

        #endregion Leitura Capitulo

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
        public ICommand ResponderCommand { get; set; }
        public ICommand ResponderComentarioCommand { get; set; }
        public ICommand CancelarComentarioCommand { get; set; }

        #endregion Comentários

        private readonly IUsuarioService _usuarioService;
        private readonly ICapituloRepository _capituloRepository;
        private readonly IComentarioRepository _comentarioRepository;
        private readonly ICapituloService _capituloService;
        private readonly IArquivoService _arquivoService;
        private readonly IComentarioService _comentarioService;
        private readonly IImagemService _imagemService;

        public LeituraCapituloViewModel(IUsuarioService usuarioService, ICapituloRepository capituloRepository, IComentarioRepository comentarioRepository, ICapituloService capituloService, IArquivoService arquivoService, IComentarioService comentarioService, IImagemService imagemService, ChapterInfo chapter)
        {
            _usuarioService = usuarioService;
            _capituloRepository = capituloRepository;
            _comentarioRepository = comentarioRepository;
            _capituloService = capituloService;
            _arquivoService = arquivoService;
            _comentarioService = comentarioService;
            _imagemService = imagemService;
            _exibirMenuAdministrador = _usuarioService.UsuarioLogado.Administrador;
            _texts = _arquivoService.ExtrairDadosFrasesLoading();

            AplicarLoading(true);

            Task.Run(() => CarregarDadosCapituloAsync(chapter)).ConfigureAwait(false);

            #region Comentários

            ComentarCommand = new AsyncRelayCommand<object>(Comentar);
            FiltroMelhoresCommand = new RelayCommandHelper<object>(FiltroMelhores);
            FiltroRecentesCommand = new RelayCommandHelper<object>(FiltroRecentes);
            FiltroAntigosCommand = new RelayCommandHelper<object>(FiltroAntigos);
            LikeCommand = new AsyncRelayCommand<Comentarios>(CurtirComentario);
            DislikeCommand = new AsyncRelayCommand<Comentarios>(DislikarComentario);
            ResponderCommand = new RelayCommandHelper<Comentarios>(Responder);
            ResponderComentarioCommand = new AsyncRelayCommand<Comentarios>(ResponderComentario);
            CancelarComentarioCommand = new RelayCommandHelper<Comentarios>(CancelarComentario);

            #endregion Comentários
        }

        #region Leitura Capitulo

        public async Task CarregarDadosCapituloAsync(ChapterInfo chapter)
        {
            (Capitulos, DadosCapituloAtual) = _capituloService.FormatarDadosPaginasCapitulo(await _capituloRepository.BuscarCapituloObrasPorIdAsync(chapter.ObraId, chapter.ChapterId, true, true).ConfigureAwait(false));
            Paginas = DadosCapituloAtual.ListaPaginas;
            CapituloSelecionado = Capitulos.Where(x => x.Id == chapter.ChapterId).FirstOrDefault();
            TituloCapitulo = $"{DadosCapituloAtual.NomeObra} - {DadosCapituloAtual.NumeroCapituloDisplay}";
            CapituloAnteriorHabilitado = Capitulos.Any(x => x.NumeroCapitulo == CapituloSelecionado.NumeroCapitulo - 1);
            ProximoCapituloHabilitado = Capitulos.Any(x => x.NumeroCapitulo == CapituloSelecionado.NumeroCapitulo + 1);
            AplicarLoading(false);
        }

        public void OnScrollChanged(ScrollChangedEventArgs e)
        {
            // Verifica a posição do scroll
            double scrollPosition = e.VerticalOffset;

            // Exibe o botão "Voltar ao Topo" se o usuário rolar para baixo (mais de 100 pixels)
            ExibirBotaoVoltarTopo = scrollPosition > 100;
        }

        public async Task CapituloAnterior()
        {
            AplicarLoading(true);

            await ConfigurarMudancaPaginaThread("Capítulo Anterior", false);
        }

        public async Task ProximoCapitulo()
        {
            AplicarLoading(true);
            await ConfigurarMudancaPaginaThread("Próximo Capítulo", false);
        }

        public async Task ConfigurarMudancaPaginaThread(string tipoBotao, bool alteradoViaCombobox)
        {
            if (Capitulos.Count == 0 || CapituloSelecionado == null) return;

            int index = Capitulos.IndexOf(CapituloSelecionado);
            int novoIndex = (tipoBotao is "Próximo Capítulo") && index < Capitulos.Count - 1 ? index + 1 :
                            (tipoBotao is "Capítulo Anterior") && index > 0 ? index - 1 : index;

            if (novoIndex != index || alteradoViaCombobox)
            {
                // Marca que a atualização está acontecendo internamente, para evitar disparar o setter
                isUpdatingCapituloSelecionado = true;

                // Se alteradoViaCombobox for true, mantém o CapituloSelecionado atual
                var capituloParaAtualizar = alteradoViaCombobox ? CapituloSelecionado : Capitulos[novoIndex];

                // Alterar CapituloSelecionado de maneira assíncrona sem disparar o setter
                CapituloSelecionado = capituloParaAtualizar;

                // Carregar dados do capítulo assíncrono
                var capitulo = await _capituloRepository.BuscarCapituloObrasPorIdAsync(
                    capituloParaAtualizar.ObraId, capituloParaAtualizar.Id, false, true).ConfigureAwait(false);

                DadosCapituloAtual = _capituloService.FormatarDadosPaginasCapitulo(capitulo).Item2;
                Paginas = DadosCapituloAtual.ListaPaginas;
                TituloCapitulo = $"{DadosCapituloAtual.NomeObra} - {DadosCapituloAtual.NumeroCapituloDisplay}";

                ExibirComentarios = false;

                // Marca a operação como concluída
                isUpdatingCapituloSelecionado = false;
            }

            CapituloAnteriorHabilitado = Capitulos.Any(x => x.NumeroCapitulo == CapituloSelecionado.NumeroCapitulo - 1);
            ProximoCapituloHabilitado = Capitulos.Any(x => x.NumeroCapitulo == CapituloSelecionado.NumeroCapitulo + 1);

            AplicarLoading(false);
        }

        #endregion Leitura Capitulo

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
            Comentarios = new ObservableCollection<Comentarios>(await _comentarioService.FormatarDadosComentarios(DadosCapituloAtual.ObraId, DadosCapituloAtual.Id).ConfigureAwait(false));
            NotifyOfPropertyChange(() => Comentarios);

            ComentariosCount = Comentarios.Count + Comentarios.Sum(c => c.Respostas.Count);
            ExibirBotaoComentarios = false;
            ExibirComentarios = true;
            LoadingComentarios = false;
            HabilitarCampos = true;
        }

        public async Task Comentar(object obj)
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
                    IdObra = DadosCapituloAtual.ObraId,
                    IdCapitulo = DadosCapituloAtual.Id,
                    IdUsuario = _usuarioService.UsuarioLogado.Id,
                    ImagemPerfil = _imagemService.ByteArrayToImage(_usuarioService.UsuarioLogado.ImageByte),
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
                    IdObra = DadosCapituloAtual.ObraId,
                    IdCapitulo = DadosCapituloAtual.Id,
                    IdUsuario = _usuarioService.UsuarioLogado.Id,
                    ImagemPerfil = _imagemService.ByteArrayToImage(_usuarioService.UsuarioLogado.ImageByte),
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

        public void CancelarComentario(Comentarios comentario)
        {
            comentario.IsRespostaVisivel = false;
            NovaResposta = string.Empty;
        }

        public async Task CurtirComentario(Comentarios comentario)
        {
            var podeRealizarLike = await _comentarioRepository.ConsultarLikesDeslikesUsuarioAsync(comentario, "Like").ConfigureAwait(false);

            if (podeRealizarLike)
            {
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

            if (podeRealizarDislike)
            {
                await _comentarioRepository.CadastrarRemoverAvaliacaoComentarioAsync(comentario, "Dislike", "Adicionar").ConfigureAwait(false);
                comentario.ContadorDislikes++;
            }
            else
            {
                await _comentarioRepository.CadastrarRemoverAvaliacaoComentarioAsync(comentario, "Dislike", "Remover").ConfigureAwait(false);
                comentario.ContadorDislikes--;
            }
        }

        public void Responder(Comentarios comentario)
        {
            comentario.MostrarResposta();
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

        public async void AplicarLoading(bool loading)
        {
            if (loading)
            {
                Loading = true;
                ExibirSecoes = Visibility.Collapsed;
                ExibirBotaoComentarios = false;

                _index = 0;
                _textIndex = 0;
                _removing = false;

                _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(20) };
                _timer.Tick += (s, e) =>
                {
                    var currentText = _texts[_textIndex];

                    if (_removing)
                    {
                        if (_index > 0)
                            AnimatedText = currentText[..(--_index)];
                        else
                        {
                            _removing = false;
                            _textIndex = (_textIndex + 1) % _texts.Count;
                        }
                    }
                    else
                    {
                        if (_index < currentText.Length)
                            AnimatedText = currentText[..(++_index)];
                        else
                            _removing = true;
                    }
                };
                _timer.Start();
            }
            else
            {
                try
                {
                    await Task.Delay(100);
                    _timer?.Stop();
                    Loading = false;
                    ExibirSecoes = Visibility.Visible;
                    ExibirBotaoComentarios = true;
                }
                catch { }
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