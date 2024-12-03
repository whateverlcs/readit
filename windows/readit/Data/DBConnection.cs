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
                var usuarioDB = md.UsuarioModelToUsuarioDB(usuario);

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
                clog.RealizarLogExcecao(e.ToString(), "BuscarObrasPorId()");
                return new List<Obras>();
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
                var imagemDB = md.ImagemModelToUsuarioDB(imagem);
                var obraDB = md.ObraModelToUsuarioDB(obra);

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