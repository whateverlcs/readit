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
                await using var transaction = await _context.Database.BeginTransactionAsync(_usuarioService.Token);

                try
                {
                    if (_usuarioService.UsuarioLogado == null)
                        return false;

                    ef.Models.AvaliacoesObra avDB = await (from av in _context.AvaliacoesObras
                                                           where av.ObsId == obraId && av.UsuId == _usuarioService.UsuarioLogado.Id
                                                           select av).FirstOrDefaultAsync(_usuarioService.Token);

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
                    _logger.LogError(e, "AtualizarRatingAsync(int obraId, double rating)");
                    await transaction.RollbackAsync(_usuarioService.Token);
                    return false;
                }
            }
        }

        public async Task<double> BuscarRatingObraAsync(int obraId)
        {
            using (var _context = _contextFactory.CreateDbContext())
            {
                try
                {
                    return await _context.AvaliacoesObras
                    .Where(av => av.ObsId == obraId)
                    .Select(av => (double?)av.AvoNota)
                    .AverageAsync(_usuarioService.Token) ?? 0;
                }
                catch (TaskCanceledException)
                {
                    return 0;
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "BuscarRatingObraAsync(int obraId)");
                    return 0;
                }
            }
        }
    }
}