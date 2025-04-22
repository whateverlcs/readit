using Readit.Core.Desktop.Domain;
using Readit.Core.Domain;

namespace Readit.Infra.Desktop.Mappers
{
    public static class ComentarioMapper
    {
        public static ComentariosDesktop DomainToDesktop(this Comentarios comentario)
        {
            return new ComentariosDesktop
            {
                Id = comentario.Id,
                ImageByte = comentario.ImageByte,
                UsuarioApelido = comentario.UsuarioApelido,
                TempoUltimaAtualizacaoFormatado = comentario.TempoUltimaAtualizacaoFormatado,
                TempoDecorrido = comentario.TempoDecorrido,
                TempoUltimaAtualizacaoDecorrido = comentario.TempoUltimaAtualizacaoDecorrido,
                IdObra = comentario.IdObra,
                IdUsuario = comentario.IdUsuario,
                IdCapitulo = comentario.IdCapitulo,
                ComentarioTexto = comentario.ComentarioTexto,
                ContadorLikes = comentario.ContadorLikes,
                ContadorDislikes = comentario.ContadorDislikes,
                TempoDecorridoFormatado = comentario.TempoDecorridoFormatado,
                IsUsuarioOuAdministrador = comentario.IsUsuarioOuAdministrador
            };
        }
    }
}