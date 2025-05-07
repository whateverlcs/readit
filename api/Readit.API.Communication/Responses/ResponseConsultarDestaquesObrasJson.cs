namespace Readit.API.Communication.Responses
{
    public class ResponseConsultarDestaquesObrasJson
    {
        public List<Destaques> Destaques { get; set; } = null!;
    }

    public class Destaques
    {
        public int Rank { get; set; }
        public string Nome { get; set; } = string.Empty;
        public byte[] Imagem { get; set; } = null!;
        public byte[] ImagemFlag { get; set; } = null!;
        public List<string> Generos { get; set; } = null!;
        public string Filter { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public double Rating { get; set; }
    }
}