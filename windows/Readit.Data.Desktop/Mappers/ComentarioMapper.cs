using Readit.Core.Desktop.Domain;
using Readit.Core.Domain;

namespace Readit.Data.Desktop.Mappers
{
    public static class ComentarioMapper
    {
        public static Comentarios DesktopToDomain(this ComentariosDesktop comentario)
        {
            return new Comentarios
            {
                Id = comentario.Id,
                ComentarioTexto = comentario.ComentarioTexto,
                TempoDecorrido = comentario.TempoDecorrido,
                TempoUltimaAtualizacaoDecorrido = comentario.TempoUltimaAtualizacaoDecorrido,
                IdObra = comentario.IdObra,
                IdUsuario = comentario.IdUsuario,
                IdCapitulo = comentario.IdCapitulo,
                Pai = comentario.Pai?.DesktopToDomain()
            };
        }
    }
}