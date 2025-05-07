using Readit.Core.Domain;

namespace Readit.API.Communication.Responses
{
    public class ResponseConsultarDetalhesObraJson
    {
        public int IdObra { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public string DataPublicacao { get; set; } = string.Empty;
        public string DataAtualizacao { get; set; } = string.Empty;
        public string PostadoPor { get; set; } = string.Empty;
        public double Rating { get; set; }
        public int Views { get; set; }
        public byte[] Imagem { get; set; } = null!;
        public byte[] ImagemFlag { get; set; } = null!;
        public string[] Generos { get; set; } = null!;
        public bool Bookmark { get; set; }
        public double RatingUsuario { get; set; }
        public List<ChapterInfo> Capitulos { get; set; } = null!;
    }
}