using Readit.Core.Domain;
using ef = Readit.Data.Models;

namespace Readit.Data.Mappers
{
    public static class UsuarioMapper
    {
        public static ef.Usuario ToEntity(this Usuario usuario)
        {
            return new ef.Usuario
            {
                UsuId = usuario.Id,
                UsuNome = usuario.Nome,
                UsuApelido = usuario.Apelido,
                UsuEmail = usuario.Email,
                UsuSenha = usuario.Senha,
                UsuAdministrador = usuario.Administrador,
                ImgId = usuario.IdImagem
            };
        }

        public static Usuario ToDomain(this ef.Usuario usuario)
        {
            return new Usuario
            {
                Id = usuario.UsuId,
                Nome = usuario.UsuNome,
                Apelido = usuario.UsuApelido,
                Email = usuario.UsuEmail,
                Senha = usuario.UsuSenha,
                Administrador = usuario.UsuAdministrador,
                IdImagem = usuario.ImgId
            };
        }

        public static List<Usuario> ToDomainList(this ef.Usuario[] usuario)
        {
            return usuario
                .Select(usuDB => new Usuario
                {
                    Id = usuDB.UsuId,
                    Nome = usuDB.UsuNome,
                    Apelido = usuDB.UsuApelido,
                    Email = usuDB.UsuEmail,
                    Senha = usuDB.UsuSenha,
                    Administrador = usuDB.UsuAdministrador,
                    IdImagem = usuDB.ImgId
                })
                .ToList();
        }
    }
}