namespace Readit.Data.Models;

public partial class RespostasComentario
{
    public int RscId { get; set; }

    public int CtsId { get; set; }

    public int ResId { get; set; }

    public virtual Comentario Cts { get; set; } = null!;

    public virtual Comentario Res { get; set; } = null!;
}