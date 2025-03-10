using Microsoft.EntityFrameworkCore;
using Readit.Core.Domain;
using Readit.Core.Repositories;
using Readit.Data.Context;
using Readit.Data.Mappers;
using Readit.Infra.Logging;
using ef = Readit.Data;

namespace Readit.Data.Repositories
{
    public class PaginaCapituloRepository : IPaginaCapituloRepository
    {
        private readonly IDbContextFactory<ReaditContext> _contextFactory;
        private readonly ILoggingService _logger;

        public PaginaCapituloRepository(IDbContextFactory<ReaditContext> contextFactory, ILoggingService logger)
        {
            _contextFactory = contextFactory;
            _logger = logger;
        }

        public async Task<List<PaginasCapitulo>> BuscarPaginasCapituloPorIdsAsync(List<int> idsCap)
        {
            using (var _context = _contextFactory.CreateDbContext())
            {
                try
                {
                    ef.Models.PaginasCapitulo[] paginasCapituloDB;

                    if (idsCap.Count > 0)
                    {
                        paginasCapituloDB = await (from p in _context.PaginasCapitulos
                                                   where idsCap.Contains(p.CpoId)
                                                   select p).ToArrayAsync();
                    }
                    else
                    {
                        paginasCapituloDB = await (from p in _context.PaginasCapitulos
                                                   select p).ToArrayAsync();
                    }

                    List<PaginasCapitulo> listaPaginasCapitulo = new List<PaginasCapitulo>();

                    foreach (var pagCap in paginasCapituloDB.ToDomainList())
                    {
                        listaPaginasCapitulo.Add(pagCap);
                    }

                    return listaPaginasCapitulo;
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "BuscarPaginasCapituloPorIdsAsync(List<int> idsCap)");
                    return new List<PaginasCapitulo>();
                }
            }
        }
    }
}