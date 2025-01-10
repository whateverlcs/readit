using Caliburn.Micro;
using readit.Controls;
using readit.Data;
using readit.Models;
using System.Collections.ObjectModel;
using System.Windows;

namespace readit.ViewModels
{
    public class CadastroGeneroViewModel : Screen
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

        private string _generoDigitado;

        public string GeneroDigitado
        {
            get { return _generoDigitado; }
            set
            {
                _generoDigitado = value;
                NotifyOfPropertyChange(() => GeneroDigitado);
                AtualizarSugestoes();
            }
        }

        private ObservableCollection<Generos> _listaGeneros = new ObservableCollection<Generos>();

        public ObservableCollection<Generos> ListaGeneros
        {
            get { return _listaGeneros; }
            set
            {
                _listaGeneros = value;
                NotifyOfPropertyChange(() => ListaGeneros);
            }
        }

        public ObservableCollection<Generos> Generos;

        private ControlPrincipal cp = new ControlPrincipal();

        private ControlLogs clog = new ControlLogs();

        private DBConnection db = new DBConnection();

        public CadastroGeneroViewModel()
        {
            Thread thread = new(PopularGeneros) { IsBackground = true };
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        public void PopularGeneros()
        {
            var generos = db.BuscarGenerosPorObra(null);

            ListaGeneros = new(generos);
            Generos = new(generos);
        }

        public void AtualizarSugestoes()
        {
            if (string.IsNullOrWhiteSpace(GeneroDigitado))
            {
                ListaGeneros = new ObservableCollection<Generos>(Generos);
            }
            else
            {
                var filtered = Generos
                    .Where(item => item.Nome.StartsWith(GeneroDigitado, System.StringComparison.OrdinalIgnoreCase))
                    .ToList();

                ListaGeneros = new ObservableCollection<Generos>(filtered);
                NotifyOfPropertyChange(() => ListaGeneros);
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

        public void CadastrarGenero()
        {
            AplicarLoading(true);

            Thread thread = new(CadastrarGeneroThread) { IsBackground = true };
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        public void CadastrarGeneroThread()
        {
            List<string> erros = ValidarCampos();

            if (erros.Count > 0)
            {
                _ = ExibirMensagemFlashAsync("Erro", erros);
                AplicarLoading(false);
                return;
            }

            if (db.BuscarGenerosPorNome(GeneroDigitado).Count != 0)
            {
                _ = ExibirMensagemFlashAsync("Erro", ["O Nome do Gênero inserido já existe no sistema."]);
                AplicarLoading(false);
                return;
            }

            try
            {
                bool sucesso = db.CadastrarGenero(new Generos
                {
                    Nome = GeneroDigitado
                });

                if (sucesso)
                {
                    _ = ExibirMensagemFlashAsync("Sucesso", ["Gênero cadastrado com sucesso!"]);
                }
                else
                {
                    _ = ExibirMensagemFlashAsync("Erro", ["Ocorreu um erro ao realizar o cadastro, favor tentar novamente em alguns minutos."]);
                }
            }
            catch (Exception e)
            {
                clog.RealizarLogExcecao(e.ToString(), "CadastrarGeneroThread()");
                _ = ExibirMensagemFlashAsync("Erro", ["Ocorreu um erro ao realizar o cadastro, favor tentar novamente em alguns minutos."]);
            }
            finally
            {
                LimparDados();
            }
        }

        public List<string> ValidarCampos()
        {
            List<string> erros = [];

            if (!string.IsNullOrEmpty(GeneroDigitado) && GeneroDigitado.Length > 255) { erros.Add("O Nome do Gênero não pode ser maior do que 255 caracteres."); }
            if (string.IsNullOrEmpty(GeneroDigitado)) { erros.Add("O Nome do Gênero não foi digitado."); }

            return erros;
        }

        public void LimparDados()
        {
            GeneroDigitado = "";
            AplicarLoading(false);

            Thread thread = new(PopularGeneros) { IsBackground = true };
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        public void AplicarLoading(bool loading)
        {
            Loading = !loading ? false : true;
            HabilitarCampos = !loading ? true : false;
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