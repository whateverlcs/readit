namespace Readit.Data.Models;

public partial class Obra
{
    public int ObsId { get; set; }

    public string ObsNomeObra { get; set; } = null!;

    public byte ObsStatus { get; set; }

    public byte ObsTipo { get; set; }

    public string? ObsDescricao { get; set; }

    public DateTime? ObsDataPublicacao { get; set; }

    public DateTime? ObsDataAtualizacao { get; set; }

    public int UsuId { get; set; }

    public int ImgId { get; set; }

    public virtual ICollection<AvaliacoesObra> AvaliacoesObras { get; set; } = new List<AvaliacoesObra>();

    public virtual ICollection<BookmarksUsuario> BookmarksUsuarios { get; set; } = new List<BookmarksUsuario>();

    public virtual ICollection<CapitulosObra> CapitulosObras { get; set; } = new List<CapitulosObra>();

    public virtual ICollection<Comentario> Comentarios { get; set; } = new List<Comentario>();

    public virtual Imagen Img { get; set; } = null!;

    public virtual ICollection<ObrasGenero> ObrasGeneros { get; set; } = new List<ObrasGenero>();

    public virtual Usuario Usu { get; set; } = null!;

    public virtual ICollection<VisualizacoesObra> VisualizacoesObras { get; set; } = new List<VisualizacoesObra>();
}