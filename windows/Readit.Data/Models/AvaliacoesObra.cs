namespace Readit.Data.Models;

public partial class AvaliacoesObra
{
    public int AvoId { get; set; }

    public byte AvoNota { get; set; }

    public DateTime? AvoDataAvaliacao { get; set; }

    public DateTime? AvoDataAtualizacao { get; set; }

    public int UsuId { get; set; }

    public int ObsId { get; set; }

    public virtual Obra Obs { get; set; } = null!;

    public virtual Usuario Usu { get; set; } = null!;
}