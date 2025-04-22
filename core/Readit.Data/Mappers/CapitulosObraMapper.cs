using Readit.Core.Domain;
using ef = Readit.Data.Models;

namespace Readit.Data.Mappers
{
    public static class CapitulosObraMapper
    {
        public static ef.CapitulosObra ToEntity(this CapitulosObra capObra)
        {
            return new ef.CapitulosObra
            {
                CpoId = capObra.Id,
                CpoNumeroCapitulo = capObra.NumeroCapitulo,
                CpoDataPublicacao = capObra.DataPublicacao,
                CpoDataAtualizacao = capObra.DataAtualizacao,
                UsuId = capObra.UsuarioId,
                ObsId = capObra.ObraId
            };
        }

        public static CapitulosObra ToDomain(this ef.CapitulosObra capituloObra)
        {
            return new CapitulosObra
            {
                Id = capituloObra.CpoId,
                NumeroCapitulo = capituloObra.CpoNumeroCapitulo,
                DataPublicacao = capituloObra.CpoDataPublicacao,
                DataAtualizacao = capituloObra.CpoDataAtualizacao,
                UsuarioId = capituloObra.UsuId,
                ObraId = capituloObra.ObsId
            };
        }

        public static CapitulosObra ToDomainDynamic(this IEnumerable<dynamic> capituloObrasDB)
        {
            var capPagObraDB = capituloObrasDB.First();

            return new CapitulosObra
            {
                Id = capPagObraDB.IdCapitulo,
                ObraId = capPagObraDB.IdObra,
                NomeObra = capPagObraDB.NomeObra,
                NumeroCapituloDisplay = $"Capítulo {capPagObraDB.NumeroCapitulo:D2}",
                ListaPaginas = capituloObrasDB.Select(p => new PaginasCapitulo
                {
                    Pagina = p.Pagina,
                    NumeroPagina = p.NumeroPagina,
                    CapituloId = p.IdCapitulo
                }).ToList()
            };
        }

        public static List<CapitulosObra> ToDomainList(this ef.CapitulosObra[] capitulosObra)
        {
            return capitulosObra
                .Select(capObraDB => new CapitulosObra
                {
                    Id = capObraDB.CpoId,
                    NumeroCapitulo = capObraDB.CpoNumeroCapitulo,
                    DataPublicacao = capObraDB.CpoDataPublicacao,
                    DataAtualizacao = capObraDB.CpoDataAtualizacao,
                    UsuarioId = capObraDB.UsuId,
                    ObraId = capObraDB.ObsId
                })
                .ToList();
        }

        public static List<CapitulosObra> ToDomainListReduzido(this IEnumerable<dynamic> capObraDB)
        {
            return capObraDB
                .Select(capObraDB => new CapitulosObra
                {
                    Id = capObraDB.Id,
                    NumeroCapitulo = capObraDB.NumeroCapitulo,
                    NumeroCapituloDisplay = $"Capítulo {capObraDB.NumeroCapitulo:D2}",
                    ObraId = capObraDB.IdObra,
                })
                .ToList();
        }
    }
}