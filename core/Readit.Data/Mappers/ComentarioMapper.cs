using Readit.Core.Domain;
using ef = Readit.Data.Models;

namespace Readit.Data.Mappers
{
    public static class ComentarioMapper
    {
        public static ef.Comentario ToEntity(this Comentarios comentario)
        {
            return new ef.Comentario
            {
                CtsId = comentario.Id,
                CtsComentario = comentario.ComentarioTexto,
                CtsData = comentario.TempoDecorrido,
                CtsDataAtualizacao = comentario.TempoUltimaAtualizacaoDecorrido,
                ObsId = comentario.IdObra,
                UsuId = comentario.IdUsuario,
                CpoId = comentario.IdCapitulo
            };
        }

        public static List<Comentarios> ToDomainList(this ef.Comentario[] comentarios)
        {
            return comentarios
                .Select(comentarioDB => new Comentarios
                {
                    Id = comentarioDB.CtsId,
                    ComentarioTexto = comentarioDB.CtsComentario,
                    TempoDecorrido = comentarioDB.CtsData,
                    TempoUltimaAtualizacaoDecorrido = comentarioDB.CtsDataAtualizacao,
                    IdObra = comentarioDB.ObsId,
                    IdUsuario = comentarioDB.UsuId,
                    IdCapitulo = comentarioDB.CpoId
                })
                .ToList();
        }

        public static List<Comentarios> ToDomainListDynamic(this IEnumerable<dynamic> comentario)
        {
            return comentario
                .Select(comentarioDB => new Comentarios
                {
                    Id = comentarioDB.Id,
                    ComentarioTexto = comentarioDB.Comentario,
                    TempoDecorrido = comentarioDB.Data,
                    TempoUltimaAtualizacaoDecorrido = comentarioDB.DataAtualizacao,
                    ImageByte = comentarioDB.ImagemUsuario,
                    UsuarioApelido = comentarioDB.ApelidoUsuario,
                    ContadorLikes = comentarioDB.Likes,
                    ContadorDislikes = comentarioDB.Dislikes,
                    IdObra = comentarioDB.IdObra,
                    IdCapitulo = comentarioDB.IdCapitulo,
                    IdUsuario = comentarioDB.IdUsuario,
                    Respostas = new System.Collections.ObjectModel.ObservableCollection<Comentarios>(
                    (comentarioDB.Respostas as IEnumerable<dynamic> ?? Enumerable.Empty<dynamic>())
                    .Cast<dynamic>()
                    .Select(respostasDb => new Comentarios
                    {
                        Id = respostasDb.Id,
                        ComentarioTexto = respostasDb.Comentario,
                        TempoDecorrido = respostasDb.Data,
                        TempoUltimaAtualizacaoDecorrido = respostasDb.DataAtualizacao,
                        ImageByte = respostasDb.ImagemUsuario,
                        UsuarioApelido = respostasDb.ApelidoUsuario,
                        ContadorLikes = respostasDb.Likes,
                        ContadorDislikes = respostasDb.Dislikes,
                        IdObra = respostasDb.IdObra,
                        IdCapitulo = respostasDb.IdCapitulo,
                        IdUsuario = respostasDb.IdUsuario
                    }).ToList())
                })
                .ToList();
        }
    }
}