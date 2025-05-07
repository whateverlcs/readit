using Readit.Core.Domain;

namespace Readit.API.Communication.Requests
{
    public class RequestGerenciarObrasJson
    {
        public Obras Obra { get; set; }
        public Imagens Imagem { get; set; }
        public List<Generos> Generos { get; set; }
    }
}