﻿namespace Readit.Data.Models;

public partial class BookmarksUsuario
{
    public int BkuId { get; set; }

    public int UsuId { get; set; }

    public int ObsId { get; set; }

    public virtual Obra Obs { get; set; } = null!;

    public virtual Usuario Usu { get; set; } = null!;
}