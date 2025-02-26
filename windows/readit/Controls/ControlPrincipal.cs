using Newtonsoft.Json;
using readit.Data;
using readit.Enums;
using readit.Models;
using SharpCompress.Archives;
using SharpCompress.Common;
using System.IO;
using System.Net.Mail;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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

        public BitmapImage ByteArrayToImage(byte[] imageBytes)
        {
            using (var stream = new MemoryStream(imageBytes))
            {
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = stream;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
                return bitmapImage;
            }
        }

        public byte[] ConvertBitmapImageToByteArray(ImageSource imageSource)
        {
            if (imageSource is BitmapSource bitmapSource)
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    PngBitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                    encoder.Save(memoryStream);
                    return memoryStream.ToArray();
                }
            }

            return [];
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
            var capitulosIdentificados = db.BuscarCapituloObrasPorIds(listaCapitulosObra.ConvertAll(g => g.NumeroCapitulo), listaCapitulosObra.First().ObraId);

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

        public List<PostagensObras> FormatarDadosUltimasAtualizacoes(List<PostagensObras> postagens)
        {
            foreach (var postagem in postagens)
            {
                postagem.Status = ObterStatus(postagem.StatusNumber);
                postagem.Image = ByteArrayToImage(postagem.ImageByte);

                foreach (var capInfo in postagem.ChapterInfos)
                {
                    capInfo.TimeAgo = FormatarData(capInfo.TimeAgoDate);
                }
            }

            return postagens;
        }

        public List<PostagensObras> FormatarDadosBookmarks(List<PostagensObras> postagens)
        {
            foreach (var postagem in postagens)
            {
                postagem.Image = ByteArrayToImage(postagem.ImageByte);
                postagem.Title = postagem.Title = postagem.Title.Length > 39 ? postagem.Title.Substring(0, 39).Trim() + "..." : postagem.Title.Trim();
            }

            return postagens;
        }

        public List<PostagensObras> FormatarDadosListagemObras(List<PostagensObras> postagens)
        {
            foreach (var postagem in postagens)
            {
                postagem.Image = ByteArrayToImage(postagem.ImageByte);
                postagem.Title = postagem.Title = postagem.Title.Length > 39 ? postagem.Title.Substring(0, 39).Trim() + "..." : postagem.Title.Trim();
                postagem.Rating = Math.Round(postagem.Rating, 1);
                postagem.Status = ObterStatus(postagem.StatusNumber);
                postagem.Tipo = ObterTipo(postagem.TipoNumber);
            }

            return postagens;
        }

        public List<DestaquesItem> FormatarDadosObrasEmDestaques()
        {
            var obrasEmDestaque = db.BuscarObrasEmDestaque();

            foreach (var obra in obrasEmDestaque)
            {
                obra.Image = ByteArrayToImage(obra.ImageByte);
                obra.Rating = Math.Round(obra.Rating, 1);
            }

            return obrasEmDestaque;
        }

        public DetalhesObra FormatarDadosDetalhamentoObra(string nomeObra)
        {
            var detalhesObra = db.BuscarDetalhesObra(nomeObra);

            detalhesObra.Image = ByteArrayToImage(detalhesObra.ImageByte);
            detalhesObra.Rating = Math.Round(detalhesObra.Rating, 1);
            detalhesObra.Status = ObterStatus(detalhesObra.StatusNumber);
            detalhesObra.Type = ObterTipo(detalhesObra.TypeNumber);
            detalhesObra.Description = detalhesObra.Description.Length > 437 ? detalhesObra.Description.Substring(0, 437).Trim() + "..." : detalhesObra.Description.Trim();
            detalhesObra.PostedBy = detalhesObra.PostedBy.Length > 16 ? detalhesObra.PostedBy.Substring(0, 16).Trim() + "..." : detalhesObra.PostedBy.Trim();

            return detalhesObra;
        }

        public string ObterStatus(int status)
        {
            return status switch
            {
                (int)EnumObra.StatusObra.EmAndamento => "Em Andamento",
                (int)EnumObra.StatusObra.EmHiato => "Em Hiato",
                (int)EnumObra.StatusObra.Finalizado => "Finalizado",
                (int)EnumObra.StatusObra.Cancelado => "Cancelado",
                (int)EnumObra.StatusObra.Dropado => "Dropado",
                _ => "Desconhecido"
            };
        }

        public string ObterTipo(int tipo)
        {
            return tipo switch
            {
                (int)EnumObra.TipoObra.Manhwa => "Manhwa",
                (int)EnumObra.TipoObra.Donghua => "Donghua",
                (int)EnumObra.TipoObra.Manga => "Manga",
                _ => "Desconhecido"
            };
        }

        public string FormatarData(DateTime? data)
        {
            if (data == null) return "Desconhecido";
            var diferenca = DateTime.Now - data.Value;

            if (diferenca.TotalMinutes < 60)
                return $"{(int)diferenca.TotalMinutes} min atrás";
            if (diferenca.TotalHours < 24)
                return $"{(int)diferenca.TotalHours}h atrás";
            if (diferenca.TotalDays < 7)
                return $"{(int)diferenca.TotalDays} dias atrás";

            return data.Value.ToString("dd/MM/yyyy");
        }

        public List<string> ExtrairDadosFrasesLoading()
        {
            string jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Files", "phrases-loading.json");

            string jsonContent = File.ReadAllText(jsonFilePath);

            List<string> curiosidades = JsonConvert.DeserializeObject<List<string>>(jsonContent);

            Shuffle(curiosidades);

            return curiosidades;
        }

        public void Shuffle<T>(List<T> list)
        {
            Random rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}