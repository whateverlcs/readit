using Readit.Core.Domain;
using Readit.Core.Enums;
using ef = Readit.Data.Models;

namespace Readit.Data.Mappers
{
    public static class TipoVisualizacaoObraMapper
    {
        public static ef.TipoVisualizacaoObra ToEntity(this TipoVisualizacaoObra tipoVisualizacaoObra)
        {
            return new ef.TipoVisualizacaoObra
            {
                TvoId = tipoVisualizacaoObra.Id,
                TvoVisualizacao = tipoVisualizacaoObra.Visualizacao
            };
        }

        public static TipoVisualizacaoObra ToDomain(this ef.TipoVisualizacaoObra tipoVisualizacaoObra)
        {
            return new TipoVisualizacaoObra
            {
                Id = tipoVisualizacaoObra.TvoId,
                Nome = tipoVisualizacaoObra.TvoVisualizacao == (int)EnumObra.TipoVisualizacaoObra.PaginaInteira ? "Página Inteira" : "Por Página",
                Visualizacao = tipoVisualizacaoObra.TvoVisualizacao,
            };
        }

        public static List<TipoVisualizacaoObra> ToDomainList(this ef.TipoVisualizacaoObra[] tipoVisualizacaoObraUsuarioDB)
        {
            return tipoVisualizacaoObraUsuarioDB
                .Select(tvoDB => new TipoVisualizacaoObra
                {
                    Id = tvoDB.TvoId,
                    Nome = tvoDB.TvoVisualizacao == (int)EnumObra.TipoVisualizacaoObra.PaginaInteira ? "Página Inteira" : "Por Página",
                    Visualizacao = tvoDB.TvoVisualizacao,
                })
                .ToList();
        }
    }
}