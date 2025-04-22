using Caliburn.Micro;
using Readit.Core.Domain;
using Readit.Core.Repositories;
using Readit.Core.Services;
using Readit.Infra.Desktop.Helpers;
using Readit.Infra.Logging;
using Readit.WPF.Infrastructure;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace Readit.WPF.ViewModels
{
    public class CadastroCapituloViewModel : Screen
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

        private ObservableCollection<Obras> _listaObras = new ObservableCollection<Obras>();

        public ObservableCollection<Obras> ListaObras
        {
            get { return _listaObras; }
            set
            {
                _listaObras = value;
                NotifyOfPropertyChange(() => ListaObras);
            }
        }

        private Obras _obraSelecionada;

        public Obras ObraSelecionada
        {
            get { return _obraSelecionada; }
            set
            {
                _obraSelecionada = value;
                NotifyOfPropertyChange(() => ObraSelecionada);
            }
        }

        private string _txtDropCapitulos = "Nenhum capítulo inserido";

        public string TxtDropCapitulos
        {
            get { return _txtDropCapitulos; }
            set
            {
                _txtDropCapitulos = value;
                NotifyOfPropertyChange(() => TxtDropCapitulos);
            }
        }

        private string _tituloPrincipal = "CADASTRAR CAPÍTULOS";

        public string TituloPrincipal
        {
            get { return _tituloPrincipal; }
            set
            {
                _tituloPrincipal = value;
                NotifyOfPropertyChange(() => TituloPrincipal);
            }
        }

        private string _tituloBotao = "Cadastrar Capítulo(s)";

        public string TituloBotao
        {
            get { return _tituloBotao; }
            set
            {
                _tituloBotao = value;
                NotifyOfPropertyChange(() => TituloBotao);
            }
        }

        #region Editar Capítulos

        private string ModoAtual = "Cadastrar";

        private ObservableCollection<CapitulosObra> _capitulosObra = new ObservableCollection<CapitulosObra>();

        public ObservableCollection<CapitulosObra> CapitulosObra
        {
            get { return _capitulosObra; }
            set
            {
                _capitulosObra = value;
                NotifyOfPropertyChange(() => CapitulosObra);
            }
        }

        private Obras _obraEdicaoSelecionada;

        public Obras ObraEdicaoSelecionada
        {
            get { return _obraEdicaoSelecionada; }
            set
            {
                _obraEdicaoSelecionada = value;
                if (_obraEdicaoSelecionada != null)
                {
                    AplicarLoading(true);
                    Task.Run(() => CarregarDadosEdicaoObra(_obraEdicaoSelecionada)).ConfigureAwait(false);
                }
                NotifyOfPropertyChange(() => ObraEdicaoSelecionada);
            }
        }

        private bool _exibirDadosEdicao;

        public bool ExibirDadosEdicao
        {
            get { return _exibirDadosEdicao; }
            set
            {
                _exibirDadosEdicao = value;
                NotifyOfPropertyChange(() => ExibirDadosEdicao);
            }
        }

        private bool _exibirCapitulosEdicao;

        public bool ExibirCapitulosEdicao
        {
            get { return _exibirCapitulosEdicao; }
            set
            {
                _exibirCapitulosEdicao = value;
                NotifyOfPropertyChange(() => ExibirCapitulosEdicao);
            }
        }

        private bool _exibirDadosCadastro = true;

        public bool ExibirDadosCadastro
        {
            get { return _exibirDadosCadastro; }
            set
            {
                _exibirDadosCadastro = value;
                NotifyOfPropertyChange(() => ExibirDadosCadastro);
            }
        }

        private bool _habilitarSelectEdicao;

        public bool HabilitarSelectEdicao
        {
            get { return _habilitarSelectEdicao; }
            set
            {
                _habilitarSelectEdicao = value;
                NotifyOfPropertyChange(() => HabilitarSelectEdicao);
            }
        }

        private string _toggleTitulo = "EDITAR CAPÍTULOS";

        public string ToggleTitulo
        {
            get { return _toggleTitulo; }
            set
            {
                _toggleTitulo = value;
                NotifyOfPropertyChange(() => ToggleTitulo);
            }
        }

        public List<CapitulosObra> ListaRemoverCapitulos = new List<CapitulosObra>();

        public ICommand RemoveChapterCommand { get; set; }

        #endregion Editar Capítulos

        private readonly IUsuarioService _usuarioService;
        private readonly IObraRepository _obraRepository;
        private readonly ICapituloRepository _capituloRepository;
        private readonly ILoggingService _logger;
        private readonly IArquivoService _arquivoService;
        private readonly ICapituloService _capituloService;

        public CadastroCapituloViewModel(IUsuarioService usuarioService, IObraRepository obraRepository, ICapituloRepository capituloRepository, ILoggingService logger, IArquivoService arquivoService, ICapituloService capituloService)
        {
            _usuarioService = usuarioService;
            _obraRepository = obraRepository;
            _capituloRepository = capituloRepository;
            _logger = logger;
            _arquivoService = arquivoService;
            _capituloService = capituloService;
            _exibirMenuAdministrador = _usuarioService.UsuarioLogado.Administrador;

            Task.Run(() => PopularObrasAsync()).ConfigureAwait(false);

            RemoveChapterCommand = new RelayCommandHelper<CapitulosObra>(RemoveChapter);
        }

        public async Task PopularObrasAsync()
        {
            var obras = await _obraRepository.BuscarObrasPorIdAsync(null).ConfigureAwait(false);
            ListaObras = new(obras.OrderBy(x => x.NomeObra));
        }

        public void CadastrarEditarCapitulo()
        {
            AplicarLoading(true);

            Task.Run(() => CadastrarEditarCapituloAsync()).ConfigureAwait(false);
        }

        public async Task CadastrarEditarCapituloAsync()
        {
            List<string> erros = ValidarCampos();

            if (erros.Count > 0)
            {
                await ExibirMensagemFlashAsync("Erro", erros);
                AplicarLoading(false);
                return;
            }

            var capitulosObra = await Task.Run(() => _arquivoService.IdentificarArquivosInseridos(ModoAtual == "Cadastrar" ? ObraSelecionada.Id : ObraEdicaoSelecionada.Id))
                                          .ConfigureAwait(false);

            if (ModoAtual == "Cadastrar" && capitulosObra.Count == 0)
            {
                await ExibirMensagemFlashAsync("Erro", ["Ocorreu um erro ao identificar os arquivos inseridos, favor tentar novamente em alguns minutos."]);
                AplicarLoading(false);
                return;
            }

            if (capitulosObra.Count > 0)
            {
                var capitulosExistentes = await Task.Run(() => _capituloService.IdentificarCapitulosExistentesBanco(capitulosObra))
                                            .ConfigureAwait(false);

                if (capitulosExistentes.Count > 0)
                {
                    await ExibirMensagemFlashAsync("Erro", [$"Os seguintes capítulos já existem: {string.Join(", ", capitulosExistentes)}"]);
                    AplicarLoading(false);
                    return;
                }
            }

            try
            {
                var sucesso = await Task.Run(() => _capituloRepository.CadastrarRemoverCapitulosAsync(capitulosObra, ListaRemoverCapitulos))
                                        .ConfigureAwait(false);

                if (sucesso)
                {
                    await ExibirMensagemFlashAsync("Sucesso", [$"Capítulo(s) {(ModoAtual == "Cadastrar" ? "cadastrado(s)" : "editado(s)")} com sucesso!"]);
                    _logger.LogFilesChapterUploaded(_usuarioService.ListaCapitulosSelecionados);
                }
                else
                {
                    await ExibirMensagemFlashAsync("Erro", [$"Ocorreu um erro ao realizar {(ModoAtual == "Cadastrar" ? "o cadastro" : "a edição")}, favor tentar novamente em alguns minutos."]);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "CadastrarCapituloThread()");
                await ExibirMensagemFlashAsync("Erro", [$"Ocorreu um erro ao realizar {(ModoAtual == "Cadastrar" ? "o cadastro" : "a edição")}, favor tentar novamente em alguns minutos."]);
            }
            finally
            {
                await Application.Current.Dispatcher.InvokeAsync(() => LimparDados(true));
            }
        }

        #region Edição de Capítulos

        public void AlterarModo()
        {
            LimparDados(false);

            AjustarVariaveisModo(ModoAtual);
        }

        public void AjustarVariaveisModo(string modoAtual)
        {
            if (modoAtual == "Cadastrar")
            {
                ModoAtual = "Editar";
                ToggleTitulo = "CADASTRAR CAPÍTULOS";
                ExibirDadosEdicao = true;
                ExibirDadosCadastro = false;
                TituloBotao = "Editar Capítulo(s)";
                TituloPrincipal = "EDITAR CAPÍTULOS";
                HabilitarCampos = false;
                HabilitarSelectEdicao = true;
            }
            else
            {
                ModoAtual = "Cadastrar";
                ToggleTitulo = "EDITAR CAPÍTULOS";
                ExibirDadosEdicao = false;
                ExibirDadosCadastro = true;
                TituloBotao = "Cadastrar Capítulo(s)";
                TituloPrincipal = "CADASTRAR CAPÍTULOS";
                HabilitarCampos = true;
                HabilitarSelectEdicao = false;
            }
        }

        public async Task CarregarDadosEdicaoObra(Obras obra)
        {
            if (obra != null)
            {
                var dados = await _capituloRepository.BuscarCapituloObrasPorIdAsync(obra.Id, 0, true, false).ConfigureAwait(false);

                CapitulosObra = new ObservableCollection<CapitulosObra>(dados.Item1);
            }

            ExibirCapitulosEdicao = true;
            AplicarLoading(false);
        }

        public void AdicionarCapitulo(string caminhoArquivo)
        {
            var cap = Convert.ToInt32(Path.GetFileNameWithoutExtension(caminhoArquivo));

            CapitulosObra.Add(new CapitulosObra
            {
                NumeroCapitulo = cap,
                NumeroCapituloDisplay = $"Capítulo {cap:D2}",
                CaminhoArquivo = caminhoArquivo
            });
        }

        public void RemoveChapter(CapitulosObra capitulo)
        {
            if (capitulo != null && CapitulosObra.Contains(capitulo))
            {
                if (capitulo.Id != 0)
                    ListaRemoverCapitulos.Add(capitulo);

                if (!string.IsNullOrEmpty(capitulo.CaminhoArquivo))
                {
                    _usuarioService.ListaCapitulosSelecionados = _usuarioService.ListaCapitulosSelecionados.Where(x => x != capitulo.CaminhoArquivo).ToList();
                }

                CapitulosObra.Remove(capitulo);
            }
        }

        #endregion Edição de Capítulos

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

        public void LimparDados(bool ajustarVariaveis)
        {
            AplicarLoading(false);
            ObraSelecionada = null;
            ObraEdicaoSelecionada = null;
            ExibirCapitulosEdicao = false;
            TxtDropCapitulos = "Nenhum capítulo inserido";
            _usuarioService.ListaCapitulosSelecionados.Clear();
            ListaRemoverCapitulos.Clear();

            if (ajustarVariaveis)
                AjustarVariaveisModo(ModoAtual == "Editar" ? "Cadastrar" : "Editar");
        }

        public void AplicarLoading(bool loading)
        {
            Loading = !loading ? false : true;
            HabilitarCampos = !loading ? true : false;
            HabilitarSelectEdicao = !loading ? true : false;
        }

        public List<string> ValidarCampos()
        {
            List<string> erros = [];

            if (ModoAtual == "Cadastrar" && _usuarioService.ListaCapitulosSelecionados.Count == 0) { erros.Add("Não foi selecionado nenhum capítulo."); }
            if (ModoAtual == "Cadastrar" && ObraSelecionada == null) { erros.Add("A Obra não foi selecionada."); }
            if (ModoAtual == "Editar" && ObraEdicaoSelecionada == null) { erros.Add("A Obra não foi selecionada."); }

            return erros;
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