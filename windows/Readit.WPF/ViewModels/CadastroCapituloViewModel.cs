using Caliburn.Micro;
using Readit.Core.Domain;
using Readit.Core.Repositories;
using Readit.Core.Services;
using Readit.Infra.Logging;
using Readit.WPF.Infrastructure;
using System.Collections.ObjectModel;
using System.Windows;

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
        }

        public async Task PopularObrasAsync()
        {
            var obras = await _obraRepository.BuscarObrasPorIdAsync(null).ConfigureAwait(false);
            ListaObras = new(obras.OrderBy(x => x.NomeObra));
        }

        public void CadastrarCapitulo()
        {
            AplicarLoading(true);

            Task.Run(() => CadastrarCapituloAsync()).ConfigureAwait(false);
        }

        public async Task CadastrarCapituloAsync()
        {
            List<string> erros = ValidarCampos();

            if (erros.Count > 0)
            {
                await ExibirMensagemFlashAsync("Erro", erros);
                AplicarLoading(false);
                return;
            }

            var capitulosObra = await Task.Run(() => _arquivoService.IdentificarArquivosInseridos(ObraSelecionada.Id))
                                          .ConfigureAwait(false);

            if (capitulosObra.Count == 0)
            {
                await ExibirMensagemFlashAsync("Erro", ["Ocorreu um erro ao identificar os arquivos inseridos, favor tentar novamente em alguns minutos."]);
                AplicarLoading(false);
                return;
            }

            var capitulosExistentes = await Task.Run(() => _capituloService.IdentificarCapitulosExistentesBanco(capitulosObra))
                                                .ConfigureAwait(false);

            if (capitulosExistentes.Count > 0)
            {
                await ExibirMensagemFlashAsync("Erro", [$"Os seguintes capítulos já existem: {string.Join(", ", capitulosExistentes)}"]);
                AplicarLoading(false);
                return;
            }

            try
            {
                var sucesso = await Task.Run(() => _capituloRepository.CadastrarCapitulosAsync(capitulosObra))
                                        .ConfigureAwait(false);

                if (sucesso)
                {
                    await ExibirMensagemFlashAsync("Sucesso", ["Capítulo(s) cadastrado(s) com sucesso!"]);
                }
                else
                {
                    await ExibirMensagemFlashAsync("Erro", ["Ocorreu um erro ao realizar o cadastro, favor tentar novamente em alguns minutos."]);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "CadastrarCapituloThread()");
                await ExibirMensagemFlashAsync("Erro", ["Ocorreu um erro ao realizar o cadastro, favor tentar novamente em alguns minutos."]);
            }
            finally
            {
                await Application.Current.Dispatcher.InvokeAsync(LimparDados);
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

        public void LimparDados()
        {
            AplicarLoading(false);
            ObraSelecionada = null;
            TxtDropCapitulos = "Nenhum capítulo inserido";
            _usuarioService.ListaCapitulosSelecionados.Clear();
        }

        public void AplicarLoading(bool loading)
        {
            Loading = !loading ? false : true;
            HabilitarCampos = !loading ? true : false;
        }

        public List<string> ValidarCampos()
        {
            List<string> erros = [];

            if (_usuarioService.ListaCapitulosSelecionados.Count == 0) { erros.Add("Não foi selecionado nenhum capítulo."); }
            if (ObraSelecionada == null) { erros.Add("O Tipo da Obra não foi selecionado."); }

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