using Caliburn.Micro;
using readit.Controls;
using readit.Data;
using System.Windows;

namespace readit.ViewModels
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

        private ControlPrincipal cp = new ControlPrincipal();

        private ControlLogs clog = new ControlLogs();

        private DBConnection db = new DBConnection();

        public void RealizarLogin()
        {
            HabilitarCampos = false;
            Loading = true;
            ManterEmail = false;

            Thread thread = new(RealizarLoginThread) { IsBackground = true };
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        public void RealizarLoginThread()
        {
            try
            {
                List<string> erros = cp.ValidarCampos("", "", Senha, Email);

                if (erros.Count > 0)
                {
                    _ = ExibirMensagemFlashAsync("Erro", erros);
                    return;
                }

                var usuario = db.BuscarUsuarioPorEmail(Email).FirstOrDefault();

                if (usuario == null)
                {
                    _ = ExibirMensagemFlashAsync("Erro", ["O e-mail inserido não existe no sistema, favor tentar novamente."]);
                    return;
                }

                if (BC.EnhancedVerify(Senha, usuario.Senha))
                {
                    Global.UsuarioLogado = usuario;

                    _ = ExibirMensagemFlashAsync("Informação", [$"Bem vindo, {usuario.Nome}!"]);
                    Thread.Sleep(2000);
                    _ = ActiveView.OpenItem(new PaginaInicialViewModel());
                }
                else
                {
                    _ = ExibirMensagemFlashAsync("Erro", ["A senha inserida está incorreta, favor tentar novamente."]);
                    ManterEmail = true;
                }
            }
            catch (Exception e)
            {
                clog.RealizarLogExcecao(e.ToString(), "RealizarLoginThread()");
                _ = ExibirMensagemFlashAsync("Erro", ["Ocorreu um erro ao realizar o login, favor tentar novamente em alguns minutos."]);
            }
            finally
            {
                LimparDados();
            }
        }

        public async Task ExibirMensagemFlashAsync(string tipoMensagem, List<string> mensagens)
        {
            foreach (var mensagem in mensagens)
            {
                Application.Current.Dispatcher.Invoke(() =>
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

                await Task.Delay(4000);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    ExibirMensagem = false;
                });
            }
        }

        public void RealizarCadastro()
        {
            _ = ActiveView.OpenItem(new CadastroViewModel());
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