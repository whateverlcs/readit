using Newtonsoft.Json;
using Readit.Core.Domain;
using Readit.Core.Services;
using Readit.Infra.Logging;
using SharpCompress.Archives;
using SharpCompress.Common;

namespace Readit.Infra.Services
{
    public class ArquivoService : IArquivoService
    {
        private readonly ILoggingService _logger;
        private readonly IUsuarioService _usuarioService;
        private readonly IUtilService _utilService;
        private readonly IConfiguracaoService _configuracaoService;

        public ArquivoService(ILoggingService logger, IUsuarioService usuarioService, IUtilService utilService, IConfiguracaoService configuracaoService)
        {
            _logger = logger;
            _usuarioService = usuarioService;
            _utilService = utilService;
            _configuracaoService = configuracaoService;
        }

        public void CriarPastaControle()
        {
            string caminhoPastaTemporaria = _configuracaoService.GetTempPath();

            if (!Directory.Exists(caminhoPastaTemporaria))
                Directory.CreateDirectory(caminhoPastaTemporaria);
        }

        public List<CapitulosObra> IdentificarArquivosInseridos(int obraId)
        {
            try
            {
                string caminhoPastaTemporaria = _configuracaoService.GetTempPath();
                DirectoryInfo di = new DirectoryInfo(caminhoPastaTemporaria);
                List<CapitulosObra> listaCapitulosObra = new List<CapitulosObra>();

                foreach (var capitulo in _usuarioService.ListaCapitulosSelecionados)
                {
                    DeletarArquivosPastaTemporaria(di);
                    di.Refresh();

                    CapitulosObra cap = new CapitulosObra
                    {
                        NumeroCapitulo = Convert.ToInt32(Path.GetFileNameWithoutExtension(capitulo)),
                        ObraId = obraId,
                        UsuarioId = _usuarioService.UsuarioLogado.Id
                    };

                    ArchiveFactory.WriteToDirectory(capitulo, caminhoPastaTemporaria, new ExtractionOptions { ExtractFullPath = true, Overwrite = true });

                    foreach (var paginas in di.GetFiles())
                    {
                        cap.ListaPaginas.Add(new PaginasCapitulo
                        {
                            NumeroPagina = Convert.ToInt32(Path.GetFileNameWithoutExtension(paginas.FullName)),
                            Pagina = File.ReadAllBytes(paginas.FullName),
                            Tamanho = $"{paginas.Length / 1024.0} KB"
                        });
                    }

                    listaCapitulosObra.Add(cap);
                }

                return listaCapitulosObra;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Erro ao identificar arquivos inseridos");
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
                _logger.LogError(e, "Erro ao deletar arquivos da pasta temporária");
            }
        }

        public List<string> ExtrairDadosFrasesLoading()
        {
            string jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Resources/Data Files", "phrases-loading.json");

            string jsonContent = File.ReadAllText(jsonFilePath);

            List<string> curiosidades = JsonConvert.DeserializeObject<List<string>>(jsonContent);

            _utilService.Shuffle(curiosidades);

            return curiosidades;
        }
    }
}