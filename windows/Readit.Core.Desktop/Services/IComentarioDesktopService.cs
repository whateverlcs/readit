using Readit.Core.Desktop.Domain;

namespace Readit.Core.Desktop.Services
{
    public interface IComentarioDesktopService
    {
        Task<List<ComentariosDesktop>> FormatarDadosComentarios(int idObra, int? idCapitulo);
    }
}