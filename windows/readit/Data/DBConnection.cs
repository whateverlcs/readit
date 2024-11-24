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