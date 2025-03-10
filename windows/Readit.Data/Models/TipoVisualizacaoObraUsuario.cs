namespace Readit.Data.Models;

public partial class TipoVisualizacaoObraUsuario
{
    public int TvouId { get; set; }

    public int UsuId { get; set; }

    public int TvoId { get; set; }

    public virtual TipoVisualizacaoObra Tvo { get; set; } = null!;

    public virtual Usuario Usu { get; set; } = null!;
}