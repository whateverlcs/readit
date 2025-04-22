namespace Readit.Data.Models;

public partial class Comentario
{
    public int CtsId { get; set; }

    public string CtsComentario { get; set; } = null!;

    public DateTime? CtsData { get; set; }

    public DateTime? CtsDataAtualizacao { get; set; }

    public int ObsId { get; set; }

    public int UsuId { get; set; }

    public int? CpoId { get; set; }

    public virtual ICollection<AvaliacoesComentario> AvaliacoesComentarios { get; set; } = new List<AvaliacoesComentario>();

    public virtual CapitulosObra? Cpo { get; set; } = null!;

    public virtual Obra Obs { get; set; } = null!;

    public virtual ICollection<RespostasComentario> RespostasComentarioCts { get; set; } = new List<RespostasComentario>();

    public virtual ICollection<RespostasComentario> RespostasComentarioRes { get; set; } = new List<RespostasComentario>();

    public virtual Usuario Usu { get; set; } = null!;
}