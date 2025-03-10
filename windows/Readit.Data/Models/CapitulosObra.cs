namespace Readit.Data.Models;

public partial class CapitulosObra
{
    public int CpoId { get; set; }

    public int CpoNumeroCapitulo { get; set; }

    public DateTime? CpoDataPublicacao { get; set; }

    public DateTime? CpoDataAtualizacao { get; set; }

    public int UsuId { get; set; }

    public int ObsId { get; set; }

    public virtual ICollection<Comentario> Comentarios { get; set; } = new List<Comentario>();

    public virtual Obra Obs { get; set; } = null!;

    public virtual ICollection<PaginasCapitulo> PaginasCapitulos { get; set; } = new List<PaginasCapitulo>();

    public virtual Usuario Usu { get; set; } = null!;
}