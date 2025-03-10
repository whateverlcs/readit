using Readit.Core.Domain;

namespace Readit.Core.Repositories
{
    public interface ICapituloRepository
    {
        public Task<List<CapitulosObra>> BuscarCapituloObrasPorIdsAsync(List<int> numCapitulos, int idObra);

        public Task<(List<CapitulosObra>, CapitulosObra)> BuscarCapituloObrasPorIdAsync(int idObra, int chapterId, bool numeroCapitulos, bool paginasCapitulo);

        public Task<bool> CadastrarCapitulosAsync(List<CapitulosObra> listaCapitulosObra);
    }
}