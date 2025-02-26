using Caliburn.Micro;
using Microsoft.WindowsAPICodePack.Dialogs;
using readit.Controls;
using readit.Data;
using readit.Enums;
using readit.Models;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;

namespace readit.ViewModels
{
    public class CadastroObraViewModel : Screen
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

        private string _imagemSelecionada = "../Images/upload-image.png";

        public string ImagemSelecionada
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

        private ControlPrincipal cp = new ControlPrincipal();

        private ControlLogs clog = new ControlLogs();

        private DBConnection db = new DBConnection();

        public CadastroObraViewModel()
        {
            Thread thread = new(PopularCampos) { IsBackground = true };
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
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

        public void PopularCampos()
        {
            PopularObras();
            PopularGeneros();
            PopularTipos();
            PopularStatus();
        }

        public void PopularObras()
        {
            var obras = db.BuscarObrasPorId(null).OrderBy(x => x.NomeObra);

            ListaObras = new(obras);
            Obras = new(obras);
        }

        public void PopularGeneros()
        {
            var generos = db.BuscarGenerosPorObra(null).OrderBy(x => x.Nome);

            ListaGeneros = new(generos);
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

        public void CadastrarObra()
        {
            AplicarLoading(true);

            Thread thread = new(CadastrarObraThread) { IsBackground = true };
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        public void CadastrarObraThread()
        {
            List<string> erros = ValidarCampos();

            if (erros.Count > 0)
            {
                _ = ExibirMensagemFlashAsync("Erro", erros);
                AplicarLoading(false);
                return;
            }

            if (db.BuscarObrasPorNome(ObraDigitada).Count != 0)
            {
                _ = ExibirMensagemFlashAsync("Erro", ["O Nome da Obra inserido já existe no sistema."]);
                AplicarLoading(false);
                return;
            }

            try
            {
                bool sucesso = db.CadastrarObra(new Obras
                {
                    NomeObra = ObraDigitada,
                    Status = (byte)StatusSelecionado.Id,
                    Tipo = (byte)TipoSelecionado.Id,
                    Descricao = DescricaoObra,
                    UsuarioId = Global.UsuarioLogado.Id
                },
                new Imagens
                {
                    Imagem = cp.ConvertImageToByteArray(ImagemSelecionada),
                    Formato = Path.GetExtension(ImagemSelecionada),
                    Tipo = (byte)EnumObra.TipoImagem.Obra
                },
                ListaGeneros.Where(x => x.IsSelected).ToList());

                if (sucesso)
                {
                    _ = ExibirMensagemFlashAsync("Sucesso", ["Obra cadastrada com sucesso!"]);
                }
                else
                {
                    _ = ExibirMensagemFlashAsync("Erro", ["Ocorreu um erro ao realizar o cadastro, favor tentar novamente em alguns minutos."]);
                }
            }
            catch (Exception e)
            {
                clog.RealizarLogExcecao(e.ToString(), "CadastrarObraThread()");
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

            if (string.IsNullOrEmpty(ImagemSelecionada) || ImagemSelecionada.Equals("../Images/upload-image.png")) { erros.Add("A Imagem da capa da obra não foi selecionada."); }
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
                    ImagemSelecionada = dialog.FileName;

                    string nomeImagem = Path.GetFileName(dialog.FileName);
                    TxtUploadFotoObra = dialog.FileName.Length <= 31 ? nomeImagem : $"{nomeImagem[..31]}...";
                }
            }
            catch (Exception e)
            {
                clog.RealizarLogExcecao(e.ToString(), "SelecionarImagem()");
            }
        }

        public void LimparDados()
        {
            ImagemSelecionada = "../Images/upload-image.png";
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
            AplicarLoading(false);

            Thread thread = new(PopularObras) { IsBackground = true };
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        public void AplicarLoading(bool loading)
        {
            Loading = !loading ? false : true;
            HabilitarCampos = !loading ? true : false;
        }

        public void AtualizarGenerosSelecionadosDisplay()
        {
            NotifyOfPropertyChange(() => GenerosSelecionadosDisplay);
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