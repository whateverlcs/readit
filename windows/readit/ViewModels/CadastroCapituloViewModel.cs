using Caliburn.Micro;
using readit.Controls;
using readit.Data;
using readit.Models;
using System.Collections.ObjectModel;
using System.Windows;

namespace readit.ViewModels
{
    public class CadastroCapituloViewModel : Screen
    {
        private bool _exibirMenuAdministrador = Global.UsuarioLogado.Administrador;

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

        private ControlPrincipal cp = new ControlPrincipal();

        private ControlLogs clog = new ControlLogs();

        private DBConnection db = new DBConnection();

        public CadastroCapituloViewModel()
        {
            Thread thread = new(PopularObras) { IsBackground = true };
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        public void PopularObras()
        {
            ListaObras = new(db.BuscarObrasPorId(null).OrderBy(x => x.NomeObra));
        }

        public void CadastrarCapitulo()
        {
            AplicarLoading(true);

            Thread thread = new(CadastrarCapituloThread) { IsBackground = true };
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        public void CadastrarCapituloThread()
        {
            List<string> erros = ValidarCampos();

            if (erros.Count > 0)
            {
                _ = ExibirMensagemFlashAsync("Erro", erros);
                AplicarLoading(false);
                return;
            }

            var capitulosObra = cp.IdentificarArquivosInseridos(ObraSelecionada.Id);

            if (capitulosObra.Count == 0)
            {
                _ = ExibirMensagemFlashAsync("Erro", ["Ocorreu um erro ao identificar os arquivos inseridos, favor tentar novamente em alguns minutos."]);
                AplicarLoading(false);
                return;
            }

            List<string> capitulosExistentes = cp.IdentificarCapitulosExistentesBanco(capitulosObra);

            if (capitulosExistentes.Count > 0)
            {
                _ = ExibirMensagemFlashAsync("Erro", [$"Os seguintes capítulos já existem: {string.Join(", ", capitulosExistentes)}"]);
                AplicarLoading(false);
                return;
            }

            try
            {
                bool sucesso = db.CadastrarCapitulos(capitulosObra);

                if (sucesso)
                {
                    _ = ExibirMensagemFlashAsync("Sucesso", ["Capítulo(s) cadastrado(s) com sucesso!"]);
                }
                else
                {
                    _ = ExibirMensagemFlashAsync("Erro", ["Ocorreu um erro ao realizar o cadastro, favor tentar novamente em alguns minutos."]);
                }
            }
            catch (Exception e)
            {
                clog.RealizarLogExcecao(e.ToString(), "CadastrarCapituloThread()");
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

        public void LimparDados()
        {
            AplicarLoading(false);
            ObraSelecionada = null;
            TxtDropCapitulos = "Nenhum capítulo inserido";
            Global.ListaCapitulosSelecionados.Clear();
        }

        public void AplicarLoading(bool loading)
        {
            Loading = !loading ? false : true;
            HabilitarCampos = !loading ? true : false;
        }

        public List<string> ValidarCampos()
        {
            List<string> erros = [];

            if (Global.ListaCapitulosSelecionados.Count == 0) { erros.Add("Não foi selecionado nenhum capítulo."); }
            if (ObraSelecionada == null) { erros.Add("O Tipo da Obra não foi selecionado."); }

            return erros;
        }

        public void SelecionarCadastro()
        {
            _ = ActiveView.OpenItemMain(new SelecaoCadastroViewModel());
        }

        public void PaginaInicial()
        {
            _ = ActiveView.OpenItemMain(new PaginaInicialViewModel());
        }

        public void EditarPerfil()
        {
            _ = ActiveView.OpenItemMain(new EditarPerfilViewModel());
        }
    }
}