using Readit.Core.Domain;

namespace Readit.Core.Repositories
{
    public interface IGeneroRepository
    {
        public Task<List<Generos>> BuscarGenerosPorNomeAsync(string nomeGenero);

        public Task<List<Generos>> BuscarGenerosPorObraAsync(int? idObra);

        public Task<bool> CadastrarGeneroAsync(Generos genero);
    }
}