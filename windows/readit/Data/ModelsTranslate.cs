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

        public ef.Imagen ImagemModelToUsuarioDB(Imagens imagem)
        {
            ef.Imagen img = new ef.Imagen
            {
                ImgId = imagem.Id,
                ImgFormato = imagem.Formato,
                ImgDataInclusao = imagem.DataInclusao,
                ImgDataAtualizacao = imagem.DataAtualizacao,
                ImgImagem = imagem.Imagem,
                ImgTipo = imagem.Tipo
            };

            return img;
        }

        public ef.Obra ObraModelToUsuarioDB(Obras obra)
        {
            ef.Obra ob = new ef.Obra
            {
                ObsId = obra.Id,
                ObsNomeObra = obra.NomeObra,
                ObsStatus = obra.Status,
                ObsTipo = obra.Tipo,
                ObsDescricao = obra.Descricao,
                ObsDataPublicacao = obra.DataPublicacao,
                ObsDataAtualizacao = obra.DataAtualizacao,
                UsuId = obra.UsuarioId,
                ImgId = obra.ImagemId
            };

            return ob;
        }

        public List<Usuario> UsuariosDBToModel(ef.Usuario[] usuariosDB)
        {
            List<Usuario> listaUsuarios = [];

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

        public List<Obras> ObrasDBToModel(ef.Obra[] obrasDB)
        {
            List<Obras> listaObras = [];

            foreach (var obraDB in obrasDB)
            {
                Obras obra = new Obras
                {
                    Id = obraDB.ObsId,
                    NomeObra = obraDB.ObsNomeObra,
                    Status = obraDB.ObsStatus,
                    Tipo = obraDB.ObsTipo,
                    Descricao = obraDB.ObsDescricao,
                    DataAtualizacao = obraDB.ObsDataAtualizacao,
                    DataPublicacao = obraDB.ObsDataPublicacao,
                    ImagemId = obraDB.ImgId,
                    UsuarioId = obraDB.UsuId
                };

                listaObras.Add(obra);
            }

            return listaObras;
        }

        public List<Generos> GenerosDBToModel(ef.Genero[] generosDB)
        {
            List<Generos> listaGeneros = [];

            foreach (var generoDB in generosDB)
            {
                Generos genero = new Generos
                {
                    Id = generoDB.GnsId,
                    Nome = generoDB.GnsNome,
                };

                listaGeneros.Add(genero);
            }

            return listaGeneros;
        }
    }
}