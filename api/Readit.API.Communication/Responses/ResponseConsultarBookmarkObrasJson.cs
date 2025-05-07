namespace Readit.API.Communication.Responses
{
    public class ResponseConsultarBookmarkObrasJson
    {
        public List<Bookmarks> Bookmarks { get; set; } = null!;
    }

    public class Bookmarks
    {
        public int IdObra { get; set; }
        public byte[] Imagem { get; set; } = null!;
        public byte[] ImagemFlag { get; set; } = null!;
        public string Nome { get; set; } = string.Empty;
        public string NomeAbreviado { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
    }
}