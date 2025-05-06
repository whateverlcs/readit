using Readit.Core.Domain;

namespace Readit.API.Communication.Responses
{
    public class ResponseConsultarPreferenciasUsuarioJson
    {
        public List<PreferenciasUsuario> PreferenciasUsuario { get; set; } = null!;
    }
}