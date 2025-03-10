namespace Readit.Data.Models;

public partial class ImagensComentario
{
    public int IctId { get; set; }

    public byte[] IctImagem { get; set; } = null!;

    public string IctFormato { get; set; } = null!;

    public DateTime? IctDataInclusao { get; set; }

    public DateTime? IctDataAtualizacao { get; set; }

    public int CtsId { get; set; }

    public virtual Comentario Cts { get; set; } = null!;
}