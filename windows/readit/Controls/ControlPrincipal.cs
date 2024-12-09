using readit.Data;
using readit.Models;
using SharpCompress.Archives;
using SharpCompress.Common;
using System.IO;
using System.Net.Mail;

namespace readit.Controls
{
    public class ControlPrincipal
    {
#if DEBUG
        public string caminhoPastaTemporaria = App.GetSetting("temp");
#else
        public caminhoPastaTemporaria = AppDomain.CurrentDomain.BaseDirectory;
#endif

        private ControlLogs clog = new ControlLogs();

        private DBConnection db = new DBConnection();

        public List<string> ValidarCampos(string nome, string apelido, string senha, string email)
        {
            List<string> erros = [];

            if (!string.IsNullOrEmpty(nome) && nome.Length > 100) { erros.Add("O Nome Completo não pode ser maior do que 100 caracteres."); }
            if (!string.IsNullOrEmpty(apelido) && apelido.Length > 50) { erros.Add("O Apelido não pode ser maior do que 50 caracteres."); }
            if (!string.IsNullOrEmpty(senha) && senha.Length > 255) { erros.Add("A Senha não pode ser maior do que 255 caracteres."); }
            if (!string.IsNullOrEmpty(email) && email.Length > 100) { erros.Add("O E-mail não pode ser maior do que 100 caracteres."); }
            try { if (!string.IsNullOrEmpty(email)) { var mailAddress = new MailAddress(email); } } catch (FormatException) { erros.Add("O E-mail inserido não é um e-mail válido."); }

            return erros;
        }

        public byte[] ConvertImageToByteArray(string imagePath)
        {
            return File.ReadAllBytes(imagePath);
        }

        public List<CapitulosObra> IdentificarArquivosInseridos(int obraId)
        {
            try
            {
                DirectoryInfo di = new DirectoryInfo(caminhoPastaTemporaria);

                List<CapitulosObra> listaCapitulosObra = new List<CapitulosObra>();

                foreach (var capitulo in Global.ListaCapitulosSelecionados)
                {
                    DeletarArquivosPastaTemporaria(di);

                    di.Refresh();

                    CapitulosObra cap = new CapitulosObra
                    {
                        NumeroCapitulo = Convert.ToInt32(Path.GetFileNameWithoutExtension(capitulo)),
                        ObraId = obraId,
                        UsuarioId = Global.UsuarioLogado.Id
                    };

                    ArchiveFactory.WriteToDirectory(capitulo, caminhoPastaTemporaria, new ExtractionOptions { ExtractFullPath = true, Overwrite = true });

                    foreach (var paginas in di.GetFiles())
                    {
                        PaginasCapitulo pag = new PaginasCapitulo
                        {
                            NumeroPagina = Convert.ToInt32(Path.GetFileNameWithoutExtension(paginas.FullName)),
                            Pagina = ConvertImageToByteArray(paginas.FullName),
                            Tamanho = $"{paginas.Length / 1024.0} KB"
                        };

                        cap.ListaPaginas.Add(pag);
                    }

                    listaCapitulosObra.Add(cap);
                }

                return listaCapitulosObra;
            }
            catch (Exception e)
            {
                clog.RealizarLogExcecao(e.ToString(), "IdentificarArquivosInseridos()");
                return new List<CapitulosObra>();
            }
        }

        public void DeletarArquivosPastaTemporaria(DirectoryInfo di)
        {
            try
            {
                foreach (var file in di.GetFiles())
                {
                    File.Delete(file.FullName);
                }
            }
            catch (Exception e)
            {
                clog.RealizarLogExcecao(e.ToString(), "DeletarArquivosPastaTemporaria(DirectoryInfo di)");
            }
        }

        public List<string> IdentificarCapitulosExistentesBanco(List<CapitulosObra> listaCapitulosObra)
        {
            var capitulosIdentificados = db.BuscarCapituloObrasPorIds(listaCapitulosObra.ConvertAll(g => g.ObraId));

            if (capitulosIdentificados.Count == 0) return [];

            return capitulosIdentificados.ConvertAll(g => g.NumeroCapitulo.ToString());
        }

        public void CriarPastaControle()
        {
            if (!Directory.Exists(caminhoPastaTemporaria))
            {
                Directory.CreateDirectory(caminhoPastaTemporaria);
            }
        }
    }
}