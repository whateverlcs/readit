using Caliburn.Micro;
using readit.Controls;
using readit.Data;
using readit.Models;
using System.Globalization;
using System.Windows;

namespace readit.ViewModels
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

        private ControlPrincipal cp = new ControlPrincipal();

        private ControlLogs clog = new ControlLogs();

        private DBConnection db = new DBConnection();

        public void CadastrarUsuario()
        {
            HabilitarCampos = false;
            Loading = true;

            Thread thread = new(CadastrarUsuarioThread) { IsBackground = true };
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        public void CadastrarUsuarioThread()
        {
            try
            {
                List<string> erros = cp.ValidarCampos(NomeCompleto, Apelido, Senha, Email);

                if (erros.Count > 0)
                {
                    _ = ExibirMensagemFlashAsync("Erro", erros);
                    return;
                }

                if (db.BuscarUsuarioPorEmail(Email).Count != 0)
                {
                    _ = ExibirMensagemFlashAsync("Erro", ["O e-mail inserido já existe no sistema."]);
                    return;
                }

                bool sucesso = db.CadastrarUsuario(new Usuario
                {
                    Nome = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(NomeCompleto),
                    Apelido = Apelido,
                    Email = Email,
                    Senha = BC.EnhancedHashPassword(Senha, 13),
                    Administrador = false
                });

                if (sucesso)
                {
                    _ = ExibirMensagemFlashAsync("Sucesso", ["O cadastro foi realizado com sucesso!"]);
                }
                else
                {
                    _ = ExibirMensagemFlashAsync("Erro", ["Ocorreu um erro ao realizar o cadastro, favor tentar novamente em alguns minutos."]);
                }
            }
            catch (Exception e)
            {
                clog.RealizarLogExcecao(e.ToString(), "CadastrarUsuarioThread()");
                _ = ExibirMensagemFlashAsync("Erro", ["Ocorreu um erro ao realizar o cadastro, favor tentar novamente em alguns minutos."]);
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

        public void VoltarLogin()
        {
            _ = ActiveView.OpenItem(new LoginViewModel());
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