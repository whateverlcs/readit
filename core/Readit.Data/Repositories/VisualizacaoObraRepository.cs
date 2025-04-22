using Microsoft.EntityFrameworkCore;
using Readit.Core.Repositories;
using Readit.Core.Services;
using Readit.Data.Context;
using Readit.Infra.Logging;
using ef = Readit.Data;

namespace Readit.Data.Repositories
{
    public class VisualizacaoObraRepository : IVisualizacaoObraRepository
    {
        private readonly IDbContextFactory<ReaditContext> _contextFactory;
        private readonly ILoggingService _logger;
        private readonly IUsuarioService _usuarioService;

        public VisualizacaoObraRepository(IDbContextFactory<ReaditContext> contextFactory, ILoggingService logger, IUsuarioService usuarioService)
        {
            _contextFactory = contextFactory;
            _logger = logger;
            _usuarioService = usuarioService;
        }

        public async Task<bool> AtualizarViewObraAsync(string nomeObra)
        {
            using (var _context = _contextFactory.CreateDbContext())
            {
                using var transaction = await _context.Database.BeginTransactionAsync(_usuarioService.Token);

                try
                {
                    ef.Models.Obra obraDB = await (from o in _context.Obras
                                                   where o.ObsNomeObra == nomeObra
                                                   select o).FirstOrDefaultAsync(_usuarioService.Token);

                    ef.Models.VisualizacoesObra voDB = await (from vo in _context.VisualizacoesObras
                                                              where vo.ObsId == obraDB.ObsId
                                                              select vo).FirstOrDefaultAsync(_usuarioService.Token);

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
                    await _context.SaveChangesAsync(_usuarioService.Token);

                    await transaction.CommitAsync(_usuarioService.Token);
                    return true;
                }
                catch (TaskCanceledException)
                {
                    return false;
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "AtualizarViewObraAsync(string nomeObra)");
                    await transaction.RollbackAsync(_usuarioService.Token);
                    return false;
                }
            }
        }
    }
}