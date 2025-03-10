using Microsoft.EntityFrameworkCore;
using Readit.Core.Repositories;
using Readit.Data.Context;
using Readit.Infra.Logging;
using ef = Readit.Data;

namespace Readit.Data.Repositories
{
    public class VisualizacaoObraRepository : IVisualizacaoObraRepository
    {
        private readonly IDbContextFactory<ReaditContext> _contextFactory;
        private readonly ILoggingService _logger;

        public VisualizacaoObraRepository(IDbContextFactory<ReaditContext> contextFactory, ILoggingService logger)
        {
            _contextFactory = contextFactory;
            _logger = logger;
        }

        public async Task<bool> AtualizarViewObraAsync(string nomeObra)
        {
            using (var _context = _contextFactory.CreateDbContext())
            {
                using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    ef.Models.Obra obraDB = await (from o in _context.Obras
                                                   where o.ObsNomeObra == nomeObra
                                                   select o).FirstOrDefaultAsync();

                    ef.Models.VisualizacoesObra voDB = await (from vo in _context.VisualizacoesObras
                                                              where vo.ObsId == obraDB.ObsId
                                                              select vo).FirstOrDefaultAsync();

                    if (voDB == null)
                    {
                        voDB = new ef.Models.VisualizacoesObra
                        {
                            ObsId = obraDB.ObsId,
                            VsoViews = 1
                        };
                    }
                    else
                    {
                        voDB.VsoViews++;
                    }

                    _context.Entry(voDB).State = voDB.VsoId != 0 ? EntityState.Modified : EntityState.Added;
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();
                    return true;
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "AtualizarViewObraAsync(string nomeObra)");
                    await transaction.RollbackAsync();
                    return false;
                }
            }
        }
    }
}