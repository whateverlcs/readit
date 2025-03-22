namespace Readit.Data.Models;

public partial class Avaliaco
{
    public int AvaId { get; set; }

    public string AvaAvaliacao { get; set; } = null!;

    public virtual ICollection<AvaliacoesComentario> AvaliacoesComentarios { get; set; } = new List<AvaliacoesComentario>();
}