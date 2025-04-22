namespace Readit.Core.Repositories
{
    public interface IAvaliacaoObraRepository
    {
        public Task<bool> AtualizarRatingAsync(int obraId, double rating);

        public Task<double> BuscarRatingObraAsync(int obraId);
    }
}