namespace Readit.Core.Repositories
{
    public interface IVisualizacaoObraRepository
    {
        public Task<bool> AtualizarViewObraAsync(string nomeObra);
    }
}