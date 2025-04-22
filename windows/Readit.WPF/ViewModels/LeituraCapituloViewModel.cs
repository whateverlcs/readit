using Caliburn.Micro;
using Readit.Core.Desktop.Domain;
using Readit.Core.Desktop.Services;
using Readit.Core.Domain;
using Readit.Core.Repositories;
using Readit.Core.Services;
using Readit.Data.Desktop.Mappers;
using Readit.Infra.Desktop.Helpers;
using Readit.WPF.Infrastructure;
using SharpCompress;
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

        private List<CapitulosObraDesktop> _capitulos;

        public List<CapitulosObraDesktop> Capitulos
        {
            get => _capitulos;
            set
            {
                _capitulos = value;
                NotifyOfPropertyChange(() => Capitulos);
            }
        }

        /// <summary>
        /// Variável de controle que verifica se está atualizando o capitulo selecionado ou não.
        /// </summary>
        private bool isUpdatingCapituloSelecionado = false;

        private CapitulosObraDesktop _capituloSelecionado;

        public CapitulosObraDesktop CapituloSelecionado
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

        private CapitulosObraDesktop DadosCapituloAtual;

        private List<PaginasCapituloDesktop> _paginas;

        public List<PaginasCapituloDesktop> Paginas
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

        private ObservableCollection<ComentariosDesktop> _comentarios;

        public ObservableCollection<ComentariosDesktop> Comentarios
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
        private readonly ICapituloRepository _capituloRepository;
        private readonly IComentarioRepository _comentarioRepository;
        private readonly ICapituloDesktopService _capituloService;
        private readonly IArquivoService _arquivoService;
        private readonly IComentarioDesktopService _comentarioService;
        private readonly IImagemService _imagemService;
        private readonly IImagemDesktopService _imagemDesktopService;
        private readonly IUtilService _utilService;

        public LeituraCapituloViewModel(IUsuarioService usuarioService, ICapituloRepository capituloRepository, IComentarioRepository comentarioRepository, ICapituloDesktopService capituloService, IArquivoService arquivoService, IComentarioDesktopService comentarioService, IImagemService imagemService, IImagemDesktopService imagemDesktopService, IUtilService utilService, ChapterInfo chapter)
        {
            _usuarioService = usuarioService;
            _capituloRepository = capituloRepository;
            _comentarioRepository = comentarioRepository;
            _capituloService = capituloService;
            _arquivoService = arquivoService;
            _comentarioService = comentarioService;
            _imagemService = imagemService;
            _imagemDesktopService = imagemDesktopService;
            _utilService = utilService;
            _exibirMenuAdministrador = _usuarioService.UsuarioLogado.Administrador;
            _texts = _arquivoService.ExtrairDadosFrasesLoading();

            AplicarLoading(true);

            Task.Run(() => CarregarDadosCapituloAsync(chapter)).ConfigureAwait(false);

            #region Comentários

            ComentarCommand = new AsyncRelayCommand<object>(RealizarComentario);

            FiltroMelhoresCommand = new RelayCommandHelper<object>(FiltroMelhores);
            FiltroRecentesCommand = new RelayCommandHelper<object>(FiltroRecentes);
            FiltroAntigosCommand = new RelayCommandHelper<object>(FiltroAntigos);

            LikeCommand = new AsyncRelayCommand<ComentariosDesktop>(CurtirComentario);
            DislikeCommand = new AsyncRelayCommand<ComentariosDesktop>(DislikarComentario);

            ExibirResponderComentarioCommand = new RelayCommandHelper<ComentariosDesktop>(ExibirCampoResponder);
            ResponderComentarioCommand = new AsyncRelayCommand<ComentariosDesktop>(ResponderComentario);

            CancelarComentarioCommand = new RelayCommandHelper<ComentariosDesktop>(CancelarComentario);

            ExibirEditarComentarioCommand = new RelayCommandHelper<ComentariosDesktop>(ExibirCampoEditarComentario);
            ExcluirComentarioCommand = new AsyncRelayCommand<ComentariosDesktop>(ExcluirComentario);
            CancelarEdicaoCommand = new RelayCommandHelper<ComentariosDesktop>(CancelarEdicaoComentarioCommand);
            EditarComentarioCommand = new AsyncRelayCommand<ComentariosDesktop>(EditarComentario);

            ExibirEditarRespostaCommand = new RelayCommandHelper<ComentariosDesktop>(ExibirCampoEditarResposta);
            CancelarEdicaoRespostaCommand = new RelayCommandHelper<ComentariosDesktop>(CancelarEdicaoRespostaComentarioCommand);

            #endregion Comentários
        }

        #region Leitura Capitulo

        public async Task CarregarDadosCapituloAsync(ChapterInfo chapter)
        {
            (Capitulos, DadosCapituloAtual) = _capituloService.FormatarDadosPaginasCapitulo(await _capituloRepository.BuscarCapituloObrasPorIdAsync(chapter.ObraId, chapter.ChapterId, true, true).ConfigureAwait(false));
            Paginas = DadosCapituloAtual.ListaPaginasDesktop;
            CapituloSelecionado = Capitulos.Where(x => x.Id == chapter.ChapterId).FirstOrDefault();
            TituloCapitulo = $"{DadosCapituloAtual.NomeObra} - {DadosCapituloAtual.NumeroCapituloDisplay}";
            CapituloAnteriorHabilitado = Capitulos.Any(x => x.NumeroCapitulo < CapituloSelecionado.NumeroCapitulo);
            ProximoCapituloHabilitado = Capitulos.Any(x => x.NumeroCapitulo > CapituloSelecionado.NumeroCapitulo);
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
                Paginas = DadosCapituloAtual.ListaPaginasDesktop;
                TituloCapitulo = $"{DadosCapituloAtual.NomeObra} - {DadosCapituloAtual.NumeroCapituloDisplay}";

                ExibirComentarios = false;

                // Marca a operação como concluída
                isUpdatingCapituloSelecionado = false;
            }

            CapituloAnteriorHabilitado = Capitulos.Any(x => x.NumeroCapitulo < CapituloSelecionado.NumeroCapitulo);
            ProximoCapituloHabilitado = Capitulos.Any(x => x.NumeroCapitulo > CapituloSelecionado.NumeroCapitulo);

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
            UsuarioImagem = _imagemDesktopService.ByteArrayToImage(_usuarioService.UsuarioLogado.ImageByte);
            Comentarios = new ObservableCollection<ComentariosDesktop>(await _comentarioService.FormatarDadosComentarios(DadosCapituloAtual.ObraId, DadosCapituloAtual.Id).ConfigureAwait(false));
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
                var novoComentario = new ComentariosDesktop
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
                    ImagemPerfil = _imagemDesktopService.ByteArrayToImage(_usuarioService.UsuarioLogado.ImageByte),
                    IsUsuarioOuAdministrador = true
                };

                var sucesso = await _comentarioRepository.CadastrarComentarioAsync(novoComentario.DesktopToDomain()).ConfigureAwait(false);

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
                    await ExibirMensagemFlashAsync("Erro", ["Ocorreu um erro ao realizar o comentário no capítulo."]);
                }
            }
        }

        public async Task ResponderComentario(ComentariosDesktop comentario)
        {
            if (!string.IsNullOrEmpty(NovaResposta))
            {
                var novaResposta = new ComentariosDesktop
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
                    ImagemPerfil = _imagemDesktopService.ByteArrayToImage(_usuarioService.UsuarioLogado.ImageByte),
                    IsUsuarioOuAdministrador = true
                };

                var sucesso = await _comentarioRepository.CadastrarComentarioAsync(novaResposta.DesktopToDomain()).ConfigureAwait(false);

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
                    await ExibirMensagemFlashAsync("Erro", ["Ocorreu um erro ao realizar o comentário no capítulo."]);
                }
            }
        }

        public async Task EditarComentario(ComentariosDesktop comentario)
        {
            if (!string.IsNullOrEmpty(NovaEdicaoComentario) || !string.IsNullOrEmpty(NovaEdicaoResposta))
            {
                comentario.ComentarioTexto = string.IsNullOrEmpty(NovaEdicaoComentario) ? NovaEdicaoResposta : NovaEdicaoComentario;
                comentario.TempoUltimaAtualizacaoDecorrido = DateTime.Now;
                comentario.TempoDecorridoFormatado = _utilService.FormatarData(comentario.TempoUltimaAtualizacaoDecorrido);

                var sucesso = await _comentarioRepository.EditarComentarioAsync(comentario.DesktopToDomain()).ConfigureAwait(false);

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
                    await ExibirMensagemFlashAsync("Erro", ["Ocorreu um erro ao editar o comentário do capítulo."]);
                }
            }
        }

        public async Task ExcluirComentario(ComentariosDesktop comentario)
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

        public async Task RealizarExclusaoComentario(ComentariosDesktop comentario)
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

        public void CancelarComentario(ComentariosDesktop comentario)
        {
            comentario.IsRespostaVisivel = false;
            NovaResposta = string.Empty;
        }

        public void CancelarEdicaoComentarioCommand(ComentariosDesktop comentario)
        {
            comentario.IsEdicaoComentarioVisivel = false;
            NovaEdicaoComentario = string.Empty;
        }

        public void CancelarEdicaoRespostaComentarioCommand(ComentariosDesktop comentario)
        {
            comentario.IsEdicaoRespostaVisivel = false;
            NovaEdicaoResposta = string.Empty;
        }

        public async Task CurtirComentario(ComentariosDesktop comentario)
        {
            var podeRealizarLike = await _comentarioRepository.ConsultarLikesDeslikesUsuarioAsync(comentario.DesktopToDomain(), "Like").ConfigureAwait(false);
            var podeRealizarDislike = await _comentarioRepository.ConsultarLikesDeslikesUsuarioAsync(comentario.DesktopToDomain(), "Dislike").ConfigureAwait(false);

            if (podeRealizarLike)
            {
                if (!podeRealizarDislike)
                {
                    await _comentarioRepository.CadastrarRemoverAvaliacaoComentarioAsync(comentario.DesktopToDomain(), "Dislike", "Remover").ConfigureAwait(false);
                    comentario.ContadorDislikes--;
                }

                await _comentarioRepository.CadastrarRemoverAvaliacaoComentarioAsync(comentario.DesktopToDomain(), "Like", "Adicionar").ConfigureAwait(false);
                comentario.ContadorLikes++;
            }
            else
            {
                await _comentarioRepository.CadastrarRemoverAvaliacaoComentarioAsync(comentario.DesktopToDomain(), "Like", "Remover").ConfigureAwait(false);
                comentario.ContadorLikes--;
            }
        }

        public async Task DislikarComentario(ComentariosDesktop comentario)
        {
            var podeRealizarDislike = await _comentarioRepository.ConsultarLikesDeslikesUsuarioAsync(comentario.DesktopToDomain(), "Dislike").ConfigureAwait(false);
            var podeRealizarLike = await _comentarioRepository.ConsultarLikesDeslikesUsuarioAsync(comentario.DesktopToDomain(), "Like").ConfigureAwait(false);

            if (podeRealizarDislike)
            {
                if (!podeRealizarLike)
                {
                    await _comentarioRepository.CadastrarRemoverAvaliacaoComentarioAsync(comentario.DesktopToDomain(), "Like", "Remover").ConfigureAwait(false);
                    comentario.ContadorLikes--;
                }

                await _comentarioRepository.CadastrarRemoverAvaliacaoComentarioAsync(comentario.DesktopToDomain(), "Dislike", "Adicionar").ConfigureAwait(false);
                comentario.ContadorDislikes++;
            }
            else
            {
                await _comentarioRepository.CadastrarRemoverAvaliacaoComentarioAsync(comentario.DesktopToDomain(), "Dislike", "Remover").ConfigureAwait(false);
                comentario.ContadorDislikes--;
            }
        }

        public void ExibirCampoResponder(ComentariosDesktop comentario)
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

        public void ExibirCampoEditarComentario(ComentariosDesktop comentario)
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

        public void ExibirCampoEditarResposta(ComentariosDesktop comentario)
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