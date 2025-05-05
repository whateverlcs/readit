using Readit.Core.Domain;

namespace Readit.API.Communication.Responses
{
    public class ResponseConsultarGenerosJson
    {
        public List<Generos> ListaGeneros { get; set; } = null!;
    }
}