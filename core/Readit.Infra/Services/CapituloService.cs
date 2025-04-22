using Readit.Core.Domain;
using Readit.Core.Repositories;
using Readit.Core.Services;

namespace Readit.Infra.Services
{
    public class CapituloService : ICapituloService
    {
        private readonly ICapituloRepository _capituloRepository;

        public CapituloService(ICapituloRepository capituloRepository)
        {
            _capituloRepository = capituloRepository;
        }

        public async Task<List<string>> IdentificarCapitulosExistentesBanco(List<CapitulosObra> listaCapitulosObra)
        {
            var capitulosIdentificados = await _capituloRepository.BuscarCapituloObrasPorIdsAsync(listaCapitulosObra.ConvertAll(g => g.NumeroCapitulo), listaCapitulosObra.First().ObraId);

            if (capitulosIdentificados.Count == 0) return [];

            return capitulosIdentificados.ConvertAll(g => g.NumeroCapitulo.ToString());
        }
    }
}