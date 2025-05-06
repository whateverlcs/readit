using Readit.Core.Domain;

namespace Readit.API.Communication.Responses
{
    public class ResponseConsultarPreferenciasJson
    {
        public List<Preferencias> Preferencias { get; set; } = null!;
    }
}