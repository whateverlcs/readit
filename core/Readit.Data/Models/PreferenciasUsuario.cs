namespace Readit.Data.Models;

public partial class PreferenciasUsuario
{
    public int PfuId { get; set; }

    public int UsuId { get; set; }

    public int PreId { get; set; }

    public virtual Preferencia Pre { get; set; } = null!;

    public virtual Usuario Usu { get; set; } = null!;
}