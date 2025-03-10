using Readit.Core.Domain;

namespace Readit.Core.Repositories
{
    public interface ITipoVisualizacaoObraRepository
    {
        public Task<List<TipoVisualizacaoObra>> BuscarTiposVisualizacaoObraPorIdAsync(int idTipoVisualizacao);

        public Task<List<TipoVisualizacaoObraUsuario>> BuscarTiposVisualizacaoObraUsuarioPorIdAsync(int idUsuario);
    }
}