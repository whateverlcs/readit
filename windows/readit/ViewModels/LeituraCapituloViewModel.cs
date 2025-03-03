using Caliburn.Micro;
using readit.Controls;
using readit.Data;
using readit.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace readit.ViewModels
{
    public class LeituraCapituloViewModel : Screen
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

        #region Loading

        private Visibility _exibirSecoes;

        public Visibility ExibirSecoes
        {
            get { return _exibirSecoes; }
            set
            {
                _exibirSecoes = value;
                NotifyOfPropertyChange(() => ExibirSecoes);
            }
        }

        private List<string> _texts;
        private int _index, _textIndex;
        private bool _removing;
        private DispatcherTimer _timer;

        private string _animatedText;

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

        private List<CapitulosObra> _capitulos;

        public List<CapitulosObra> Capitulos
        {
            get => _capitulos;
            set
            {
                _capitulos = value;
                NotifyOfPropertyChange(() => Capitulos);
            }
        }

        private CapitulosObra _capituloSelecionado;

        public CapitulosObra CapituloSelecionado
        {
            get => _capituloSelecionado;
            set
            {
                if (_capituloSelecionado != null && value.Id != _capituloSelecionado.Id)
                {
                    AplicarLoading(true);
                    Thread thread = new(() => ConfigurarMudancaPaginaThread(null, true)) { IsBackground = true };
                    thread.SetApartmentState(ApartmentState.STA);
                    thread.Start();
                }

                _capituloSelecionado = value;
                NotifyOfPropertyChange(() => CapituloSelecionado);
            }
        }

        private CapitulosObra DadosCapituloAtual;

        private List<PaginasCapitulo> _paginas;

        public List<PaginasCapitulo> Paginas
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

        private ControlPrincipal cp = new ControlPrincipal();

        private ControlLogs clog = new ControlLogs();

        private DBConnection db = new DBConnection();

        public LeituraCapituloViewModel(ChapterInfo chapter)
        {
            AplicarLoading(true);

            Thread thread = new(() => CarregarDadosCapituloThread(chapter)) { IsBackground = true };
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        public void CarregarDadosCapituloThread(ChapterInfo chapter)
        {
            (Capitulos, DadosCapituloAtual) = cp.FormatarDadosPaginasCapitulo(db.BuscarCapituloObrasPorId(chapter.ObraId, chapter.ChapterId, true, true));
            Paginas = DadosCapituloAtual.ListaPaginas;
            CapituloSelecionado = Capitulos.Where(x => x.Id == chapter.ChapterId).FirstOrDefault();
            TituloCapitulo = $"{DadosCapituloAtual.NomeObra} - {DadosCapituloAtual.NumeroCapituloDisplay}";
            CapituloAnteriorHabilitado = Capitulos.Any(x => x.NumeroCapitulo == CapituloSelecionado.NumeroCapitulo - 1);
            ProximoCapituloHabilitado = Capitulos.Any(x => x.NumeroCapitulo == CapituloSelecionado.NumeroCapitulo + 1);
            AplicarLoading(false);
        }

        public void OnScrollChanged(ScrollChangedEventArgs e)
        {
            // Verifica a posição do scroll
            double scrollPosition = e.VerticalOffset;

            // Exibe o botão "Voltar ao Topo" se o usuário rolar para baixo (mais de 100 pixels)
            ExibirBotaoVoltarTopo = scrollPosition > 100;
        }

        public void CapituloAnterior()
        {
            AplicarLoading(true);

            Thread thread = new(() => ConfigurarMudancaPaginaThread("Capítulo Anterior", false)) { IsBackground = true };
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        public void ProximoCapitulo()
        {
            AplicarLoading(true);

            Thread thread = new(() => ConfigurarMudancaPaginaThread("Próximo Capítulo", false)) { IsBackground = true };
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        public void ConfigurarMudancaPaginaThread(string tipoBotao, bool alteradoViaCombobox)
        {
            if (Capitulos.Count == 0 || CapituloSelecionado == null) return;

            int index = Capitulos.IndexOf(CapituloSelecionado);
            int novoIndex = (tipoBotao is null or "Próximo Capítulo") && index < Capitulos.Count - 1 ? index + 1 :
                            (tipoBotao is null or "Capítulo Anterior") && index > 0 ? index - 1 : index;

            if (novoIndex != index)
            {
                var dbContext = new DBConnection();
                if (!alteradoViaCombobox) { CapituloSelecionado = Capitulos[novoIndex]; }
                DadosCapituloAtual = cp.FormatarDadosPaginasCapitulo(dbContext.BuscarCapituloObrasPorId(CapituloSelecionado.ObraId, CapituloSelecionado.Id, false, true)).Item2;
                Paginas = DadosCapituloAtual.ListaPaginas;
                TituloCapitulo = $"{DadosCapituloAtual.NomeObra} - {DadosCapituloAtual.NumeroCapituloDisplay}";
            }

            CapituloAnteriorHabilitado = Capitulos.Any(x => x.NumeroCapitulo == CapituloSelecionado.NumeroCapitulo - 1);
            ProximoCapituloHabilitado = Capitulos.Any(x => x.NumeroCapitulo == CapituloSelecionado.NumeroCapitulo + 1);

            AplicarLoading(false);
        }

        public void AplicarLoading(bool loading)
        {
            if (loading)
            {
                Loading = true;
                ExibirSecoes = Visibility.Collapsed;

                _texts = cp.ExtrairDadosFrasesLoading();
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
                    Thread.Sleep(100);
                    _timer?.Stop();
                    Loading = false;
                    ExibirSecoes = Visibility.Visible;
                }
                catch { }
            }
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

        public void BookmarksUsuario()
        {
            _ = ActiveView.OpenItemMain(new BookmarksViewModel());
        }

        public void ListagemObras()
        {
            _ = ActiveView.OpenItemMain(new ListagemObrasViewModel());
        }
    }
}