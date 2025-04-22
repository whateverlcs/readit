namespace Readit.Core.Domain
{
    public class Obras
    {
        public int Id { get; set; }
        public string NomeObra { get; set; }
        public byte Status { get; set; }
        public byte Tipo { get; set; }
        public string? Descricao { get; set; }
        public DateTime? DataPublicacao { get; set; }
        public DateTime? DataAtualizacao { get; set; }
        public int UsuarioId { get; set; }
        public int ImagemId { get; set; }
    }
}