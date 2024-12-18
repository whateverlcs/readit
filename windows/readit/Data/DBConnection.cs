using EntityFramework_DB.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using readit.Controls;
using readit.Models;
using ef = EntityFramework_DB;

namespace readit.Data
{
    public class DBConnection
    {
        private readonly ReaditContext context = new ReaditContext();

        private ModelsTranslate md = new ModelsTranslate();

        private ControlLogs clog = new ControlLogs();

        public bool CadastrarUsuario(Usuario usuario)
        {
            using var transaction = context.Database.BeginTransaction();

            try
            {
                var usuarioDB = md.UsuarioModelToDB(usuario);

                context.Usuarios.Add(usuarioDB);
                context.SaveChanges();

                transaction.Commit();
                return true;
            }
            catch (Exception e)
            {
                clog.RealizarLogExcecao(e.ToString(), "CadastrarUsuario(Usuario usuario)");
                transaction.Rollback();
                return false;
            }
        }

        public List<Usuario> BuscarUsuarioPorEmail(string email)
        {
            try
            {
                ef.Models.Usuario[] usuDB;

                if (!string.IsNullOrEmpty(email))
                {
                    usuDB = (from u in context.Usuarios where u.UsuEmail == email select u).ToArray();
                }
                else
                {
                    usuDB = (from u in context.Usuarios select u).ToArray();
                }

                List<Usuario> listaUsuarios = new List<Usuario>();

                foreach (var usu in md.UsuariosDBToModel(usuDB.ToArray()))
                {
                    listaUsuarios.Add(usu);
                }

                return listaUsuarios;
            }
            catch (Exception e)
            {
                clog.RealizarLogExcecao(e.ToString(), "BuscarUsuarioPorEmail(string email)");
                return new List<Usuario>();
            }
        }

        public List<Obras> BuscarObrasPorId(int? idObra)
        {
            try
            {
                ef.Models.Obra[] obrasDB;

                if (idObra != null && idObra != 0)
                {
                    obrasDB = (from o in context.Obras where o.ObsId == idObra select o).ToArray();
                }
                else
                {
                    obrasDB = (from o in context.Obras select o).ToArray();
                }

                List<Obras> listaObras = new List<Obras>();

                foreach (var obra in md.ObrasDBToModel(obrasDB.ToArray()))
                {
                    listaObras.Add(obra);
                }

                return listaObras;
            }
            catch (Exception e)
            {
                clog.RealizarLogExcecao(e.ToString(), "BuscarObrasPorId(int? idObra)");
                return new List<Obras>();
            }
        }

        public List<CapitulosObra> BuscarCapituloObrasPorIds(List<int> idsObra)
        {
            try
            {
                ef.Models.CapitulosObra[] capitulosObrasDB;

                if (idsObra.Count > 0)
                {
                    capitulosObrasDB = (from c in context.CapitulosObras where idsObra.Contains(c.ObsId) select c).ToArray();
                }
                else
                {
                    capitulosObrasDB = (from c in context.CapitulosObras select c).ToArray();
                }

                List<CapitulosObra> listaCapitulosObras = new List<CapitulosObra>();

                foreach (var capObra in md.CapitulosObrasDBToModel(capitulosObrasDB.ToArray()))
                {
                    listaCapitulosObras.Add(capObra);
                }

                return listaCapitulosObras;
            }
            catch (Exception e)
            {
                clog.RealizarLogExcecao(e.ToString(), "BuscarCapituloObrasPorIds(List<int> idsObra)");
                return new List<CapitulosObra>();
            }
        }

        public List<PaginasCapitulo> BuscarPaginasCapituloPorIds(List<int> idsCap)
        {
            try
            {
                ef.Models.PaginasCapitulo[] paginasCapituloDB;

                if (idsCap.Count > 0)
                {
                    paginasCapituloDB = (from p in context.PaginasCapitulos where idsCap.Contains(p.CpoId) select p).ToArray();
                }
                else
                {
                    paginasCapituloDB = (from p in context.PaginasCapitulos select p).ToArray();
                }

                List<PaginasCapitulo> listaPaginasCapitulo = new List<PaginasCapitulo>();

                foreach (var pagCap in md.PaginasCapituloDBToModel(paginasCapituloDB.ToArray()))
                {
                    listaPaginasCapitulo.Add(pagCap);
                }

                return listaPaginasCapitulo;
            }
            catch (Exception e)
            {
                clog.RealizarLogExcecao(e.ToString(), "BuscarPaginasCapituloPorIds(List<int> idsCap)");
                return new List<PaginasCapitulo>();
            }
        }

        public List<Obras> BuscarObrasPorNome(string nomeObra)
        {
            try
            {
                ef.Models.Obra[] obrasDB;

                if (!string.IsNullOrEmpty(nomeObra))
                {
                    obrasDB = (from o in context.Obras where o.ObsNomeObra == nomeObra select o).ToArray();
                }
                else
                {
                    obrasDB = (from o in context.Obras select o).ToArray();
                }

                List<Obras> listaObras = new List<Obras>();

                foreach (var obra in md.ObrasDBToModel(obrasDB.ToArray()))
                {
                    listaObras.Add(obra);
                }

                return listaObras;
            }
            catch (Exception e)
            {
                clog.RealizarLogExcecao(e.ToString(), "BuscarObrasPorNome()");
                return new List<Obras>();
            }
        }

