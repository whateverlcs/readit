namespace Readit.Data.Models;

public partial class Imagen
{
    public int ImgId { get; set; }

    public byte[] ImgImagem { get; set; } = null!;

    public string ImgFormato { get; set; } = null!;

    public DateTime? ImgDataInclusao { get; set; }

    public DateTime? ImgDataAtualizacao { get; set; }

    public byte ImgTipo { get; set; }

    public virtual ICollection<Obra> Obras { get; set; } = new List<Obra>();

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}