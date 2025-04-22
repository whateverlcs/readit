using Readit.Core.Domain;
using ef = Readit.Data.Models;

namespace Readit.Data.Mappers
{
    public static class PaginasCapituloMapper
    {
        public static ef.PaginasCapitulo ToEntity(this PaginasCapitulo pagCap)
        {
            return new ef.PaginasCapitulo
            {
                PgcId = pagCap.Id,
                PgcNumeroPagina = pagCap.NumeroPagina,
                PgcPagina = pagCap.Pagina,
                PgcTamanho = pagCap.Tamanho,
                CpoId = pagCap.CapituloId
            };
        }

        public static PaginasCapitulo ToDomain(this ef.PaginasCapitulo pagCap)
        {
            return new PaginasCapitulo
            {
                Id = pagCap.PgcId,
                NumeroPagina = pagCap.PgcNumeroPagina,
                Pagina = pagCap.PgcPagina,
                Tamanho = pagCap.PgcTamanho,
                CapituloId = pagCap.CpoId,
            };
        }

        public static List<PaginasCapitulo> ToDomainList(this ef.PaginasCapitulo[] paginasCapitulo)
        {
            return paginasCapitulo
                .Select(pagCapDB => new PaginasCapitulo
                {
                    Id = pagCapDB.PgcId,
                    NumeroPagina = pagCapDB.PgcNumeroPagina,
                    Pagina = pagCapDB.PgcPagina,
                    Tamanho = pagCapDB.PgcTamanho,
                    CapituloId = pagCapDB.CpoId
                })
                .ToList();
        }
    }
}