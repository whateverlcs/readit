using Caliburn.Micro;
using Microsoft.WindowsAPICodePack.Dialogs;
using readit.Controls;
using readit.Data;
using readit.Models;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Media;

namespace readit.ViewModels
{
    public class EditarPerfilViewModel : Screen
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

        private TipoVisualizacaoObra _tipoVisualizacaoLeituraSelecionado;

        public TipoVisualizacaoObra TipoVisualizacaoLeituraSelecionado
        {
            get { return _tipoVisualizacaoLeituraSelecionado; }
            set
            {
                _tipoVisualizacaoLeituraSelecionado = value;
                NotifyOfPropertyChange(() => TipoVisualizacaoLeituraSelecionado);
            }
        }

        private ObservableCollection<TipoVisualizacaoObra> _listaTiposVisualizacaoLeitura = new ObservableCollection<TipoVisualizacaoObra>();

        public ObservableCollection<TipoVisualizacaoObra> ListaTiposVisualizacaoLeitura
        {
            get { return _listaTiposVisualizacaoLeitura; }
            set
            {
                _listaTiposVisualizacaoLeitura = value;
                NotifyOfPropertyChange(() => ListaTiposVisualizacaoLeitura);
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

        private ControlPrincipal cp = new ControlPrincipal();

        private ControlLogs clog = new ControlLogs();

        private DBConnection db = new DBConnection();

        public EditarPerfilViewModel()
        {
            Thread thread = new(CarregarInformacoesPerfilThread) { IsBackground = true };
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        public void CarregarInformacoesPerfilThread()
        {
            var usuarioLogado = db.BuscarUsuarioPorEmail(Global.UsuarioLogado.Email).FirstOrDefault();
            var imagemUsuario = db.BuscarImagemPorId(Convert.ToInt32(usuarioLogado.IdImagem)).FirstOrDefault();
            var tiposVisualizacaoObra = db.BuscarTiposVisualizacaoObraPorId(0);
            var tipoVisualizacaoUsuario = db.BuscarTiposVisualizacaoObraUsuarioPorId(usuarioLogado.Id).FirstOrDefault();

            NomeCompleto = usuarioLogado.Nome;
            NomeUsuario = usuarioLogado.Nome;
            Apelido = usuarioLogado.Apelido;
            Email = usuarioLogado.Email;
            ImagemPerfil = cp.ByteArrayToImage(imagemUsuario.Imagem);
            ListaTiposVisualizacaoLeitura = new ObservableCollection<TipoVisualizacaoObra>(tiposVisualizacaoObra);
            TipoVisualizacaoLeituraSelecionado = tipoVisualizacaoUsuario != null ? ListaTiposVisualizacaoLeitura.Where(x => x.Id == tipoVisualizacaoUsuario.TipoVisualizacaoObraId).FirstOrDefault() : new TipoVisualizacaoObra();

            Global.UsuarioLogado = usuarioLogado;
        }

        public void AtualizarInformacoes()
        {
            AplicarLoading(true);

            Thread thread = new(AtualizarInformacoesThread) { IsBackground = true };
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        public void AtualizarInformacoesThread()
        {
            List<string> erros = ValidarCampos();

            if (erros.Count > 0)
            {
                _ = ExibirMensagemFlashAsync("Erro", erros);
                AplicarLoading(false);
                return;
            }

            if (!VerificarAlteracoesPerfil())
            {
                _ = ExibirMensagemFlashAsync("Erro", ["Não há dados a serem atualizados."]);
                AplicarLoading(false);
                return;
            }

            try
            {
#pragma warning disable
                bool sucesso = db.CadastrarUsuario(new Usuario
                {
                    Id = Global.UsuarioLogado.Id,
                    Nome = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(NomeCompleto),
                    Apelido = Apelido,
                    Senha = !string.IsNullOrEmpty(Senha) ? BC.EnhancedHashPassword(Senha, 13) : Global.UsuarioLogado.Senha,
                },
                !string.IsNullOrEmpty(CaminhoNovaImagem) ? new Imagens
                {
                    Id = (int)Global.UsuarioLogado.IdImagem,
                    Imagem = cp.ConvertBitmapImageToByteArray(ImagemPerfil),
                    Formato = Path.GetExtension(CaminhoNovaImagem)
                } : null,
                new TipoVisualizacaoObraUsuario
                {
                    TipoVisualizacaoObraId = TipoVisualizacaoLeituraSelecionado.Id,
                    UsuarioId = Global.UsuarioLogado.Id
                });
#pragma warning restore

                if (sucesso)
                {
                    _ = ExibirMensagemFlashAsync("Sucesso", ["Perfil atualizado com sucesso!"]);
                }
                else
                {
                    _ = ExibirMensagemFlashAsync("Erro", ["Ocorreu um erro ao atualizar o seu perfil, favor tentar novamente em alguns minutos."]);
                }
            }
            catch (Exception e)
            {
                clog.RealizarLogExcecao(e.ToString(), "AtualizarInformacoesThread()");
                _ = ExibirMensagemFlashAsync("Erro", ["Ocorreu um erro ao realizar a atualização do seu perfil, favor tentar novamente em alguns minutos."]);
            }
            finally
            {
                LimparDados();
            }
        }

        public bool VerificarAlteracoesPerfil()
        {
            bool alterado = false;

            if (NomeCompleto != Global.UsuarioLogado.Nome) { alterado = true; }
            if (Apelido != Global.UsuarioLogado.Apelido) { alterado = true; }
            if (!string.IsNullOrEmpty(CaminhoNovaImagem)) { alterado = true; }
            if (!string.IsNullOrEmpty(Senha) && !BC.EnhancedVerify(Senha, Global.UsuarioLogado.Senha)) { alterado = true; }

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
                    ImagemPerfil = cp.ByteArrayToImage(cp.ConvertImageToByteArray(dialog.FileName));
                    CaminhoNovaImagem = dialog.FileName;
                }
            }
            catch (Exception e)
            {
                clog.RealizarLogExcecao(e.ToString(), "RealizarUploadNovaFoto()");
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
            CaminhoNovaImagem = "";
            Senha = "";

            Thread thread = new(CarregarInformacoesPerfilThread) { IsBackground = true };
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
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
            if (TipoVisualizacaoLeituraSelecionado == null || TipoVisualizacaoLeituraSelecionado != null && TipoVisualizacaoLeituraSelecionado.Id == 0) { erros.Add("Selecione um tipo de visualização de leitura."); }

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

        public void BookmarksUsuario()
        {
            _ = ActiveView.OpenItemMain(new BookmarksViewModel());
        }
    }
}