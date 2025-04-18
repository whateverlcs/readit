﻿namespace Readit.Data.Models;

public partial class Usuario
{
    public int UsuId { get; set; }

    public string UsuNome { get; set; } = null!;

    public string UsuApelido { get; set; } = null!;

    public string UsuEmail { get; set; } = null!;

    public string UsuSenha { get; set; } = null!;

    public bool UsuAdministrador { get; set; }

    public int? ImgId { get; set; }

    public virtual ICollection<AvaliacoesComentario> AvaliacoesComentarios { get; set; } = new List<AvaliacoesComentario>();

    public virtual ICollection<AvaliacoesObra> AvaliacoesObras { get; set; } = new List<AvaliacoesObra>();

    public virtual ICollection<BookmarksUsuario> BookmarksUsuarios { get; set; } = new List<BookmarksUsuario>();

    public virtual ICollection<CapitulosObra> CapitulosObras { get; set; } = new List<CapitulosObra>();

    public virtual ICollection<Comentario> Comentarios { get; set; } = new List<Comentario>();

    public virtual Imagen? Img { get; set; }

    public virtual ICollection<Obra> Obras { get; set; } = new List<Obra>();

    public virtual ICollection<PreferenciasUsuario> PreferenciasUsuarios { get; set; } = new List<PreferenciasUsuario>();
}