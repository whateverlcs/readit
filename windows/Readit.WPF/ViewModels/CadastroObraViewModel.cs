using Caliburn.Micro;
using Microsoft.WindowsAPICodePack.Dialogs;
using Readit.Core.Domain;
using Readit.Core.Enums;
using Readit.Core.Repositories;
using Readit.Core.Services;
using Readit.Infra.Logging;
using Readit.WPF.Infrastructure;
using SharpCompress;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Media;

namespace Readit.WPF.ViewModels
{
    public class CadastroObraViewModel : Screen
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

        #region Cadastro Obra

        private string _obraDigitada;

        public string ObraDigitada
        {
            get { return _obraDigitada; }
            set
            {
                _obraDigitada = value;
                NotifyOfPropertyChange(() => ObraDigitada);
                AtualizarSugestoes();
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

        public ObservableCollection<Obras> Obras;

        private Generos _generoSelecionado;

        public Generos GeneroSelecionado
        {
            get { return _generoSelecionado; }
            set
            {
                _generoSelecionado = value;
                NotifyOfPropertyChange(() => GeneroSelecionado);
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

        /// <summary>
        /// Irá ser exibido no combobox os generos selecionados, separados por virgula.
        /// </summary>
        public string GenerosSelecionadosDisplay
        {
            get { return string.Join(", ", ListaGeneros.Where(i => i.IsSelected).Select(i => i.Nome)); }
            set
            {
                NotifyOfPropertyChange(() => GenerosSelecionadosDisplay);
            }
        }

        private Status _statusSelecionado;

        public Status StatusSelecionado
        {
            get { return _statusSelecionado; }
            set
            {
                _statusSelecionado = value;
                NotifyOfPropertyChange(() => StatusSelecionado);
            }
        }

        private ObservableCollection<Status> _listaStatus = new ObservableCollection<Status>();

        public ObservableCollection<Status> ListaStatus
        {
            get { return _listaStatus; }
            set
            {
                _listaStatus = value;
                NotifyOfPropertyChange(() => ListaStatus);
            }
        }

        private Tipos _tipoSelecionado;

        public Tipos TipoSelecionado
        {
            get { return _tipoSelecionado; }
            set
            {
                _tipoSelecionado = value;
                NotifyOfPropertyChange(() => TipoSelecionado);
            }
        }

        private ObservableCollection<Tipos> _listaTipos = new ObservableCollection<Tipos>();

        public ObservableCollection<Tipos> ListaTipos
        {
            get { return _listaTipos; }
            set
            {
                _listaTipos = value;
                NotifyOfPropertyChange(() => ListaTipos);
            }
        }

        private string _descricaoObra;

        public string DescricaoObra
        {
            get { return _descricaoObra; }
            set
            {
                _descricaoObra = value;
                NotifyOfPropertyChange(() => DescricaoObra);
            }
        }

        private string CaminhoImagem = "../Resources/Images/upload-image.png";

        private ImageSource _imagemSelecionada;

        public ImageSource ImagemSelecionada
        {
            get { return _imagemSelecionada; }
            set
            {
                _imagemSelecionada = value;
                NotifyOfPropertyChange(() => ImagemSelecionada);
            }
        }

        private string _txtUploadFotoObra = "Realizar Upload da Foto da Obra";

        public string TxtUploadFotoObra
        {
            get { return _txtUploadFotoObra; }
            set
            {
                _txtUploadFotoObra = value;
                NotifyOfPropertyChange(() => TxtUploadFotoObra);
            }
        }

        private string _tituloPrincipal = "CADASTRAR OBRAS";

        public string TituloPrincipal
        {
            get { return _tituloPrincipal; }
            set
            {
                _tituloPrincipal = value;
                NotifyOfPropertyChange(() => TituloPrincipal);
            }
        }

        private string _tituloBotao = "Cadastrar Obra";

        public string TituloBotao
        {
            get { return _tituloBotao; }
            set
            {
                _tituloBotao = value;
                NotifyOfPropertyChange(() => TituloBotao);
            }
        }

        #endregion Cadastro Obra

        #region Edição de Obras

        private string ModoAtual = "Cadastrar";

        private ObservableCollection<Obras> _listaObrasEdicao = new ObservableCollection<Obras>();

        public ObservableCollection<Obras> ListaObrasEdicao
        {
            get { return _listaObrasEdicao; }
            set
            {
                _listaObrasEdicao = value;
                NotifyOfPropertyChange(() => ListaObrasEdicao);
            }
        }

        private Obras _obraSelecionada;

        public Obras ObraSelecionada
        {
            get { return _obraSelecionada; }
            set
            {
                _obraSelecionada = value;
                if (_obraSelecionada != null)
                {
                    AplicarLoading(true);
                    Task.Run(() => CarregarDadosEdicaoObra(_obraSelecionada)).ConfigureAwait(false);
                    NotifyOfPropertyChange(() => ObraSelecionada);
                }
            }
        }

        private bool _exibirSelectEdicao;

        public bool ExibirSelectEdicao
        {
            get { return _exibirSelectEdicao; }
            set
            {
                _exibirSelectEdicao = value;
                NotifyOfPropertyChange(() => ExibirSelectEdicao);
            }
        }

        private bool _habilitarSelectEdicao;

        public bool HabilitarSelectEdicao
        {
            get { return _habilitarSelectEdicao; }
            set
            {
                _habilitarSelectEdicao = value;
                NotifyOfPropertyChange(() => HabilitarSelectEdicao);
            }
        }

        private string _toggleTitulo = "EDITAR OBRAS";

        public string ToggleTitulo
        {
            get { return _toggleTitulo; }
            set
            {
                _toggleTitulo = value;
                NotifyOfPropertyChange(() => ToggleTitulo);
            }
        }

        #endregion Edição de Obras

        private readonly IUsuarioService _usuarioService;
        private readonly IObraRepository _obraRepository;
        private readonly IGeneroRepository _generoRepository;
        private readonly ILoggingService _logger;
        private readonly IImagemService _imagemService;

        public CadastroObraViewModel(IUsuarioService usuarioService, IObraRepository obraRepository, IGeneroRepository generoRepository, ILoggingService logger, IImagemService imagemService)
        {
            _usuarioService = usuarioService;
            _obraRepository = obraRepository;
            _generoRepository = generoRepository;
            _logger = logger;
            _imagemService = imagemService;
            _exibirMenuAdministrador = _usuarioService.UsuarioLogado.Administrador;

            Task.Run(() => PopularCamposAsync()).ConfigureAwait(false);
        }

        public void AlterarModo()
        {
            LimparDados();

            if (ModoAtual == "Cadastrar")
            {
                ModoAtual = "Editar";
                ToggleTitulo = "CADASTRAR OBRAS";
                ExibirSelectEdicao = true;
                TituloBotao = "Editar Obra";
                TituloPrincipal = "EDITAR OBRAS";
                HabilitarCampos = false;
                HabilitarSelectEdicao = true;
            }
            else
            {
                ModoAtual = "Cadastrar";
                ToggleTitulo = "EDITAR OBRAS";
                ExibirSelectEdicao = false;
                TituloBotao = "Cadastrar Obra";
                TituloPrincipal = "CADASTRAR OBRAS";
                HabilitarCampos = true;
                HabilitarSelectEdicao = false;
            }
        }

        public async Task CarregarDadosEdicaoObra(Obras obra)
        {
            if (obra != null)
            {
                var dados = await _obraRepository.BuscarDadosObraPorIdAsync(obra.Id).ConfigureAwait(false);

                if (dados != null)
                {
                    ObraDigitada = dados.Title;
                    ListaGeneros.ForEach(x =>
                    {
                        x.IsSelected = dados.Genres.Contains(x.Nome) ? true : false;
                    });
                    GenerosSelecionadosDisplay = string.Join(", ", ListaGeneros.Where(i => i.IsSelected).Select(i => i.Nome));
                    StatusSelecionado = ListaStatus.Where(x => x.Id == dados.StatusNumber).First();
                    TipoSelecionado = ListaTipos.Where(x => x.Id == dados.TipoNumber).First();
                    DescricaoObra = dados.Descricao;
                    ImagemSelecionada = _imagemService.ByteArrayToImage(dados.ImageByte);
                    CaminhoImagem = "Imagem Obra";
                    TxtUploadFotoObra = "Capa da obra";
                }
            }

            AplicarLoading(false);
        }

        public void AtualizarSugestoes()
        {
            if (string.IsNullOrWhiteSpace(ObraDigitada))
            {
                ListaObras = new ObservableCollection<Obras>(Obras);
            }
            else
            {
                var filtered = Obras
                    .Where(item => item.NomeObra.StartsWith(ObraDigitada, System.StringComparison.OrdinalIgnoreCase))
                    .ToList();

                ListaObras = new ObservableCollection<Obras>(filtered);
                NotifyOfPropertyChange(() => ListaObras);
            }
        }

        public async Task PopularCamposAsync()
        {
            await Task.WhenAll(
                PopularObrasAsync(),
                PopularGenerosAsync()
            ).ConfigureAwait(false);

            PopularTipos();
            PopularStatus();
            ImagemSelecionada = _imagemService.ByteArrayToImage(_imagemService.ConvertImageToByteArray(Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, "Resources/Images", "upload-image.png")));
            CaminhoImagem = "../Resources/Images/upload-image.png";
        }

        public async Task PopularObrasAsync()
        {
            var obras = await _obraRepository.BuscarObrasPorIdAsync(null).ConfigureAwait(false);
            ListaObras = Obras = ListaObrasEdicao = new ObservableCollection<Obras>(obras.OrderBy(x => x.NomeObra));
        }

        public async Task PopularGenerosAsync()
        {
            var generos = await _generoRepository.BuscarGenerosPorObraAsync(null).ConfigureAwait(false);
            ListaGeneros = new ObservableCollection<Generos>(generos.OrderBy(x => x.Nome));
        }

        public void PopularStatus()
        {
            ListaStatus = [
                new(){ Id = (int)EnumObra.StatusObra.EmAndamento, Nome = "Em Andamento" },
                new(){ Id = (int)EnumObra.StatusObra.EmHiato, Nome = "Em Hiato" },
                new(){ Id = (int)EnumObra.StatusObra.Finalizado, Nome = "Finalizado" },
                new(){ Id = (int)EnumObra.StatusObra.Cancelado, Nome = "Cancelado" },
                new(){ Id = (int)EnumObra.StatusObra.Dropado, Nome = "Dropado" }
            ];
        }

        public void PopularTipos()
        {
            ListaTipos = [
                new(){ Id = (int)EnumObra.TipoObra.Manhwa, Nome = "Manhwa" },
                new(){ Id = (int)EnumObra.TipoObra.Donghua, Nome = "Donghua" },
                new(){ Id = (int)EnumObra.TipoObra.Manga, Nome = "Manga" }
            ];
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

        public void CadastrarEditarObra()
        {
            AplicarLoading(true);

            Task.Run(() => CadastrarEditarObraAsync()).ConfigureAwait(false);
        }

        public async Task CadastrarEditarObraAsync()
        {
            List<string> erros = ValidarCampos();

            if (erros.Count > 0)
            {
                await ExibirMensagemFlashAsync("Erro", erros);
                AplicarLoading(false);
                return;
            }

            if (ModoAtual == "Cadastrar" && (await _obraRepository.BuscarObrasPorNomeAsync(ObraDigitada).ConfigureAwait(false)).Count > 0)
            {
                await ExibirMensagemFlashAsync("Erro", ["O Nome da Obra inserido já existe no sistema."]);
                AplicarLoading(false);
                return;
            }

            try
            {
                bool sucesso = await _obraRepository.CadastrarEditarObraAsync(new Obras
                {
                    Id = ModoAtual == "Editar" ? ObraSelecionada.Id : 0,
                    NomeObra = ObraDigitada,
                    Status = (byte)StatusSelecionado.Id,
                    Tipo = (byte)TipoSelecionado.Id,
                    Descricao = DescricaoObra,
                    UsuarioId = _usuarioService.UsuarioLogado.Id,
                    ImagemId = ModoAtual == "Editar" ? ObraSelecionada.ImagemId : 0,
                },
                new Imagens
                {
                    Imagem = _imagemService.ConvertBitmapImageToByteArray(ImagemSelecionada),
                    Formato = Path.GetExtension(CaminhoImagem),
                    Tipo = (byte)EnumObra.TipoImagem.Obra
                },
                ListaGeneros.Where(x => x.IsSelected).ToList()).ConfigureAwait(false);

                if (sucesso)
                {
                    await ExibirMensagemFlashAsync("Sucesso", [$"Obra {(ModoAtual == "Cadastrar" ? "cadastrada" : "editada")} com sucesso!"]);
                }
                else
                {
                    await ExibirMensagemFlashAsync("Erro", [$"Ocorreu um erro ao realizar {(ModoAtual == "Cadastrar" ? "o cadastro" : "a edição")}, favor tentar novamente em alguns minutos."]);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "CadastrarEditarObraAsync()");
                await ExibirMensagemFlashAsync("Erro", [$"Ocorreu um erro ao realizar {(ModoAtual == "Cadastrar" ? "o cadastro" : "a edição")}, favor tentar novamente em alguns minutos."]);
            }
            finally
            {
                await Application.Current.Dispatcher.InvokeAsync(LimparDados);
            }
        }

        public List<string> ValidarCampos()
        {
            List<string> erros = [];

            if (string.IsNullOrEmpty(CaminhoImagem) || CaminhoImagem.Equals("../Resources/Images/upload-image.png")) { erros.Add("A Imagem da capa da obra não foi selecionada."); }
            if (!string.IsNullOrEmpty(ObraDigitada) && ObraDigitada.Length > 255) { erros.Add("O Nome da Obra não pode ser maior do que 255 caracteres."); }
            if (string.IsNullOrEmpty(ObraDigitada)) { erros.Add("O Nome da Obra não foi digitado."); }
            if (ListaGeneros.All(x => !x.IsSelected)) { erros.Add("Os Gêneros da Obra não foram selecionados."); }
            if (StatusSelecionado == null) { erros.Add("O Status da Obra não foi selecionado."); }
            if (TipoSelecionado == null) { erros.Add("O Tipo da Obra não foi selecionado."); }
            if (string.IsNullOrEmpty(DescricaoObra)) { erros.Add("A Descrição da Obra não foi selecionada."); }

            return erros;
        }

        public void SelecionarImagem()
        {
            try
            {
                CommonOpenFileDialog dialog = new CommonOpenFileDialog()
                {
                    Title = "Selecione uma imagem",
                    EnsureFileExists = true,
                    Filters = { new CommonFileDialogFilter("Image Files", "*.jpg;*.jpeg;*.png;") },
                    InitialDirectory = @"C:\"
                };

                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    ImagemSelecionada = _imagemService.ByteArrayToImage(_imagemService.ConvertImageToByteArray(dialog.FileName));
                    CaminhoImagem = dialog.FileName;

                    string nomeImagem = Path.GetFileName(dialog.FileName);
                    TxtUploadFotoObra = nomeImagem.Length <= 31 ? nomeImagem : $"{nomeImagem[..31]}...";
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "SelecionarImagem()");
            }
        }

        public void LimparDados()
        {
            CaminhoImagem = "../Resources/Images/upload-image.png";
            ImagemSelecionada = _imagemService.ByteArrayToImage(_imagemService.ConvertImageToByteArray(Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, "Resources/Images", "upload-image.png")));
            TxtUploadFotoObra = "Realizar Upload da Foto da Obra";
            var listaAuxGeneros = ListaGeneros;
            listaAuxGeneros.ToList().ForEach(x => x.IsSelected = false);
            ListaGeneros = new ObservableCollection<Generos>(listaAuxGeneros);
            GenerosSelecionadosDisplay = "";
            ObraDigitada = "";
            DescricaoObra = "";
            GeneroSelecionado = null;
            StatusSelecionado = null;
            TipoSelecionado = null;
            ObraSelecionada = null;
            AplicarLoading(false);

            Task.Run(() => PopularObrasAsync()).ConfigureAwait(false);
        }

        public void AplicarLoading(bool loading)
        {
            Loading = !loading ? false : true;
            HabilitarCampos = !loading ? true : false;
            HabilitarSelectEdicao = !loading ? true : false;
        }

        public void AtualizarGenerosSelecionadosDisplay()
        {
            NotifyOfPropertyChange(() => GenerosSelecionadosDisplay);
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