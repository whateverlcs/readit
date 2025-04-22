namespace Readit.Core.Domain
{
    public class Imagens
    {
        public int Id { get; set; }
        public byte[] Imagem { get; set; }
        public string Formato { get; set; }
        public DateTime? DataInclusao { get; set; }
        public DateTime? DataAtualizacao { get; set; }
        public byte Tipo { get; set; }
    }
}