using Readit.Core.Domain;

namespace Readit.Core.Desktop.Domain
{
    public class CapitulosObraDesktop : CapitulosObra
    {
        public List<PaginasCapituloDesktop> ListaPaginasDesktop { get; set; } = new List<PaginasCapituloDesktop>();
    }
}