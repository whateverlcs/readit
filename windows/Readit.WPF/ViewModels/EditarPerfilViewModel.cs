using Caliburn.Micro;
using Microsoft.WindowsAPICodePack.Dialogs;
using Readit.Core.Domain;
using Readit.Core.Repositories;
using Readit.Core.Services;
using Readit.Infra.Logging;
using Readit.WPF.Infrastructure;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Media;

namespace Readit.WPF.ViewModels
{
    public class EditarPerfilViewModel : Screen
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

        private string _nomeUsuario;

        public string NomeUsuario
        {
            get { return _nomeUsuario; }
            set
            {
                _nomeUsuario = value;
                NotifyOfPropertyChange(() => NomeUsuario);
            }
        }

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

        private ImageSource _imagemPerfil;

        public ImageSource ImagemPerfil
        {
            get { return _imagemPerfil; }
            set
            {
                _imagemPerfil = value;
                NotifyOfPropertyChange(() => ImagemPerfil);
            }
        }

        public string CaminhoNovaImagem;

        private ObservableCollection<Preferencias> _listaPreferencias = new ObservableCollection<Preferencias>();

        public ObservableCollection<Preferencias> ListaPreferencias
        {
            get { return _listaPreferencias; }
            set
            {
                _listaPreferencias = value;
                NotifyOfPropertyChange(() => ListaPreferencias);
            }
        }

        public List<PreferenciasUsuario> ListaPreferenciasUsuario;

        /// <summary>
        /// Exibe as preferencias selecionadas pelo usuário que irão aparecer no combobox separadas por virgula.
        /// </summary>
        public string PreferenciasSelecionadasDisplay
        {
            get { return string.Join(", ", ListaPreferencias.Where(i => i.IsSelected).Select(i => i.Nome)); }
            set
            {
                NotifyOfPropertyChange(() => PreferenciasSelecionadasDisplay);
            }
        }

        private readonly IUsuarioService _usuarioService;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IImagemRepository _imagemRepository;
        private readonly IPreferenciasRepository _preferenciasRepository;
        private readonly ILoggingService _logger;
        private readonly IImagemService _imagemService;

        public EditarPerfilViewModel(IUsuarioService usuarioService, IUsuarioRepository usuarioRepository, IImagemRepository imagemRepository, IPreferenciasRepository preferenciasRepository, ILoggingService logger, IImagemService imagemService)
        {
            _usuarioService = usuarioService;
            _usuarioRepository = usuarioRepository;
            _imagemRepository = imagemRepository;
            _preferenciasRepository = preferenciasRepository;
            _logger = logger;
            _imagemService = imagemService;
            _exibirMenuAdministrador = _usuarioService.UsuarioLogado.Administrador;

            Task.Run(() => CarregarInformacoesPerfilAsync()).ConfigureAwait(false);
        }

        public async Task CarregarInformacoesPerfilAsync()
        {
            var usuarioLogado = (await _usuarioRepository.BuscarUsuarioPorEmailAsync(_usuarioService.UsuarioLogado.Email).ConfigureAwait(false)).FirstOrDefault();
            var imagemUsuario = (await _imagemRepository.BuscarImagemPorIdAsync(Convert.ToInt32(usuarioLogado.IdImagem)).ConfigureAwait(false)).FirstOrDefault();

            NomeCompleto = usuarioLogado.Nome;
            NomeUsuario = usuarioLogado.Nome;
            Apelido = usuarioLogado.Apelido;
            Email = usuarioLogado.Email;
            ImagemPerfil = _imagemService.ByteArrayToImage(imagemUsuario.Imagem);

            _usuarioService.UsuarioLogado = usuarioLogado;
            await PopularPreferenciasAsync().ConfigureAwait(false);
        }

        public void AtualizarPreferenciasSelecionadasDisplay()
        {
            NotifyOfPropertyChange(() => PreferenciasSelecionadasDisplay);
        }

        public async Task PopularPreferenciasAsync()
        {
            var preferencias = await _preferenciasRepository.BuscarPreferenciasAsync().ConfigureAwait(false);
            ListaPreferenciasUsuario = await _preferenciasRepository.BuscarPreferenciasUsuarioAsync().ConfigureAwait(false);

            var idsSelecionados = new HashSet<int>(ListaPreferenciasUsuario.Select(pu => pu.IdPreferencia));

            ListaPreferencias = new ObservableCollection<Preferencias>(
                preferencias.Select(p => { p.IsSelected = idsSelecionados.Contains(p.Id); return p; })
                            .OrderBy(p => p.Nome));

            PreferenciasSelecionadasDisplay = string.Join(", ", ListaPreferencias.Where(i => i.IsSelected).Select(i => i.Nome));
        }

