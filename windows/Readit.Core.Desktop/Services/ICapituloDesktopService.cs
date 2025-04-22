using Readit.Core.Desktop.Domain;
using Readit.Core.Domain;

namespace Readit.Core.Desktop.Services
{
    public interface ICapituloDesktopService
    {
        (List<CapitulosObraDesktop>, CapitulosObraDesktop) FormatarDadosPaginasCapitulo((List<CapitulosObra>, CapitulosObra) capitulos);
    }
}