using Caliburn.Micro;
using Readit.Core.Domain;
using Readit.Core.Enums;
using Readit.Core.Repositories;
using Readit.Core.Services;
using Readit.Infra.Logging;
using System.Globalization;
using System.IO;
using System.Windows;

namespace Readit.WPF.ViewModels
{
    public class CadastroViewModel : Screen
    {
        private string _nomeCompleto;

        public string NomeCompleto
        {
            get { return _nomeCompleto; }
            set
            {
                _nomeCompleto = value;
                NotifyOfPropertyChange(() => NomeCompleto);
            }
        }

        private string _apelido;

        public string Apelido
        {
            get { return _apelido; }
            set
            {
                _apelido = value;
                NotifyOfPropertyChange(() => Apelido);
            }
        }

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

        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ILoggingService _logger;
        private readonly IUsuarioService _usuarioService;
        private readonly IImagemService _imagemService;

        public CadastroViewModel(IUsuarioRepository usuarioRepository, ILoggingService logger, IUsuarioService usuarioService, IImagemService imagemService)
        {
            _usuarioRepository = usuarioRepository;
            _logger = logger;
            _usuarioService = usuarioService;
            _imagemService = imagemService;
        }

        public void CadastrarUsuario()
        {
            HabilitarCampos = false;
            Loading = true;

            Task.Run(() => CadastrarUsuarioAsync()).ConfigureAwait(false);
        }

        public async Task CadastrarUsuarioAsync()
        {
            try
            {
                List<string> erros = _usuarioService.ValidarCampos(NomeCompleto, Apelido, Senha, Email);

                if (erros.Count > 0)
                {
                    await ExibirMensagemFlashAsync("Erro", erros);
                    return;
                }

                if ((await _usuarioRepository.BuscarUsuarioPorEmailAsync(Email).ConfigureAwait(false)).Count != 0)
                {
                    await ExibirMensagemFlashAsync("Erro", ["O e-mail inserido já existe no sistema."]);
                    return;
                }

                bool sucesso = await _usuarioRepository.CadastrarUsuarioAsync(new Usuario
                {
                    Nome = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(NomeCompleto),
                    Apelido = Apelido,
                    Email = Email,
                    Senha = BC.EnhancedHashPassword(Senha, 13),
                    Administrador = false
                },
                new Imagens
                {
                    Imagem = _imagemService.ConvertImageToByteArray(Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, "Resources/Images", "profile-default.jpg")),
                    Formato = Path.GetExtension("../Resources/Images/profile-default.jpg"),
                    Tipo = (byte)EnumObra.TipoImagem.Perfil
                },
                null).ConfigureAwait(false);

                if (sucesso)
                {
                    await ExibirMensagemFlashAsync("Sucesso", ["O cadastro foi realizado com sucesso!"]);
                }
                else
                {
                    await ExibirMensagemFlashAsync("Erro", ["Ocorreu um erro ao realizar o cadastro, favor tentar novamente em alguns minutos."]);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "CadastrarUsuarioThread()");
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

        /// <summary>
        /// Volta para a página de Login
        /// </summary>
        public void VoltarLogin()
        {
            _ = ActiveView.OpenItem<LoginViewModel>();
        }

        public void LimparDados()
        {
            NomeCompleto = "";
            Apelido = "";
            Email = "";
            Senha = "";
            HabilitarCampos = true;
            Loading = false;
        }
    }
}