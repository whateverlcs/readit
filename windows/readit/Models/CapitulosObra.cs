namespace readit.Models
{
    public class CapitulosObra
    {
        public int Id { get; set; }
        public string NomeObra { get; set; }
        public string NumeroCapituloDisplay { get; set; }
        public int NumeroCapitulo { get; set; }
        public DateTime? DataPublicacao { get; set; }
        public DateTime? DataAtualizacao { get; set; }
        public int UsuarioId { get; set; }
        public int ObraId { get; set; }
        public List<PaginasCapitulo> ListaPaginas { get; set; } = new List<PaginasCapitulo>();
    }
}