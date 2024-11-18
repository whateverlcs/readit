using System;
using System.Collections.Generic;

namespace EntityFramework_DB.Models;

public partial class PaginasCapitulo
{
    public int PgcId { get; set; }

    public int PgcNumeroPagina { get; set; }

    public byte[] PgcPagina { get; set; } = null!;

    public string PgcTamanho { get; set; } = null!;

    public int CpoId { get; set; }

    public virtual CapitulosObra Cpo { get; set; } = null!;
}
