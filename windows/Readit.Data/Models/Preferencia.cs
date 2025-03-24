namespace Readit.Data.Models;

public partial class Preferencia
{
    public int PreId { get; set; }

    public string PrePreferencia { get; set; } = null!;

    public virtual ICollection<PreferenciasUsuario> PreferenciasUsuarios { get; set; } = new List<PreferenciasUsuario>();
}