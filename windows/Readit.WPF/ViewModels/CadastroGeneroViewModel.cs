using Caliburn.Micro;
using Readit.Core.Domain;
using Readit.Core.Repositories;
using Readit.Core.Services;
using Readit.Infra.Logging;
using System.Collections.ObjectModel;
using System.Windows;

namespace Readit.WPF.ViewModels
{
    public class CadastroGeneroViewModel : Screen
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

        private readonly IUsuarioService _usuarioService;
        private readonly IGeneroRepository _generoRepository;
        private readonly ILoggingService _logger;

        public CadastroGeneroViewModel(IUsuarioService usuarioService, IGeneroRepository generoRepository, ILoggingService logger)
        {
            _usuarioService = usuarioService;
            _generoRepository = generoRepository;
            _logger = logger;
            _exibirMenuAdministrador = _usuarioService.UsuarioLogado.Administrador;

            Task.Run(() => PopularGeneros()).ConfigureAwait(false);
        }

        public async Task PopularGeneros()
        {
            var generos = await _generoRepository.BuscarGenerosPorObraAsync(null).ConfigureAwait(false);

            ListaGeneros = new ObservableCollection<Generos>(generos.OrderBy(x => x.Nome));
            Generos = new ObservableCollection<Generos>(generos.OrderBy(x => x.Nome));
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

        public void CadastrarGenero()
        {
            AplicarLoading(true);

            Task.Run(() => CadastrarGeneroAsync()).ConfigureAwait(false);
        }

        public async Task CadastrarGeneroAsync()
        {
            List<string> erros = ValidarCampos();

            if (erros.Count > 0)
            {
                await ExibirMensagemFlashAsync("Erro", erros);
                AplicarLoading(false);
                return;
            }

            if ((await _generoRepository.BuscarGenerosPorNomeAsync(GeneroDigitado).ConfigureAwait(false)).Count != 0)
            {
                await ExibirMensagemFlashAsync("Erro", ["O Nome do Gênero inserido já existe no sistema."]);
                AplicarLoading(false);
                return;
            }

            try
            {
                bool sucesso = await _generoRepository.CadastrarGeneroAsync(new Generos
                {
                    Nome = GeneroDigitado
                }).ConfigureAwait(false);

                if (sucesso)
                {
                    await ExibirMensagemFlashAsync("Sucesso", ["Gênero cadastrado com sucesso!"]);
                }
                else
                {
                    await ExibirMensagemFlashAsync("Erro", ["Ocorreu um erro ao realizar o cadastro, favor tentar novamente em alguns minutos."]);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "CadastrarGeneroThread()");
                await ExibirMensagemFlashAsync("Erro", ["Ocorreu um erro ao realizar o cadastro, favor tentar novamente em alguns minutos."]);
            }
            finally
            {
                await Application.Current.Dispatcher.InvokeAsync(LimparDados);
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

            Task.Run(() => PopularGeneros()).ConfigureAwait(false);
        }

        public void AplicarLoading(bool loading)
        {
            Loading = !loading ? false : true;
            HabilitarCampos = !loading ? true : false;
        }

        public void SelecionarCadastro()
        {
            _ = ActiveView.OpenItemMain<SelecaoCadastroViewModel>();
        }

        public void PaginaInicial()
        {
            _ = ActiveView.OpenItemMain<PaginaInicialViewModel>();
        }

        public void EditarPerfil()
        {
            _ = ActiveView.OpenItemMain<EditarPerfilViewModel>();
        }

        public void BookmarksUsuario()
        {
            _ = ActiveView.OpenItemMain<BookmarksViewModel>();
        }

        public void ListagemObras()
        {
            _ = ActiveView.OpenItemMain<ListagemObrasViewModel>();
        }
    }
}