namespace Readit.API.Communication.Responses
{
    public class ResponseConsultarListagemObrasJson
    {
        public List<Listagem> Listagem { get; set; } = null!;
    }

    public class Listagem
    {
        public int IdObra { get; set; }
        public byte[] Imagem { get; set; } = null!;
        public byte[] ImagemFlag { get; set; } = null!;
        public string Nome { get; set; } = string.Empty;
        public string NomeAbreviado { get; set; } = string.Empty;
        public double Rating { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public List<string> Generos { get; set; } = null!;
        public DateTime DataPublicacao { get; set; }
        public DateTime? DataAtualizacao { get; set; }
    }
}