        public void AtualizarInformacoes()
        {
            AplicarLoading(true);

            Task.Run(() => AtualizarInformacoesAsync()).ConfigureAwait(false);
        }

        public async Task AtualizarInformacoesAsync()
        {
            List<string> erros = ValidarCampos();

            if (erros.Count > 0)
            {
                await ExibirMensagemFlashAsync("Erro", erros);
                AplicarLoading(false);
                return;
            }

            if (!VerificarAlteracoesPerfil())
            {
                await ExibirMensagemFlashAsync("Erro", ["Não há dados a serem atualizados."]);
                AplicarLoading(false);
                return;
            }

            try
            {
#pragma warning disable
                bool sucesso = await _usuarioRepository.CadastrarUsuarioAsync(new Usuario
                {
                    Id = _usuarioService.UsuarioLogado.Id,
                    Nome = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(NomeCompleto),
                    Apelido = Apelido,
                    Senha = !string.IsNullOrEmpty(Senha) ? BC.EnhancedHashPassword(Senha, 13) : _usuarioService.UsuarioLogado.Senha,
                },
                !string.IsNullOrEmpty(CaminhoNovaImagem) ? new Imagens
                {
                    Id = (int)_usuarioService.UsuarioLogado.IdImagem,
                    Imagem = _imagemService.ConvertBitmapImageToByteArray(ImagemPerfil),
                    Formato = Path.GetExtension(CaminhoNovaImagem)
                } : null,
                ListaPreferencias.Where(x => x.IsSelected).ToList()).ConfigureAwait(false);
#pragma warning restore

                if (sucesso)
                {
                    await ExibirMensagemFlashAsync("Sucesso", ["Perfil atualizado com sucesso!"]);
                }
                else
                {
                    await ExibirMensagemFlashAsync("Erro", ["Ocorreu um erro ao atualizar o seu perfil, favor tentar novamente em alguns minutos."]);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "AtualizarInformacoesThread()");
                await ExibirMensagemFlashAsync("Erro", ["Ocorreu um erro ao realizar a atualização do seu perfil, favor tentar novamente em alguns minutos."]);
            }
            finally
            {
                await Application.Current.Dispatcher.InvokeAsync(LimparDados);
            }
        }

        public bool VerificarAlteracoesPerfil()
        {
            bool alterado = false;

            if (NomeCompleto != _usuarioService.UsuarioLogado.Nome) { alterado = true; }
            if (Apelido != _usuarioService.UsuarioLogado.Apelido) { alterado = true; }
            if (!string.IsNullOrEmpty(CaminhoNovaImagem)) { alterado = true; }
            if (!string.IsNullOrEmpty(Senha) && !BC.EnhancedVerify(Senha, _usuarioService.UsuarioLogado.Senha)) { alterado = true; }
            if (!new HashSet<int>(ListaPreferencias.Where(x => x.IsSelected).Select(g => g.Id)).SetEquals(ListaPreferenciasUsuario.Select(g => g.IdPreferencia))) { alterado = true; }

            return alterado;
        }

        public void RealizarUploadNovaFoto()
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
                    ImagemPerfil = _imagemService.ByteArrayToImage(_imagemService.ConvertImageToByteArray(dialog.FileName));
                    CaminhoNovaImagem = dialog.FileName;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "RealizarUploadNovaFoto()");
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

        public void LimparDados()
        {
            AplicarLoading(false);
            CaminhoNovaImagem = "";
            Senha = "";

            Task.Run(() => CarregarInformacoesPerfilAsync()).ConfigureAwait(false);
        }

        public void AplicarLoading(bool loading)
        {
            Loading = !loading ? false : true;
            HabilitarCampos = !loading ? true : false;
        }

        public List<string> ValidarCampos()
        {
            List<string> erros = [];

            if (!string.IsNullOrEmpty(NomeCompleto) && NomeCompleto.Length > 100) { erros.Add("O Nome Completo não pode ser maior do que 100 caracteres."); }
            if (string.IsNullOrEmpty(NomeCompleto)) { erros.Add("O Nome Completo não pode ser vazio."); }
            if (!string.IsNullOrEmpty(Apelido) && Apelido.Length > 50) { erros.Add("O Apelido não pode ser maior do que 50 caracteres."); }
            if (string.IsNullOrEmpty(Apelido)) { erros.Add("O Apelido não pode ser vazio."); }
            if (!string.IsNullOrEmpty(Senha) && Senha.Length > 255) { erros.Add("A Senha não pode ser maior do que 255 caracteres."); }

            return erros;
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