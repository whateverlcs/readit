using Readit.Core.Domain;

namespace Readit.Core.Services
{
    public interface IComentarioService
    {
        Task<List<Comentarios>> FormatarDadosComentarios(int idObra, int? idCapitulo);
    }
}