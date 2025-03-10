using Microsoft.EntityFrameworkCore;
using Readit.Core.Repositories;
using Readit.Core.Services;
using Readit.Data.Context;
using Readit.Infra.Logging;
using ef = Readit.Data;

namespace Readit.Data.Repositories
{
    public class AvaliacaoObraRepository : IAvaliacaoObraRepository
    {
        private readonly IDbContextFactory<ReaditContext> _contextFactory;
        private readonly ILoggingService _logger;
        private readonly IUsuarioService _usuarioService;

        public AvaliacaoObraRepository(IDbContextFactory<ReaditContext> contextFactory, ILoggingService logger, IUsuarioService usuarioService)
        {
            _contextFactory = contextFactory;
            _logger = logger;
            _usuarioService = usuarioService;
        }

        public async Task<bool> AtualizarRatingAsync(int obraId, double rating)
        {
            using (var _context = _contextFactory.CreateDbContext())
            {
                await using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    ef.Models.AvaliacoesObra avDB = await (from av in _context.AvaliacoesObras
                                                           where av.ObsId == obraId && av.UsuId == _usuarioService.UsuarioLogado.Id
                                                           select av).FirstOrDefaultAsync();

                    if (avDB == null)
                    {
                        avDB = new ef.Models.AvaliacoesObra
                        {
                            ObsId = obraId,
                            UsuId = _usuarioService.UsuarioLogado.Id,
                            AvoNota = Convert.ToByte(rating),
                            AvoDataAvaliacao = DateTime.Now,
                            AvoDataAtualizacao = DateTime.Now
                        };
                    }
                    else
                    {
                        avDB.AvoNota = Convert.ToByte(rating);
                        avDB.AvoDataAtualizacao = DateTime.Now;
                    }

                    _context.Entry(avDB).State = avDB.AvoId != 0 ? EntityState.Modified : EntityState.Added;
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();
                    return true;
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "AtualizarRatingAsync(int obraId, double rating)");
                    await transaction.RollbackAsync();
                    return false;
                }
            }
        }
    }
}