using Readit.Core.Domain;
using ef = Readit.Data.Models;

namespace Readit.Data.Mappers
{
    public static class TipoVisualizacaoObraUsuarioMapper
    {
        public static ef.TipoVisualizacaoObraUsuario ToEntity(this TipoVisualizacaoObraUsuario tipoVisualizacaoObraUsuario)
        {
            return new ef.TipoVisualizacaoObraUsuario
            {
                TvouId = tipoVisualizacaoObraUsuario.Id,
                TvoId = tipoVisualizacaoObraUsuario.TipoVisualizacaoObraId,
                UsuId = tipoVisualizacaoObraUsuario.UsuarioId,
            };
        }

        public static TipoVisualizacaoObraUsuario ToDomain(this ef.TipoVisualizacaoObraUsuario tipoVisualizacaoObraUsuario)
        {
            return new TipoVisualizacaoObraUsuario
            {
                Id = tipoVisualizacaoObraUsuario.TvouId,
                TipoVisualizacaoObraId = tipoVisualizacaoObraUsuario.TvoId,
                UsuarioId = tipoVisualizacaoObraUsuario.UsuId,
            };
        }

        public static List<TipoVisualizacaoObraUsuario> ToDomainList(this ef.TipoVisualizacaoObraUsuario[] tipoVisualizacaoObraUsuarioDB)
        {
            return tipoVisualizacaoObraUsuarioDB
                .Select(tvouDB => new TipoVisualizacaoObraUsuario
                {
                    Id = tvouDB.TvouId,
                    TipoVisualizacaoObraId = tvouDB.TvoId,
                    UsuarioId = tvouDB.UsuId
                })
                .ToList();
        }
    }
}