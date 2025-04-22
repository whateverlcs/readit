using Microsoft.EntityFrameworkCore;
using Readit.Core.Domain;
using Readit.Core.Repositories;
using Readit.Core.Services;
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
        private readonly IUsuarioService _usuarioService;

        public PaginaCapituloRepository(IDbContextFactory<ReaditContext> contextFactory, ILoggingService logger, IUsuarioService usuarioService)
        {
            _contextFactory = contextFactory;
            _logger = logger;
            _usuarioService = usuarioService;
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
                                                   select p).ToArrayAsync(_usuarioService.Token);
                    }
                    else
                    {
                        paginasCapituloDB = await (from p in _context.PaginasCapitulos
                                                   select p).ToArrayAsync(_usuarioService.Token);
                    }

                    List<PaginasCapitulo> listaPaginasCapitulo = new List<PaginasCapitulo>();

                    foreach (var pagCap in paginasCapituloDB.ToDomainList())
                    {
                        listaPaginasCapitulo.Add(pagCap);
                    }

                    return listaPaginasCapitulo;
                }
                catch (TaskCanceledException)
                {
                    return new List<PaginasCapitulo>();
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