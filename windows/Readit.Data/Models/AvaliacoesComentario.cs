namespace Readit.Data.Models;

public partial class AvaliacoesComentario
{
    public int AvcId { get; set; }

    public int CtsId { get; set; }

    public int AvaId { get; set; }

    public virtual Avaliaco Ava { get; set; } = null!;

    public virtual Comentario Cts { get; set; } = null!;
}