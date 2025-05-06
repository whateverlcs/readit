using Readit.Core.Domain;

namespace Readit.API.Communication.Responses
{
    public class ResponseConsultarPaginasCapituloPorCapituloJson
    {
        public List<PaginasCapitulo> ListaPaginasCapitulo { get; set; } = null!;
    }
}