using System;
using System.Collections.Generic;

namespace EntityFramework_DB.Models;

public partial class VisualizacoesObra
{
    public int VsoId { get; set; }

    public int VsoViews { get; set; }

    public int ObsId { get; set; }

    public virtual Obra Obs { get; set; } = null!;
}
