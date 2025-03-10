using Microsoft.EntityFrameworkCore;
using Readit.Core.Services;
using Readit.Data.Context;
using Readit.Infra.Logging;

namespace Readit.Data.Services
{
    public class DatabaseService : IDatabaseService
    {
        private readonly IDbContextFactory<ReaditContext> _contextFactory;
        private readonly ILoggingService _log;

        public DatabaseService(IDbContextFactory<ReaditContext> contextFactory, ILoggingService log)
        {
            _contextFactory = contextFactory;
            _log = log;
        }

        public async Task<bool> TestarConexaoDBAsync()
        {
            using (var _context = _contextFactory.CreateDbContext())
            {
                try
                {
                    return await _context.Database.CanConnectAsync();
                }
                catch (Exception e)
                {
                    _log.LogError(e, "TestarConexaoDB()");
                    return false;
                }
            }
        }
    }
}