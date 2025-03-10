using Caliburn.Micro;
using Readit.Core.Domain;
using Readit.Core.Repositories;
using Readit.Core.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Readit.WPF.ViewModels
{
    public class LeituraCapituloViewModel : Screen
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

        private bool isUpdatingCapituloSelecionado = false;  // Flag para controle da atualização interna

        private CapitulosObra _capituloSelecionado;

        public CapitulosObra CapituloSelecionado
        {
            get => _capituloSelecionado;
            set
            {
                _capituloSelecionado = value;
                NotifyOfPropertyChange(() => CapituloSelecionado);

                // Não faz nada se já estiver atualizando o CapituloSelecionado
                if (isUpdatingCapituloSelecionado) return;

                AplicarLoading(true);
                _ = ConfigurarMudancaPaginaThread(null, true);
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

        private readonly IUsuarioService _usuarioService;
        private readonly ICapituloRepository _capituloRepository;
        private readonly ICapituloService _capituloService;
        private readonly IArquivoService _arquivoService;

        public LeituraCapituloViewModel(IUsuarioService usuarioService, ICapituloRepository capituloRepository, ICapituloService capituloService, IArquivoService arquivoService, ChapterInfo chapter)
        {
            _usuarioService = usuarioService;
            _capituloRepository = capituloRepository;
            _capituloService = capituloService;
            _arquivoService = arquivoService;
            _exibirMenuAdministrador = _usuarioService.UsuarioLogado.Administrador;
            _texts = _arquivoService.ExtrairDadosFrasesLoading();

            AplicarLoading(true);

            Task.Run(() => CarregarDadosCapituloAsync(chapter)).ConfigureAwait(false);
        }

        public async Task CarregarDadosCapituloAsync(ChapterInfo chapter)
        {
            (Capitulos, DadosCapituloAtual) = _capituloService.FormatarDadosPaginasCapitulo(await _capituloRepository.BuscarCapituloObrasPorIdAsync(chapter.ObraId, chapter.ChapterId, true, true).ConfigureAwait(false));
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

        public async Task CapituloAnterior()
        {
            AplicarLoading(true);

            await ConfigurarMudancaPaginaThread("Capítulo Anterior", false);
        }

        public async Task ProximoCapitulo()
        {
            AplicarLoading(true);
            await ConfigurarMudancaPaginaThread("Próximo Capítulo", false);
        }

        public async Task ConfigurarMudancaPaginaThread(string tipoBotao, bool alteradoViaCombobox)
        {
            if (Capitulos.Count == 0 || CapituloSelecionado == null) return;

            int index = Capitulos.IndexOf(CapituloSelecionado);
            int novoIndex = (tipoBotao is "Próximo Capítulo") && index < Capitulos.Count - 1 ? index + 1 :
                            (tipoBotao is "Capítulo Anterior") && index > 0 ? index - 1 : index;

            if (novoIndex != index || alteradoViaCombobox)
            {
                // Marca que a atualização está acontecendo internamente, para evitar disparar o setter
                isUpdatingCapituloSelecionado = true;

                // Se alteradoViaCombobox for true, mantém o CapituloSelecionado atual
                var capituloParaAtualizar = alteradoViaCombobox ? CapituloSelecionado : Capitulos[novoIndex];

                // Alterar CapituloSelecionado de maneira assíncrona sem disparar o setter
                CapituloSelecionado = capituloParaAtualizar;

                // Carregar dados do capítulo assíncrono
                var capitulo = await _capituloRepository.BuscarCapituloObrasPorIdAsync(
                    capituloParaAtualizar.ObraId, capituloParaAtualizar.Id, false, true).ConfigureAwait(false);

                DadosCapituloAtual = _capituloService.FormatarDadosPaginasCapitulo(capitulo).Item2;
                Paginas = DadosCapituloAtual.ListaPaginas;
                TituloCapitulo = $"{DadosCapituloAtual.NomeObra} - {DadosCapituloAtual.NumeroCapituloDisplay}";

                // Marca a operação como concluída
                isUpdatingCapituloSelecionado = false;
            }

            CapituloAnteriorHabilitado = Capitulos.Any(x => x.NumeroCapitulo == CapituloSelecionado.NumeroCapitulo - 1);
            ProximoCapituloHabilitado = Capitulos.Any(x => x.NumeroCapitulo == CapituloSelecionado.NumeroCapitulo + 1);

            AplicarLoading(false);
        }

        public async void AplicarLoading(bool loading)
        {
            if (loading)
            {
                Loading = true;
                ExibirSecoes = Visibility.Collapsed;

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
                    await Task.Delay(100);
                    _timer?.Stop();
                    Loading = false;
                    ExibirSecoes = Visibility.Visible;
                }
                catch { }
            }
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