namespace Readit.Data.Models;

public partial class ObrasGenero
{
    public int OgsId { get; set; }

    public int ObsId { get; set; }

    public int GnsId { get; set; }

    public virtual Genero Gns { get; set; } = null!;

    public virtual Obra Obs { get; set; } = null!;
}