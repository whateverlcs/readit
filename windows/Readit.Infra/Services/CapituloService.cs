using Readit.Core.Domain;
using Readit.Core.Repositories;
using Readit.Core.Services;

namespace Readit.Infra.Services
{
    public class CapituloService : ICapituloService
    {
        private readonly IImagemService _imagemService;
        private readonly ICapituloRepository _capituloRepository;

        public CapituloService(IImagemService imagemService, ICapituloRepository capituloRepository)
        {
            _imagemService = imagemService;
            _capituloRepository = capituloRepository;
        }

        public (List<CapitulosObra>, CapitulosObra) FormatarDadosPaginasCapitulo((List<CapitulosObra>, CapitulosObra) capitulos)
        {
            foreach (var pg in capitulos.Item2.ListaPaginas)
            {
                pg.PaginaImage = _imagemService.ByteArrayToImage(pg.Pagina);
            }

            return capitulos;
        }

        public async Task<List<string>> IdentificarCapitulosExistentesBanco(List<CapitulosObra> listaCapitulosObra)
        {
            var capitulosIdentificados = await _capituloRepository.BuscarCapituloObrasPorIdsAsync(listaCapitulosObra.ConvertAll(g => g.NumeroCapitulo), listaCapitulosObra.First().ObraId);

            if (capitulosIdentificados.Count == 0) return [];

            return capitulosIdentificados.ConvertAll(g => g.NumeroCapitulo.ToString());
        }
    }
}