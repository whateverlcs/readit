namespace Readit.Core.Domain
{
    public class PaginasCapitulo
    {
        public int Id { get; set; }
        public int NumeroPagina { get; set; }
        public byte[] Pagina { get; set; }
        public string Tamanho { get; set; }
        public int CapituloId { get; set; }
    }
}