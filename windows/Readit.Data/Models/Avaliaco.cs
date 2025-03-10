namespace Readit.Data.Models;

public partial class Avaliaco
{
    public int AvaId { get; set; }

    public byte AvaAvaliacao { get; set; }

    public int UsuId { get; set; }

    public virtual ICollection<AvaliacoesComentario> AvaliacoesComentarios { get; set; } = new List<AvaliacoesComentario>();

    public virtual Usuario Usu { get; set; } = null!;
}