        public List<Generos> BuscarGenerosPorNome(string nomeGenero)
        {
            try
            {
                ef.Models.Genero[] generosDB;

                if (!string.IsNullOrEmpty(nomeGenero))
                {
                    generosDB = (from g in context.Generos where g.GnsNome == nomeGenero select g).ToArray();
                }
                else
                {
                    generosDB = (from g in context.Generos select g).ToArray();
                }

                List<Generos> listaGeneros = new List<Generos>();

                foreach (var genero in md.GenerosDBToModel(generosDB.ToArray()))
                {
                    listaGeneros.Add(genero);
                }

                return listaGeneros;
            }
            catch (Exception e)
            {
                clog.RealizarLogExcecao(e.ToString(), "BuscarGenerosPorNome(string nomeGenero)");
                return new List<Generos>();
            }
        }

        public List<Generos> BuscarGenerosPorObra(int? idObra)
        {
            try
            {
                ef.Models.Genero[] generosDB;

                if (idObra != null && idObra != 0)
                {
                    generosDB = (from g in context.Generos
                                 join og in context.ObrasGeneros on g.GnsId equals og.GnsId
                                 where og.ObsId == idObra
                                 select g).ToArray();
                }
                else
                {
                    generosDB = (from g in context.Generos select g).ToArray();
                }

                List<Generos> listaGeneros = new List<Generos>();

                foreach (var genero in md.GenerosDBToModel(generosDB.ToArray()))
                {
                    listaGeneros.Add(genero);
                }

                return listaGeneros;
            }
            catch (Exception e)
            {
                clog.RealizarLogExcecao(e.ToString(), "BuscarGenerosPorObra(int? idObra)");
                return new List<Generos>();
            }
        }

        public bool CadastrarObra(Obras obra, Imagens imagem, List<Generos> listaGeneros)
        {
            using var transaction = context.Database.BeginTransaction();

            try
            {
                var imagemDB = md.ImagemModelToDB(imagem);
                var obraDB = md.ObraModelToDB(obra);

                context.Imagens.Add(imagemDB);
                context.SaveChanges();

                obraDB.ImgId = imagemDB.ImgId;

                context.Obras.Add(obraDB);
                context.SaveChanges();

                foreach (var genero in listaGeneros)
                {
                    context.ObrasGeneros.Add(new ef.Models.ObrasGenero
                    {
                        GnsId = genero.Id,
                        ObsId = obraDB.ObsId
                    });
                    context.SaveChanges();
                }

                transaction.Commit();
                return true;
            }
            catch (Exception e)
            {
                clog.RealizarLogExcecao(e.ToString(), "CadastrarObra(Obras obra, Imagens imagem, List<Generos> listaGeneros)");
                transaction.Rollback();
                return false;
            }
        }

        public bool CadastrarGenero(Generos genero)
        {
            using var transaction = context.Database.BeginTransaction();

            try
            {
                var generoDB = md.GeneroModelToDB(genero);

                context.Entry(generoDB).State = generoDB.GnsId == 0 ? EntityState.Added : EntityState.Modified;
                context.SaveChanges();

                transaction.Commit();
                return true;
            }
            catch (Exception e)
            {
                clog.RealizarLogExcecao(e.ToString(), "CadastrarGenero(Generos genero)");
                transaction.Rollback();
                return false;
            }
        }

        public bool CadastrarCapitulos(List<CapitulosObra> listaCapitulosObra)
        {
            using var transaction = context.Database.BeginTransaction();

            try
            {
                foreach (var capObra in listaCapitulosObra)
                {
                    var capObraDB = md.CapitulosObraDBToModel(capObra);

                    context.Entry(capObraDB).State = capObraDB.CpoId == 0 ? EntityState.Added : EntityState.Modified;
                    context.SaveChanges();

                    foreach (var paginaObra in capObra.ListaPaginas)
                    {
                        paginaObra.CapituloId = capObraDB.CpoId;

                        var pagObraDB = md.PaginasCapituloDBToModel(paginaObra);

                        context.Entry(pagObraDB).State = pagObraDB.PgcId == 0 ? EntityState.Added : EntityState.Modified;
                        context.SaveChanges();
                    }
                }

                transaction.Commit();
                return true;
            }
            catch (Exception e)
            {
                clog.RealizarLogExcecao(e.ToString(), "CadastrarCapitulos(List<CapitulosObra> listaCapitulosObra)");
                transaction.Rollback();
                return false;
            }
        }

        public void RealizarConexaoDB()
        {
            try
            {
                ef.Global.ConnectionString = App.config.GetConnectionString("ConnectionString");
            }
            catch (Exception e)
            {
                clog.RealizarLogExcecao(e.ToString(), "RealizarConexaoDB()");
            }
        }

        public bool TestarConexaoDB()
        {
            try
            {
                return context.Database.CanConnect();
            }
            catch (Exception e)
            {
                clog.RealizarLogExcecao(e.ToString(), "TestarConexaoDB()");
                return false;
            }
        }
    }
}