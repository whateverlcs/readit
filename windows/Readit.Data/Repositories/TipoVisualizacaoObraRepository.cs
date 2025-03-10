using Microsoft.EntityFrameworkCore;
using Readit.Core.Domain;
using Readit.Core.Repositories;
using Readit.Data.Context;
using Readit.Data.Mappers;
using Readit.Infra.Logging;
using ef = Readit.Data;

namespace Readit.Data.Repositories
{
    public class TipoVisualizacaoObraRepository : ITipoVisualizacaoObraRepository
    {
        private readonly IDbContextFactory<ReaditContext> _contextFactory;
        private readonly ILoggingService _logger;

        public TipoVisualizacaoObraRepository(IDbContextFactory<ReaditContext> contextFactory, ILoggingService logger)
        {
            _contextFactory = contextFactory;
            _logger = logger;
        }

        public async Task<List<TipoVisualizacaoObra>> BuscarTiposVisualizacaoObraPorIdAsync(int idTipoVisualizacao)
        {
            using (var _context = _contextFactory.CreateDbContext())
            {
                try
                {
                    ef.Models.TipoVisualizacaoObra[] tipoVisualizacaoObraDB;

                    if (idTipoVisualizacao != 0)
                    {
                        tipoVisualizacaoObraDB = await (from tvo in _context.TipoVisualizacaoObras
                                                        where tvo.TvoId == idTipoVisualizacao
                                                        select tvo).ToArrayAsync();
                    }
                    else
                    {
                        tipoVisualizacaoObraDB = await (from tvo in _context.TipoVisualizacaoObras
                                                        select tvo).ToArrayAsync();
                    }

                    List<TipoVisualizacaoObra> listaTiposVisualizacaoObra = new List<TipoVisualizacaoObra>();

                    foreach (var tvo in tipoVisualizacaoObraDB.ToDomainList())
                    {
                        listaTiposVisualizacaoObra.Add(tvo);
                    }

                    return listaTiposVisualizacaoObra;
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "BuscarTiposVisualizacaoObraPorIdAsync(int idTipoVisualizacao)");
                    return new List<TipoVisualizacaoObra>();
                }
            }
        }

        public async Task<List<TipoVisualizacaoObraUsuario>> BuscarTiposVisualizacaoObraUsuarioPorIdAsync(int idUsuario)
        {
            using (var _context = _contextFactory.CreateDbContext())
            {
                try
                {
                    ef.Models.TipoVisualizacaoObraUsuario[] tipoVisualizacaoObraUsuarioDB;

                    if (idUsuario != 0)
                    {
                        tipoVisualizacaoObraUsuarioDB = await (from tvou in _context.TipoVisualizacaoObraUsuarios
                                                               where tvou.UsuId == idUsuario
                                                               select tvou).ToArrayAsync();
                    }
                    else
                    {
                        tipoVisualizacaoObraUsuarioDB = await (from tvou in _context.TipoVisualizacaoObraUsuarios
                                                               select tvou).ToArrayAsync();
                    }

                    List<TipoVisualizacaoObraUsuario> listaTiposVisualizacaoObraUsuario = new List<TipoVisualizacaoObraUsuario>();

                    foreach (var tvou in tipoVisualizacaoObraUsuarioDB.ToDomainList())
                    {
                        listaTiposVisualizacaoObraUsuario.Add(tvou);
                    }

                    return listaTiposVisualizacaoObraUsuario;
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "BuscarTiposVisualizacaoObraUsuarioPorIdAsync(int idUsuario)");
                    return new List<TipoVisualizacaoObraUsuario>();
                }
            }
        }
    }
}