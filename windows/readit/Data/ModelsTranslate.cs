using readit.Models;
using ef = EntityFramework_DB.Models;

namespace readit.Data
{
    public class ModelsTranslate
    {
        public ef.Usuario UsuarioModelToUsuarioDB(Usuario usuario)
        {
            ef.Usuario usu = new ef.Usuario
            {
                UsuId = usuario.Id,
                UsuNome = usuario.Nome,
                UsuApelido = usuario.Apelido,
                UsuEmail = usuario.Email,
                UsuSenha = usuario.Senha,
                UsuAdministrador = usuario.Administrador,
                ImgId = usuario.IdImagem
            };

            return usu;
        }

        public List<Usuario> UsuariosDBToModel(ef.Usuario[] usuariosDB)
        {
            List<Usuario> listaUsuarios = new List<Usuario>();

            foreach (var usuDB in usuariosDB)
            {
                Usuario usu = new Usuario
                {
                    Id = usuDB.UsuId,
                    Nome = usuDB.UsuNome,
                    Apelido = usuDB.UsuApelido,
                    Email = usuDB.UsuEmail,
                    Senha = usuDB.UsuSenha,
                    Administrador = usuDB.UsuAdministrador,
                    IdImagem = usuDB.ImgId
                };

                listaUsuarios.Add(usu);
            }

            return listaUsuarios;
        }
    }
}