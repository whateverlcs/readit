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
    public class CapituloRepository : ICapituloRepository
    {
        private readonly IDbContextFactory<ReaditContext> _contextFactory;
        private readonly ILoggingService _logger;
        private readonly IUsuarioService _usuarioService;
        private readonly IComentarioRepository _comentarioRepository;

        public CapituloRepository(IDbContextFactory<ReaditContext> contextFactory, ILoggingService logger, IUsuarioService usuarioService, IComentarioRepository comentarioRepository)
        {
            _contextFactory = contextFactory;
            _logger = logger;
            _usuarioService = usuarioService;
            _comentarioRepository = comentarioRepository;
        }

        public async Task<(List<CapitulosObra>, CapitulosObra)> BuscarCapituloObrasPorIdAsync(int idObra, int chapterId, bool numeroCapitulos, bool paginasCapitulo)
        {
            using (var _context = _contextFactory.CreateDbContext())
            {
                try
                {
                    CapitulosObra cap = new CapitulosObra();

                    if (paginasCapitulo)
                    {
                        var paginasDB = await (from c in _context.CapitulosObras
                                               join o in _context.Obras on c.ObsId equals o.ObsId
                                               join po in _context.PaginasCapitulos on c.CpoId equals po.CpoId
                                               group new { c, po, o } by new { o.ObsNomeObra, c.CpoNumeroCapitulo, c.ObsId, po.PgcNumeroPagina, po.PgcPagina, po.CpoId } into obraGroup
                                               where obraGroup.Key.CpoId == chapterId && obraGroup.Key.ObsId == idObra
                                               select new
                                               {
                                                   IdObra = obraGroup.Key.ObsId,
                                                   NomeObra = obraGroup.Key.ObsNomeObra,
                                                   NumeroCapitulo = obraGroup.Key.CpoNumeroCapitulo,
                                                   NumeroPagina = obraGroup.Key.PgcNumeroPagina,
                                                   Pagina = obraGroup.Key.PgcPagina,
                                                   IdCapitulo = obraGroup.Key.CpoId,
                                               }).ToListAsync(_usuarioService.Token);

                        cap = paginasDB.ToDomainDynamic();
                    }

                    List<CapitulosObra> listaCapitulosObras = new List<CapitulosObra>();

                    if (numeroCapitulos)
                    {
                        var capitulosObrasDB = await (from c in _context.CapitulosObras
                                                      where c.ObsId == idObra
                                                      select new
                                                      {
                                                          Id = c.CpoId,
                                                          NumeroCapitulo = c.CpoNumeroCapitulo,
                                                          IdObra = c.ObsId
                                                      }).ToListAsync(_usuarioService.Token);

                        foreach (var capObra in capitulosObrasDB.ToDomainListReduzido())
                        {
                            capObra.NumeroCapituloDisplay = $"Capítulo {capObra.NumeroCapitulo:D2}";

                            listaCapitulosObras.Add(capObra);
                        }
                    }

                    return (listaCapitulosObras, cap);
                }
                catch (TaskCanceledException)
                {
                    return (new List<CapitulosObra>(), new CapitulosObra());
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "BuscarCapituloObrasPorIdAsync(int idObra, int chapterId, bool numeroCapitulos, bool paginasCapitulo)");
                    return (new List<CapitulosObra>(), new CapitulosObra());
                }
            }
        }

        public async Task<List<CapitulosObra>> BuscarCapituloObrasPorIdsAsync(List<int> numCapitulos, int idObra)
        {
            using (var _context = _contextFactory.CreateDbContext())
            {
                try
                {
                    ef.Models.CapitulosObra[] capitulosObrasDB;

                    if (numCapitulos.Count > 0 && idObra != 0)
                    {
                        capitulosObrasDB = await (from c in _context.CapitulosObras
                                                  where numCapitulos.Contains(c.CpoNumeroCapitulo) && c.ObsId == idObra
                                                  select c).ToArrayAsync(_usuarioService.Token);
                    }
                    else
                    {
                        capitulosObrasDB = await (from c in _context.CapitulosObras
                                                  select c).ToArrayAsync(_usuarioService.Token);
                    }

                    List<CapitulosObra> listaCapitulosObras = new List<CapitulosObra>();

                    foreach (var capObra in capitulosObrasDB.ToDomainList())
                    {
                        listaCapitulosObras.Add(capObra);
                    }

                    return listaCapitulosObras;
                }
                catch (TaskCanceledException)
                {
                    return new List<CapitulosObra>();
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "BuscarCapituloObrasPorIdsAsync(List<int> numCapitulos, int idObra)");
                    return new List<CapitulosObra>();
                }
            }
        }

        public async Task<List<CapitulosObra>> BuscarCapituloObraPorIdAsync(int idCapitulo)
        {
            using (var _context = _contextFactory.CreateDbContext())
            {
                try
                {
                    ef.Models.CapitulosObra[] capitulosObrasDB;

                    if (idCapitulo != 0)
                    {
                        capitulosObrasDB = await (from c in _context.CapitulosObras
                                                  where c.CpoId == idCapitulo
                                                  select c).ToArrayAsync(_usuarioService.Token);
                    }
                    else
                    {
                        capitulosObrasDB = await (from c in _context.CapitulosObras
                                                  select c).ToArrayAsync(_usuarioService.Token);
                    }

                    List<CapitulosObra> listaCapitulosObras = new List<CapitulosObra>();

                    foreach (var capObra in capitulosObrasDB.ToDomainList())
                    {
                        listaCapitulosObras.Add(capObra);
                    }

                    return listaCapitulosObras;
                }
                catch (TaskCanceledException)
                {
                    return new List<CapitulosObra>();
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "BuscarCapituloObraPorIdAsync(int idCapitulo)");
                    return new List<CapitulosObra>();
                }
            }
        }

        public async Task<bool> CadastrarRemoverCapitulosAsync(List<CapitulosObra> listaCapitulosObra, List<CapitulosObra> listaCapitulosObraRemover)
        {
            using (var _context = _contextFactory.CreateDbContext())
            {
                using var transaction = await _context.Database.BeginTransactionAsync(_usuarioService.Token);

                try
                {
                    foreach (var capObra in listaCapitulosObra)
                    {
                        var capObraDB = capObra.ToEntity();

                        _context.Entry(capObraDB).State = capObraDB.CpoId == 0 ? EntityState.Added : EntityState.Modified;
                        await _context.SaveChangesAsync(_usuarioService.Token);

                        foreach (var paginaObra in capObra.ListaPaginas)
                        {
                            paginaObra.CapituloId = capObraDB.CpoId;

                            var pagObraDB = paginaObra.ToEntity();

                            _context.Entry(pagObraDB).State = pagObraDB.PgcId == 0 ? EntityState.Added : EntityState.Modified;
                            await _context.SaveChangesAsync(_usuarioService.Token);
                        }
                    }

                    foreach (var capObra in listaCapitulosObraRemover)
                    {
                        var capObraDB = capObra.ToEntity();

                        var paginasCapDB = await (from pg in _context.PaginasCapitulos
                                                  where pg.CpoId == capObra.Id
                                                  select pg).ToArrayAsync(_usuarioService.Token);

                        foreach (var pagObra in paginasCapDB)
                        {
                            _context.Entry(pagObra).State = EntityState.Deleted;
                            await _context.SaveChangesAsync(_usuarioService.Token);

                            var comentariosDB = await (from c in _context.Comentarios
                                                       where c.CpoId == capObra.Id
                                                       select c).ToArrayAsync(_usuarioService.Token);

                            foreach (var comentarioDB in comentariosDB)
                            {
                                bool sucesso = await _comentarioRepository.ExcluirComentarioAsync(comentarioDB.CtsId);

                                if (!sucesso)
                                {
                                    await transaction.RollbackAsync(_usuarioService.Token);
                                    return false;
                                }
                            }
                        }

                        _context.Entry(capObraDB).State = EntityState.Deleted;
                        await _context.SaveChangesAsync(_usuarioService.Token);
                    }

                    int idObra = listaCapitulosObra.Count > 0 ? listaCapitulosObra.First().ObraId : listaCapitulosObraRemover.First().ObraId;

                    ef.Models.Obra obraDB = await (from o in _context.Obras where o.ObsId == idObra select o).FirstOrDefaultAsync();

                    if (obraDB != null)
                    {
                        obraDB.ObsDataAtualizacao = DateTime.Now;

                        _context.Entry(obraDB).State = EntityState.Modified;
                        await _context.SaveChangesAsync(_usuarioService.Token);
                    }

                    await transaction.CommitAsync(_usuarioService.Token);
                    return true;
                }
                catch (TaskCanceledException)
                {
                    return false;
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "CadastrarRemoverCapitulosAsync(List<CapitulosObra> listaCapitulosObra, List<CapitulosObra> listaCapitulosObraRemover)");
                    await transaction.RollbackAsync(_usuarioService.Token);
                    return false;
                }
            }
        }
    }
}