namespace Readit.API.Communication.Responses
{
    public class ResponseConsultarDadosObraJson
    {
        public int IdObra { get; set; }
        public string Nome { get; set; } = string.Empty;
        public byte[] Imagem { get; set; } = null!;
        public string Status { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public List<string> Generos { get; set; } = null!;
        public string Descricao { get; set; } = string.Empty;
    }
}