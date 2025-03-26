using Microsoft.EntityFrameworkCore;
using Readit.Core.Domain;
using Readit.Core.Repositories;
using Readit.Core.Services;
using Readit.Data.Context;
using Readit.Infra.Logging;

namespace Readit.Data.Repositories
{
    public class PreferenciasRepository : IPreferenciasRepository
    {
        private readonly IDbContextFactory<ReaditContext> _contextFactory;
        private readonly ILoggingService _logger;
        private readonly IUsuarioService _usuarioService;

        public PreferenciasRepository(IDbContextFactory<ReaditContext> contextFactory, ILoggingService logger, IUsuarioService usuarioService)
        {
            _contextFactory = contextFactory;
            _logger = logger;
            _usuarioService = usuarioService;
        }

        public async Task<List<Preferencias>> BuscarPreferenciasAsync()
        {
            using (var _context = _contextFactory.CreateDbContext())
            {
                try
                {
                    var preferenciasUsuarioDB = await (from pf in _context.Preferencias
                                                       select new Preferencias
                                                       {
                                                           Id = pf.PreId,
                                                           Nome = pf.PrePreferencia
                                                       }).ToListAsync(_usuarioService.Token);

                    return preferenciasUsuarioDB;
                }
                catch (TaskCanceledException)
                {
                    return new List<Preferencias>();
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "BuscarPreferenciasAsync()");
                    return new List<Preferencias>();
                }
            }
        }

        public async Task<List<PreferenciasUsuario>> BuscarPreferenciasUsuarioAsync()
        {
            using (var _context = _contextFactory.CreateDbContext())
            {
                if (_usuarioService.UsuarioLogado == null)
                    return new List<PreferenciasUsuario>();

                try
                {
                    var preferenciasUsuarioDB = await (from pf in _context.PreferenciasUsuarios
                                                       join p in _context.Preferencias on pf.PreId equals p.PreId
                                                       where pf.UsuId == _usuarioService.UsuarioLogado.Id
                                                       select new PreferenciasUsuario
                                                       {
                                                           Id = pf.PfuId,
                                                           IdUsuario = pf.UsuId,
                                                           IdPreferencia = pf.PreId,
                                                           Preferencia = p.PrePreferencia
                                                       }).ToListAsync(_usuarioService.Token);

                    return preferenciasUsuarioDB;
                }
                catch (TaskCanceledException)
                {
                    return new List<PreferenciasUsuario>();
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "BuscarPreferenciasUsuarioAsync()");
                    return new List<PreferenciasUsuario>();
                }
            }
        }
    }
}