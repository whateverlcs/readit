namespace Readit.Data.Models;

public partial class Genero
{
    public int GnsId { get; set; }

    public string GnsNome { get; set; } = null!;

    public virtual ICollection<ObrasGenero> ObrasGeneros { get; set; } = new List<ObrasGenero>();
}