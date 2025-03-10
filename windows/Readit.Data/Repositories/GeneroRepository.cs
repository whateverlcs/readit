using Microsoft.EntityFrameworkCore;
using Readit.Core.Domain;
using Readit.Core.Repositories;
using Readit.Data.Context;
using Readit.Data.Mappers;
using Readit.Infra.Logging;
using ef = Readit.Data;

namespace Readit.Data.Repositories
{
    public class GeneroRepository : IGeneroRepository
    {
        private readonly IDbContextFactory<ReaditContext> _contextFactory;
        private readonly ILoggingService _logger;

        public GeneroRepository(IDbContextFactory<ReaditContext> contextFactory, ILoggingService logger)
        {
            _contextFactory = contextFactory;
            _logger = logger;
        }

        public async Task<List<Generos>> BuscarGenerosPorNomeAsync(string nomeGenero)
        {
            using (var _context = _contextFactory.CreateDbContext())
            {
                try
                {
                    ef.Models.Genero[] generosDB;

                    if (!string.IsNullOrEmpty(nomeGenero))
                    {
                        generosDB = await (from g in _context.Generos
                                           where g.GnsNome == nomeGenero
                                           select g).ToArrayAsync();
                    }
                    else
                    {
                        generosDB = await (from g in _context.Generos
                                           select g).ToArrayAsync();
                    }

                    List<Generos> listaGeneros = new List<Generos>();

                    foreach (var genero in generosDB.ToDomainList())
                    {
                        listaGeneros.Add(genero);
                    }

                    return listaGeneros;
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "BuscarGenerosPorNomeAsync(string nomeGenero)");
                    return new List<Generos>();
                }
            }
        }

        public async Task<List<Generos>> BuscarGenerosPorObraAsync(int? idObra)
        {
            using (var _context = _contextFactory.CreateDbContext())
            {
                try
                {
                    ef.Models.Genero[] generosDB;

                    if (idObra != null && idObra != 0)
                    {
                        generosDB = await (from g in _context.Generos
                                           join og in _context.ObrasGeneros on g.GnsId equals og.GnsId
                                           where og.ObsId == idObra
                                           select g).ToArrayAsync();
                    }
                    else
                    {
                        generosDB = await (from g in _context.Generos
                                           select g).ToArrayAsync();
                    }

                    List<Generos> listaGeneros = new List<Generos>();

                    foreach (var genero in generosDB.ToDomainList())
                    {
                        listaGeneros.Add(genero);
                    }

                    return listaGeneros;
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "BuscarGenerosPorObraAsync(int? idObra)");
                    return new List<Generos>();
                }
            }
        }

        public async Task<bool> CadastrarGeneroAsync(Generos genero)
        {
            using (var _context = _contextFactory.CreateDbContext())
            {
                using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    var generoDB = genero.ToEntity();

                    _context.Entry(generoDB).State = generoDB.GnsId == 0 ? EntityState.Added : EntityState.Modified;
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();
                    return true;
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "CadastrarGeneroAsync(Generos genero)");
                    await transaction.RollbackAsync();
                    return false;
                }
            }
        }
    }
}