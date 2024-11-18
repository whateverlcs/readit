using System;
using System.Collections.Generic;

namespace EntityFramework_DB.Models;

public partial class Preferencia
{
    public int PreId { get; set; }

    public bool PreExibirConteudoAdulto { get; set; }

    public virtual ICollection<PreferenciasUsuario> PreferenciasUsuarios { get; set; } = new List<PreferenciasUsuario>();
}
