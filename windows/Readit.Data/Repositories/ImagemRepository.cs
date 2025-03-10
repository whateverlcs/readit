using Microsoft.EntityFrameworkCore;
using Readit.Core.Domain;
using Readit.Core.Repositories;
using Readit.Data.Context;
using Readit.Data.Mappers;
using Readit.Infra.Logging;
using ef = Readit.Data;

namespace Readit.Data.Repositories
{
    public class ImagemRepository : IImagemRepository
    {
        private readonly IDbContextFactory<ReaditContext> _contextFactory;
        private readonly ILoggingService _logger;

        public ImagemRepository(IDbContextFactory<ReaditContext> contextFactory, ILoggingService logger)
        {
            _contextFactory = contextFactory;
            _logger = logger;
        }

        public async Task<List<Imagens>> BuscarImagemPorIdAsync(int idImagem)
        {
            using (var _context = _contextFactory.CreateDbContext())
            {
                try
                {
                    ef.Models.Imagen[] imagensDB;

                    if (idImagem != 0)
                    {
                        imagensDB = await (from i in _context.Imagens
                                           where i.ImgId == idImagem
                                           select i).ToArrayAsync();
                    }
                    else
                    {
                        imagensDB = await (from i in _context.Imagens
                                           select i).ToArrayAsync();
                    }

                    List<Imagens> listaImagens = new List<Imagens>();

                    foreach (var img in imagensDB.ToDomainList())
                    {
                        listaImagens.Add(img);
                    }

                    return listaImagens;
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "BuscarImagemPorIdAsync(int idImagem)");
                    return new List<Imagens>();
                }
            }
        }
    }
}