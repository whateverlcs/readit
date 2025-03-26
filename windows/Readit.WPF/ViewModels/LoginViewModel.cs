using Caliburn.Micro;
using Readit.Core.Repositories;
using Readit.Core.Services;
using Readit.Infra.Logging;
using Readit.WPF.Infrastructure;
using System.Windows;

namespace Readit.WPF.ViewModels
{
    public class LoginViewModel : Screen
    {
        private string _email;

        public string Email
        {
            get { return _email; }
            set
            {
                _email = value;
                NotifyOfPropertyChange(() => Email);
            }
        }

        private string _senha;

        public string Senha
        {
            get { return _senha; }
            set
            {
                _senha = value;
                NotifyOfPropertyChange(() => Senha);
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

        public bool ManterEmail;

        private readonly IUsuarioService _usuarioService;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ILoggingService _logger;
        private readonly IConfiguracaoService _configuracaoService;

        public LoginViewModel(IUsuarioService usuarioService, IUsuarioRepository usuarioRepository, ILoggingService logger, IConfiguracaoService configuracaoService)
        {
            _usuarioService = usuarioService;
            _usuarioRepository = usuarioRepository;
            _logger = logger;
            _configuracaoService = configuracaoService;
        }

        public void RealizarLogin()
        {
            Task.Run(() => RealizarLoginAsync()).ConfigureAwait(false);
        }

        public async Task RealizarLoginAsync()
        {
            HabilitarCampos = false;
            Loading = true;
            ManterEmail = false;

            try
            {
#if DEBUG
                _usuarioService.UsuarioLogado = (await _usuarioRepository.BuscarUsuarioPorEmailAsync(_configuracaoService.GetLoginAdministrador()).ConfigureAwait(false)).FirstOrDefault();

                await ExibirMensagemFlashAsync("Informação", [$"Bem vindo, {_usuarioService.UsuarioLogado.Nome}!"]);
                await Task.Delay(2000);

                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    var windowManager = DependencyResolver.GetService<IWindowManager>();
                    var shellMainViewModel = DependencyResolver.GetService<ShellMainViewModel>();

                    windowManager.ShowWindowAsync(shellMainViewModel);

                    Application.Current.MainWindow = Application.Current.Windows.OfType<Window>()
                    .FirstOrDefault(w => w.DataContext is ShellViewModel);

                    Application.Current.MainWindow?.Close();
                });
#else
                List<string> erros = _usuarioService.ValidarCampos("", "", Senha, Email);

                if (erros.Count > 0)
                {
                    await ExibirMensagemFlashAsync("Erro", erros);
                    return;
                }

                var usuario = (await _usuarioRepository.BuscarUsuarioPorEmailAsync(Email).ConfigureAwait(false)).FirstOrDefault();

                if (usuario == null)
                {
                    await ExibirMensagemFlashAsync("Erro", ["O e-mail inserido não existe no sistema, favor tentar novamente."]);
                    return;
                }

                if (BC.EnhancedVerify(Senha, usuario.Senha))
                {
                    _usuarioService.UsuarioLogado = usuario;

                    await ExibirMensagemFlashAsync("Informação", [$"Bem vindo, {usuario.Nome}!"]);
                    await Task.Delay(2000);

                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        var windowManager = DependencyResolver.GetService<IWindowManager>();
                        var shellMainViewModel = DependencyResolver.GetService<ShellMainViewModel>();

                        windowManager.ShowWindowAsync(shellMainViewModel);

                        Application.Current.MainWindow = Application.Current.Windows.OfType<Window>()
                        .FirstOrDefault(w => w.DataContext is ShellViewModel);

                        Application.Current.MainWindow?.Close();
                    });
                }
                else
                {
                    await ExibirMensagemFlashAsync("Erro", ["A senha inserida está incorreta, favor tentar novamente."]);
                    ManterEmail = true;
                }
#endif
            }
            catch (Exception e)
            {
                _logger.LogError(e, "RealizarLoginAsync()");
                await ExibirMensagemFlashAsync("Erro", ["Ocorreu um erro ao realizar o login, favor tentar novamente em alguns minutos."]);
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

        public void RealizarCadastro()
        {
            _ = ActiveView.OpenItem<CadastroViewModel>();
        }

        public void LimparDados()
        {
            Email = ManterEmail ? Email : "";
            Senha = "";
            HabilitarCampos = true;
            Loading = false;
        }
    }
}