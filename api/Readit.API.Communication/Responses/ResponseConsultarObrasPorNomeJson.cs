using Readit.Core.Domain;

namespace Readit.API.Communication.Responses
{
    public class ResponseConsultarObrasPorNomeJson
    {
        public List<Obras> Obras { get; set; } = null!;
    }
}