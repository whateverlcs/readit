using Readit.Core.Domain;

namespace Readit.Core.Repositories
{
    public interface IPaginaCapituloRepository
    {
        public Task<List<PaginasCapitulo>> BuscarPaginasCapituloPorIdsAsync(List<int> idsCap);
    }
}