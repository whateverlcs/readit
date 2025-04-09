using Microsoft.EntityFrameworkCore;
using Readit.Data.Models;

namespace Readit.Data.Context;

public partial class ReaditContext : DbContext
{
    public ReaditContext()
    {
    }

    public ReaditContext(DbContextOptions<ReaditContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Avaliaco> Avaliacoes { get; set; }

    public virtual DbSet<AvaliacoesComentario> AvaliacoesComentarios { get; set; }

    public virtual DbSet<AvaliacoesObra> AvaliacoesObras { get; set; }

    public virtual DbSet<BookmarksUsuario> BookmarksUsuarios { get; set; }

    public virtual DbSet<CapitulosObra> CapitulosObras { get; set; }

    public virtual DbSet<Comentario> Comentarios { get; set; }

    public virtual DbSet<Genero> Generos { get; set; }

    public virtual DbSet<Imagen> Imagens { get; set; }

    public virtual DbSet<Obra> Obras { get; set; }

    public virtual DbSet<ObrasGenero> ObrasGeneros { get; set; }

    public virtual DbSet<PaginasCapitulo> PaginasCapitulos { get; set; }

    public virtual DbSet<Preferencia> Preferencias { get; set; }

    public virtual DbSet<PreferenciasUsuario> PreferenciasUsuarios { get; set; }

    public virtual DbSet<RespostasComentario> RespostasComentarios { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public virtual DbSet<VisualizacoesObra> VisualizacoesObras { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            throw new InvalidOperationException("O DbContext não foi configurado. Certifique-se de usar IDbContextFactory ou injeção de dependência.");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Avaliaco>(entity =>
        {
            entity.HasKey(e => e.AvaId).HasName("PK__Avaliaco__4C740ADA34E84EF4");

            entity.Property(e => e.AvaId).HasColumnName("ava_id");
            entity.Property(e => e.AvaAvaliacao)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("ava_avaliacao");
        });

        modelBuilder.Entity<AvaliacoesComentario>(entity =>
        {
            entity.HasKey(e => e.AvcId).HasName("PK__Avaliaco__D501B6B12F2C8E2F");

            entity.ToTable("AvaliacoesComentario");

            entity.Property(e => e.AvcId).HasColumnName("avc_id");
            entity.Property(e => e.AvaId).HasColumnName("ava_id");
            entity.Property(e => e.CtsId).HasColumnName("cts_id");
            entity.Property(e => e.UsuId).HasColumnName("usu_id");

            entity.HasOne(d => d.Ava).WithMany(p => p.AvaliacoesComentarios)
                .HasForeignKey(d => d.AvaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AvaliacoesComentarioAvaliacoes");

            entity.HasOne(d => d.Cts).WithMany(p => p.AvaliacoesComentarios)
                .HasForeignKey(d => d.CtsId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AvaliacoesComentarioComentarios");

            entity.HasOne(d => d.Usu).WithMany(p => p.AvaliacoesComentarios)
                .HasForeignKey(d => d.UsuId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AvaliacoesComentarioUsuarios");
        });

        modelBuilder.Entity<AvaliacoesObra>(entity =>
        {
            entity.HasKey(e => e.AvoId).HasName("PK__Avaliaco__47184C0EE7915E52");

            entity.ToTable("AvaliacoesObra");

            entity.Property(e => e.AvoId).HasColumnName("avo_id");
            entity.Property(e => e.AvoDataAtualizacao)
                .HasColumnType("datetime")
                .HasColumnName("avo_dataAtualizacao");
            entity.Property(e => e.AvoDataAvaliacao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("avo_dataAvaliacao");
            entity.Property(e => e.AvoNota).HasColumnName("avo_nota");
            entity.Property(e => e.ObsId).HasColumnName("obs_id");
            entity.Property(e => e.UsuId).HasColumnName("usu_id");

            entity.HasOne(d => d.Obs).WithMany(p => p.AvaliacoesObras)
                .HasForeignKey(d => d.ObsId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AvaliacoesObraObras");

            entity.HasOne(d => d.Usu).WithMany(p => p.AvaliacoesObras)
                .HasForeignKey(d => d.UsuId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AvaliacoesObraUsuarios");
        });

        modelBuilder.Entity<BookmarksUsuario>(entity =>
        {
            entity.HasKey(e => e.BkuId).HasName("PK__Bookmark__BB56EDAAECF8364F");

            entity.ToTable("BookmarksUsuario");

            entity.Property(e => e.BkuId).HasColumnName("bku_id");
            entity.Property(e => e.ObsId).HasColumnName("obs_id");
            entity.Property(e => e.UsuId).HasColumnName("usu_id");

            entity.HasOne(d => d.Obs).WithMany(p => p.BookmarksUsuarios)
                .HasForeignKey(d => d.ObsId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BookmarksUsuarioObras");

            entity.HasOne(d => d.Usu).WithMany(p => p.BookmarksUsuarios)
                .HasForeignKey(d => d.UsuId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BookmarksUsuarioUsuarios");
        });

        modelBuilder.Entity<CapitulosObra>(entity =>
        {
            entity.HasKey(e => e.CpoId).HasName("PK__Capitulo__715F258F2592C897");

            entity.ToTable("CapitulosObra");

            entity.Property(e => e.CpoId).HasColumnName("cpo_id");
            entity.Property(e => e.CpoDataAtualizacao)
                .HasColumnType("datetime")
                .HasColumnName("cpo_dataAtualizacao");
            entity.Property(e => e.CpoDataPublicacao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("cpo_dataPublicacao");
            entity.Property(e => e.CpoNumeroCapitulo).HasColumnName("cpo_numeroCapitulo");
            entity.Property(e => e.ObsId).HasColumnName("obs_id");
            entity.Property(e => e.UsuId).HasColumnName("usu_id");

            entity.HasOne(d => d.Obs).WithMany(p => p.CapitulosObras)
                .HasForeignKey(d => d.ObsId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CapitulosObraObras");

            entity.HasOne(d => d.Usu).WithMany(p => p.CapitulosObras)
                .HasForeignKey(d => d.UsuId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CapitulosObraUsuarios");
        });

        modelBuilder.Entity<Comentario>(entity =>
        {
            entity.HasKey(e => e.CtsId).HasName("PK__Comentar__D82B520C0088362B");

            entity.Property(e => e.CtsId).HasColumnName("cts_id");
            entity.Property(e => e.CpoId).HasColumnName("cpo_id");
            entity.Property(e => e.CtsComentario)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("cts_comentario");
            entity.Property(e => e.CtsData)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("cts_data");
            entity.Property(e => e.CtsDataAtualizacao)
                .HasColumnType("datetime")
                .HasColumnName("cts_dataAtualizacao");
            entity.Property(e => e.ObsId).HasColumnName("obs_id");
            entity.Property(e => e.UsuId).HasColumnName("usu_id");

            entity.HasOne(d => d.Cpo).WithMany(p => p.Comentarios)
                .HasForeignKey(d => d.CpoId)
                .HasConstraintName("FK_ComentariosCapitulosObra");

            entity.HasOne(d => d.Obs).WithMany(p => p.Comentarios)
                .HasForeignKey(d => d.ObsId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ComentariosObras");

            entity.HasOne(d => d.Usu).WithMany(p => p.Comentarios)
                .HasForeignKey(d => d.UsuId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ComentariosUsuarios");
        });

        modelBuilder.Entity<Genero>(entity =>
        {
            entity.HasKey(e => e.GnsId).HasName("PK__Generos__7C55DF659F760985");

            entity.Property(e => e.GnsId).HasColumnName("gns_id");
            entity.Property(e => e.GnsNome)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("gns_nome");
        });

        modelBuilder.Entity<Imagen>(entity =>
        {
            entity.HasKey(e => e.ImgId).HasName("PK__Imagens__6F16A71C1F8CD933");

            entity.Property(e => e.ImgId).HasColumnName("img_id");
            entity.Property(e => e.ImgDataAtualizacao)
                .HasColumnType("datetime")
                .HasColumnName("img_dataAtualizacao");
            entity.Property(e => e.ImgDataInclusao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("img_dataInclusao");
            entity.Property(e => e.ImgFormato)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("img_formato");
            entity.Property(e => e.ImgImagem).HasColumnName("img_imagem");
            entity.Property(e => e.ImgTipo).HasColumnName("img_tipo");
        });

        modelBuilder.Entity<Obra>(entity =>
        {
            entity.HasKey(e => e.ObsId).HasName("PK__Obras__4DFADA1AD1450799");

            entity.Property(e => e.ObsId).HasColumnName("obs_id");
            entity.Property(e => e.ImgId).HasColumnName("img_id");
            entity.Property(e => e.ObsDataAtualizacao)
                .HasColumnType("datetime")
                .HasColumnName("obs_dataAtualizacao");
            entity.Property(e => e.ObsDataPublicacao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("obs_dataPublicacao");
            entity.Property(e => e.ObsDescricao)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("obs_descricao");
            entity.Property(e => e.ObsNomeObra)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("obs_nomeObra");
            entity.Property(e => e.ObsStatus).HasColumnName("obs_status");
            entity.Property(e => e.ObsTipo).HasColumnName("obs_tipo");
            entity.Property(e => e.UsuId).HasColumnName("usu_id");

            entity.HasOne(d => d.Img).WithMany(p => p.Obras)
                .HasForeignKey(d => d.ImgId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ObrasImagens");

            entity.HasOne(d => d.Usu).WithMany(p => p.Obras)
                .HasForeignKey(d => d.UsuId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ObrasUsuarios");
        });

        modelBuilder.Entity<ObrasGenero>(entity =>
        {
            entity.HasKey(e => e.OgsId).HasName("PK__ObrasGen__4BEE63812A8AF57E");

            entity.Property(e => e.OgsId).HasColumnName("ogs_id");
            entity.Property(e => e.GnsId).HasColumnName("gns_id");
            entity.Property(e => e.ObsId).HasColumnName("obs_id");

            entity.HasOne(d => d.Gns).WithMany(p => p.ObrasGeneros)
                .HasForeignKey(d => d.GnsId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ObrasGenerosGeneros");

            entity.HasOne(d => d.Obs).WithMany(p => p.ObrasGeneros)
                .HasForeignKey(d => d.ObsId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ObrasGenerosObras");
        });

        modelBuilder.Entity<PaginasCapitulo>(entity =>
        {
            entity.HasKey(e => e.PgcId).HasName("PK__PaginasC__7C4DBAEC3214A0CF");

            entity.ToTable("PaginasCapitulo");

            entity.Property(e => e.PgcId).HasColumnName("pgc_id");
            entity.Property(e => e.CpoId).HasColumnName("cpo_id");
            entity.Property(e => e.PgcNumeroPagina).HasColumnName("pgc_numeroPagina");
            entity.Property(e => e.PgcPagina).HasColumnName("pgc_pagina");
            entity.Property(e => e.PgcTamanho)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("pgc_tamanho");

            entity.HasOne(d => d.Cpo).WithMany(p => p.PaginasCapitulos)
                .HasForeignKey(d => d.CpoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PaginasCapituloCapitulosObra");
        });

        modelBuilder.Entity<Preferencia>(entity =>
        {
            entity.HasKey(e => e.PreId).HasName("PK__Preferen__E0CCC60D9B3C53CE");

            entity.Property(e => e.PreId).HasColumnName("pre_id");
            entity.Property(e => e.PrePreferencia)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("pre_preferencia");
        });

        modelBuilder.Entity<PreferenciasUsuario>(entity =>
        {
            entity.HasKey(e => e.PfuId).HasName("PK__Preferen__F758117A53061600");

            entity.ToTable("PreferenciasUsuario");

            entity.Property(e => e.PfuId).HasColumnName("pfu_id");
            entity.Property(e => e.PreId).HasColumnName("pre_id");
            entity.Property(e => e.UsuId).HasColumnName("usu_id");

            entity.HasOne(d => d.Pre).WithMany(p => p.PreferenciasUsuarios)
                .HasForeignKey(d => d.PreId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PreferenciasUsuarioPreferencias");

            entity.HasOne(d => d.Usu).WithMany(p => p.PreferenciasUsuarios)
                .HasForeignKey(d => d.UsuId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PreferenciasUsuarioUsuarios");
        });

        modelBuilder.Entity<RespostasComentario>(entity =>
        {
            entity.HasKey(e => e.RscId).HasName("PK__Resposta__2997E60DFFF5240F");

            entity.ToTable("RespostasComentario");

            entity.Property(e => e.RscId).HasColumnName("rsc_id");
            entity.Property(e => e.CtsId).HasColumnName("cts_id");
            entity.Property(e => e.ResId).HasColumnName("res_id");

            entity.HasOne(d => d.Cts).WithMany(p => p.RespostasComentarioCts)
                .HasForeignKey(d => d.CtsId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RespostasComentarioComentarios");

            entity.HasOne(d => d.Res).WithMany(p => p.RespostasComentarioRes)
                .HasForeignKey(d => d.ResId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RespostasComentarioResposta");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.UsuId).HasName("PK__Usuarios__430A673C06283B43");

            entity.Property(e => e.UsuId).HasColumnName("usu_id");
            entity.Property(e => e.ImgId).HasColumnName("img_id");
            entity.Property(e => e.UsuAdministrador).HasColumnName("usu_administrador");
            entity.Property(e => e.UsuApelido)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("usu_apelido");
            entity.Property(e => e.UsuEmail)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("usu_email");
            entity.Property(e => e.UsuNome)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("usu_nome");
            entity.Property(e => e.UsuSenha)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("usu_senha");

            entity.HasOne(d => d.Img).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.ImgId)
                .HasConstraintName("FK_UsuarioImagens");
        });

        modelBuilder.Entity<VisualizacoesObra>(entity =>
        {
            entity.HasKey(e => e.VsoId).HasName("PK__Visualiz__4B9D2D2EE87C31BD");

            entity.ToTable("VisualizacoesObra");

            entity.Property(e => e.VsoId).HasColumnName("vso_id");
            entity.Property(e => e.ObsId).HasColumnName("obs_id");
            entity.Property(e => e.VsoViews).HasColumnName("vso_views");

            entity.HasOne(d => d.Obs).WithMany(p => p.VisualizacoesObras)
                .HasForeignKey(d => d.ObsId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_VisualizacoesObraObras");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}