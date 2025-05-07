namespace Readit.API.Communication.Responses
{
    public class ResponseConsultarObrasSlideshowJson
    {
        public List<Slideshow> Slideshow { get; set; } = null!;
    }

    public class Slideshow
    {
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public byte[] Imagem { get; set; }
        public string[] Generos { get; set; }
        public string UltimoCapitulo { get; set; }
    }
}