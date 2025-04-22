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
    public class BookmarkRepository : IBookmarkRepository
    {
        private readonly IDbContextFactory<ReaditContext> _contextFactory;
        private readonly ILoggingService _logger;
        private readonly IUsuarioService _usuarioService;

        public BookmarkRepository(IDbContextFactory<ReaditContext> contextFactory, ILoggingService logger, IUsuarioService usuarioService)
        {
            _contextFactory = contextFactory;
            _logger = logger;
            _usuarioService = usuarioService;
        }

        public async Task<(bool, string)> CadastrarRemoverBookmarkAsync(BookmarksUsuario bookmarkUsuario)
        {
            using (var _context = _contextFactory.CreateDbContext())
            {
                await using var transaction = await _context.Database.BeginTransactionAsync(_usuarioService.Token);

                try
                {
                    var bookmarkDB = bookmarkUsuario.ToEntity();

                    ef.Models.BookmarksUsuario bkuDB = await (from bku in _context.BookmarksUsuarios
                                                              where bku.ObsId == bookmarkDB.ObsId && bku.UsuId == bookmarkDB.UsuId
                                                              select bku)
                                                               .FirstOrDefaultAsync(_usuarioService.Token);

                    var bkEntry = bkuDB != null ? bkuDB : bookmarkDB;

                    _context.Entry(bkEntry).State = bkuDB != null ? EntityState.Deleted : EntityState.Added;

                    await _context.SaveChangesAsync(_usuarioService.Token);

                    await transaction.CommitAsync(_usuarioService.Token);
                    return (true, (bkuDB != null ? "Removido" : "Adicionado"));
                }
                catch (TaskCanceledException)
                {
                    return (false, "");
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "CadastrarRemoverBookmarkAsync(BookmarksUsuario bookmarkUsuario)");
                    await transaction.RollbackAsync(_usuarioService.Token);
                    return (false, "");
                }
            }
        }
    }
}