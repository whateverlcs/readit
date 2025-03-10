namespace Readit.Data.Models;

public partial class TipoVisualizacaoObra
{
    public int TvoId { get; set; }

    public byte TvoVisualizacao { get; set; }

    public virtual ICollection<TipoVisualizacaoObraUsuario> TipoVisualizacaoObraUsuarios { get; set; } = new List<TipoVisualizacaoObraUsuario>();
}