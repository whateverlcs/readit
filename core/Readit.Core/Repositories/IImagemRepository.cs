using Readit.Core.Domain;

namespace Readit.Core.Repositories
{
    public interface IImagemRepository
    {
        public Task<List<Imagens>> BuscarImagemPorIdAsync(int idImagem);
    }
}