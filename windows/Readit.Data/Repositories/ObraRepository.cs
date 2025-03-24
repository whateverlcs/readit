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
    public class ObraRepository : IObraRepository
    {
        private readonly IDbContextFactory<ReaditContext> _contextFactory;
        private readonly ILoggingService _logger;
        private readonly IUsuarioService _usuarioService;
        private readonly IPreferenciasRepository _preferenciaRepository;

        public ObraRepository(IDbContextFactory<ReaditContext> contextFactory, ILoggingService logger, IUsuarioService usuarioService, IPreferenciasRepository preferenciaRepository)
        {
            _contextFactory = contextFactory;
            _logger = logger;
            _usuarioService = usuarioService;
            _preferenciaRepository = preferenciaRepository;
        }

        public async Task<DetalhesObra> BuscarDetalhesObraAsync(string nomeObra)
        {
            using (var _context = _contextFactory.CreateDbContext())
            {
                try
                {
                    var obrasDB = await (from o in _context.Obras
                                         join i in _context.Imagens on o.ImgId equals i.ImgId
                                         join u in _context.Usuarios on o.UsuId equals u.UsuId
                                         join ao in _context.AvaliacoesObras on o.ObsId equals ao.ObsId into avaliacoes
                                         from avg in avaliacoes.DefaultIfEmpty()
                                         join vo in _context.VisualizacoesObras on o.ObsId equals vo.ObsId into visualizacoes
                                         from vs in visualizacoes.DefaultIfEmpty()
                                         join og in _context.ObrasGeneros on o.ObsId equals og.ObsId
                                         join g in _context.Generos on og.GnsId equals g.GnsId
                                         join cpo in _context.CapitulosObras on o.ObsId equals cpo.ObsId
                                         join bku in _context.BookmarksUsuarios on new { o.ObsId, UsuId = _usuarioService.UsuarioLogado.Id } equals new { bku.ObsId, bku.UsuId } into bookmarks
                                         from bkm in bookmarks.DefaultIfEmpty()
                                         group new { o, i, u, avg, vs, g, bkm } by new { o.ObsId, o.ObsNomeObra, o.ObsDescricao, o.ObsStatus, o.ObsTipo, o.ObsDataPublicacao, o.ObsDataAtualizacao, u.UsuApelido, i.ImgImagem } into obraGroup
                                         where obraGroup.Key.ObsNomeObra == nomeObra
                                         select new
                                         {
                                             IdObra = obraGroup.Key.ObsId,
                                             NomeObra = obraGroup.Key.ObsNomeObra,
                                             Descricao = obraGroup.Key.ObsDescricao,
                                             Status = obraGroup.Key.ObsStatus,
                                             Tipo = obraGroup.Key.ObsTipo,
                                             DataPublicacao = obraGroup.Key.ObsDataPublicacao.HasValue ? obraGroup.Key.ObsDataPublicacao.Value.ToString("MMMM d, yyyy") : null,
                                             DataAtualizacao = obraGroup.Key.ObsDataAtualizacao.HasValue ? obraGroup.Key.ObsDataAtualizacao.Value.ToString("MMMM d, yyyy") : null,
                                             NomeUsuario = obraGroup.Key.UsuApelido,
                                             MediaNota = obraGroup.Average(x => x.avg != null ? x.avg.AvoNota : 0),
                                             Views = obraGroup.Select(x => x.vs != null ? x.vs.VsoViews : 0).FirstOrDefault(),
                                             Imagem = obraGroup.Key.ImgImagem,
                                             Generos = obraGroup.Select(x => x.g.GnsNome).Distinct().ToArray(),
                                             Bookmark = obraGroup.Any(x => x.bkm != null),
                                             Capitulos = (from cpo in _context.CapitulosObras
                                                          where cpo.ObsId == obraGroup.Key.ObsId
                                                          orderby cpo.CpoNumeroCapitulo descending
                                                          select new
                                                          {
                                                              Id = cpo.CpoId,
                                                              Numero = cpo.CpoNumeroCapitulo,
                                                              DataPublicacao = cpo.CpoDataPublicacao.HasValue
                                                                               ? cpo.CpoDataPublicacao.Value.ToString("MMMM d, yyyy")
                                                                               : null
                                                          }).ToArray()
                                         }).FirstOrDefaultAsync();

                    return DetalhesObraMapper.ToDomain(obrasDB);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "BuscarDetalhesObraAsync(string nomeObra)");
                    return new DetalhesObra();
                }
            }
        }

        public async Task<List<PostagensObras>> BuscarListagemObrasAsync()
        {
            using (var _context = _contextFactory.CreateDbContext())
            {
                try
                {
                    var obrasDB = await (from o in _context.Obras
                                         join i in _context.Imagens on o.ImgId equals i.ImgId
                                         join og in _context.ObrasGeneros on o.ObsId equals og.ObsId
                                         join g in _context.Generos on og.GnsId equals g.GnsId
                                         join a in _context.AvaliacoesObras on o.ObsId equals a.ObsId into avaliacoes
                                         from avg in avaliacoes.DefaultIfEmpty()
                                         group new { o, i, g, avg } by new { o.ObsId, o.ObsNomeObra, i.ImgImagem, o.ObsStatus, o.ObsTipo, o.ObsDataPublicacao, o.ObsDataAtualizacao } into obraGroup
                                         select new
                                         {
                                             Id = obraGroup.Key.ObsId,
                                             NomeObra = obraGroup.Key.ObsNomeObra,
                                             Imagem = obraGroup.Key.ImgImagem,
                                             Rating = obraGroup.Average(x => x.avg != null ? x.avg.AvoNota : 0),
                                             Status = obraGroup.Key.ObsStatus,
                                             Tipo = obraGroup.Key.ObsTipo,
                                             Genres = obraGroup.Select(x => x.g.GnsNome).Distinct().ToList(),
                                             DataPublicacao = obraGroup.Key.ObsDataPublicacao,
                                             DataAtualizacao = obraGroup.Key.ObsDataAtualizacao,
                                         }).OrderByDescending(o => o.Rating).ToArrayAsync();

                    List<PostagensObras> listaObras = new List<PostagensObras>();

                    foreach (var obra in PostagensObrasMapper.MapListagemObras(obrasDB))
                    {
                        listaObras.Add(obra);
                    }

                    return listaObras;
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "BuscarListagemObrasAsync()");
                    return new List<PostagensObras>();
                }
            }
        }

        public async Task<List<PostagensObras>> BuscarObrasBookmarksAsync()
        {
            using (var _context = _contextFactory.CreateDbContext())
            {
                try
                {
                    var obrasDB = await (from o in _context.Obras
                                         join i in _context.Imagens on o.ImgId equals i.ImgId
                                         join bu in _context.BookmarksUsuarios on o.ObsId equals bu.ObsId
                                         group new { o, i, bu } by new { o.ObsId, o.ObsNomeObra, i.ImgImagem, bu.UsuId } into obraGroup
                                         where obraGroup.Key.UsuId == _usuarioService.UsuarioLogado.Id
                                         select new
                                         {
                                             Id = obraGroup.Key.ObsId,
                                             NomeObra = obraGroup.Key.ObsNomeObra,
                                             Imagem = obraGroup.Key.ImgImagem
                                         }).ToArrayAsync();

                    List<PostagensObras> listaObras = new List<PostagensObras>();

                    foreach (var obra in PostagensObrasMapper.MapBookmarks(obrasDB))
                    {
                        listaObras.Add(obra);
                    }

                    return listaObras;
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "BuscarObrasBookmarksAsync()");
                    return new List<PostagensObras>();
                }
            }
        }

        public async Task<List<DestaquesItem>> BuscarObrasEmDestaqueAsync()
        {
            using (var _context = _contextFactory.CreateDbContext())
            {
                try
                {
                    var preferenciasUsuario = (await _preferenciaRepository.BuscarPreferenciasUsuarioAsync()).ConvertAll(p => p.Preferencia);

                    var filtros = new Dictionary<string, Func<DateTime?>>
                    {
                        { "Semanal", () => DateTime.Now.AddDays(-7) },
                        { "Mensal", () => DateTime.Now.AddMonths(-1) },
                        { "Todos", () => null }
                    };

                    List<DestaquesItem> todasObras = new();
                    int totalObrasComAvaliacao = await (from a in _context.AvaliacoesObras
                                                        join og in _context.ObrasGeneros on a.ObsId equals og.ObsId
                                                        join g in _context.Generos on og.GnsId equals g.GnsId
                                                        group g by a.ObsId into obraGroup
                                                        where !obraGroup.Any(genero => preferenciasUsuario.Contains(genero.GnsNome))
                                                        select obraGroup.Key).Distinct().CountAsync();

                    var obrasQuery = await (from o in _context.Obras
                                            join i in _context.Imagens on o.ImgId equals i.ImgId
                                            join og in _context.ObrasGeneros on o.ObsId equals og.ObsId
                                            join g in _context.Generos on og.GnsId equals g.GnsId
                                            join a in _context.AvaliacoesObras on o.ObsId equals a.ObsId
                                            group new { o, i, g, a } by new { o.ObsId, o.ObsNomeObra, i.ImgImagem } into obraGroup
                                            where !obraGroup.Select(x => x.g.GnsNome).Any(genero => preferenciasUsuario.Contains(genero))
                                            select new
                                            {
                                                obraGroup.Key.ObsNomeObra,
                                                obraGroup.Key.ImgImagem,
                                                Rating = obraGroup.Average(x => x.a.AvoNota),
                                                Genres = obraGroup.Select(x => x.g.GnsNome).Distinct().ToList(),
                                                DataAvaliacao = obraGroup.Max(x => x.a.AvoDataAvaliacao)
                                            }).ToListAsync();

                    foreach (var filtro in filtros)
                    {
                        List<DestaquesItem> topObras = new();
                        int rank = 1;
                        DateTime? dataInicio = filtro.Value();

                        while (topObras.Count < 10 && (filtro.Key == "Todos" || dataInicio > DateTime.MinValue))
                        {
                            var query = obrasQuery
                                .Where(o => filtro.Key == "Todos" || o.DataAvaliacao >= dataInicio)
                                .OrderByDescending(o => o.Rating)
                                .ToList();

                            topObras.AddRange(query.Select(o => new DestaquesItem
                            {
                                Title = o.ObsNomeObra,
                                ImageByte = o.ImgImagem,
                                Rating = o.Rating,
                                Genres = o.Genres
                            }));

                            topObras = topObras
                                .GroupBy(x => x.Title)
                                .Select(g => g.First())
                                .ToList();

                            if (topObras.Count >= totalObrasComAvaliacao)
                                break;

                            if (topObras.Count < 10 && filtro.Key != "Todos")
                            {
                                dataInicio = filtro.Key == "Semanal" ? dataInicio.Value.AddDays(-7) : dataInicio.Value.AddMonths(-1);
                            }
                        }

                        todasObras.AddRange(topObras.Select(o => new DestaquesItem
                        {
                            Rank = rank++,
                            Title = o.Title,
                            ImageByte = o.ImageByte,
                            Genres = o.Genres,
                            Rating = o.Rating,
                            Filter = filtro.Key
                        }));
                    }

                    return todasObras;
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "BuscarObrasEmDestaque()");
                    return new List<DestaquesItem>();
                }
            }
        }

        public async Task<List<Obras>> BuscarObrasPorIdAsync(int? idObra)
        {
            using (var _context = _contextFactory.CreateDbContext())
            {
                try
                {
                    ef.Models.Obra[] obrasDB;

                    if (idObra != null && idObra != 0)
                    {
                        obrasDB = await (from o in _context.Obras where o.ObsId == idObra select o).ToArrayAsync();
                    }
                    else
                    {
                        obrasDB = await (from o in _context.Obras select o).ToArrayAsync();
                    }

                    List<Obras> listaObras = new List<Obras>();

                    foreach (var obra in obrasDB.ToDomainList())
                    {
                        listaObras.Add(obra);
                    }

                    return listaObras;
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "BuscarObrasPorId(int? idObra)");
                    return new List<Obras>();
                }
            }
        }

        public async Task<List<Obras>> BuscarObrasPorNomeAsync(string nomeObra)
        {
            using (var _context = _contextFactory.CreateDbContext())
            {
                try
                {
                    ef.Models.Obra[] obrasDB;

                    if (!string.IsNullOrEmpty(nomeObra))
                    {
                        obrasDB = await (from o in _context.Obras where o.ObsNomeObra == nomeObra select o).ToArrayAsync();
                    }
                    else
                    {
                        obrasDB = await (from o in _context.Obras select o).ToArrayAsync();
                    }

                    List<Obras> listaObras = new List<Obras>();

                    foreach (var obra in obrasDB.ToDomainList())
                    {
                        listaObras.Add(obra);
                    }

                    return listaObras;
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "BuscarObrasPorNome()");
                    return new List<Obras>();
                }
            }
        }

        public async Task<List<SlideshowItem>> BuscarObrasSlideShowAsync()
        {
            using (var _context = _contextFactory.CreateDbContext())
            {
                try
                {
                    var preferenciasUsuario = (await _preferenciaRepository.BuscarPreferenciasUsuarioAsync()).ConvertAll(p => p.Preferencia);

                    var obrasDB = await (from o in _context.Obras
                                         join i in _context.Imagens on o.ImgId equals i.ImgId
                                         join og in _context.ObrasGeneros on o.ObsId equals og.ObsId
                                         join g in _context.Generos on og.GnsId equals g.GnsId
                                         join cpo in _context.CapitulosObras on o.ObsId equals cpo.ObsId
                                         join a in _context.AvaliacoesObras on o.ObsId equals a.ObsId
                                         group new { o, i, cpo, g, a } by new { o.ObsId, o.ObsNomeObra, o.ObsDescricao, i.ImgImagem } into obraGroup
                                         where !obraGroup.Select(x => x.g.GnsNome).Any(genero => preferenciasUsuario.Contains(genero))
                                         orderby obraGroup.Average(x => x.a.AvoNota) descending
                                         select new
                                         {
                                             NomeObra = obraGroup.Key.ObsNomeObra,
                                             UltimoCapitulo = obraGroup.Max(x => x.cpo.CpoNumeroCapitulo),
                                             Descricao = obraGroup.Key.ObsDescricao,
                                             Generos = obraGroup.Select(x => x.g.GnsNome).Distinct().ToArray(),
                                             Imagem = obraGroup.Key.ImgImagem
                                         }).Take(5).ToArrayAsync();

                    List<SlideshowItem> listaObras = new List<SlideshowItem>();

                    foreach (var obra in obrasDB.ToDomainList())
                    {
                        listaObras.Add(obra);
                    }

                    return listaObras;
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "BuscarObrasSlideShow()");
                    return new List<SlideshowItem>();
                }
            }
        }

        public async Task<List<PostagensObras>> BuscarObrasUltimasAtualizacoesAsync()
        {
            using (var _context = _contextFactory.CreateDbContext())
            {
                try
                {
                    var preferenciasUsuario = (await _preferenciaRepository.BuscarPreferenciasUsuarioAsync()).ConvertAll(p => p.Preferencia);

                    var obrasDB = await (from o in _context.Obras
                                         join og in _context.ObrasGeneros on o.ObsId equals og.ObsId
                                         join g in _context.Generos on og.GnsId equals g.GnsId
                                         join i in _context.Imagens on o.ImgId equals i.ImgId
                                         join cpo in _context.CapitulosObras on o.ObsId equals cpo.ObsId
                                         group new { o, i, cpo, g } by new { o.ObsId, o.ObsNomeObra, o.ObsStatus, i.ImgImagem } into obraGroup
                                         where !obraGroup.Select(x => x.g.GnsNome).Any(genero => preferenciasUsuario.Contains(genero))
                                         orderby obraGroup.Max(x => x.cpo.CpoDataPublicacao) descending
                                         select new
                                         {
                                             Id = obraGroup.Key.ObsId,
                                             NomeObra = obraGroup.Key.ObsNomeObra,
                                             Status = obraGroup.Key.ObsStatus,
                                             Imagem = obraGroup.Key.ImgImagem,
                                             Capitulos = obraGroup.OrderByDescending(x => x.cpo.CpoNumeroCapitulo)
                                                                   .Take(3)
                                                                   .Select(x => new
                                                                   {
                                                                       Id = x.cpo.CpoId,
                                                                       Numero = x.cpo.CpoNumeroCapitulo,
                                                                       Data = x.cpo.CpoDataPublicacao
                                                                   }).ToArray()
                                         }).ToArrayAsync();

                    List<PostagensObras> listaObras = new List<PostagensObras>();

                    foreach (var obra in PostagensObrasMapper.MapUltimasAtualizacoes(obrasDB))
                    {
                        listaObras.Add(obra);
                    }

                    return listaObras;
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "BuscarObrasUltimasAtualizacoes()");
                    return new List<PostagensObras>();
                }
            }
        }

        public async Task<bool> CadastrarObraAsync(Obras obra, Imagens imagem, List<Generos> listaGeneros)
        {
            using (var _context = _contextFactory.CreateDbContext())
            {
                using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    var imagemDB = imagem.ToEntity();
                    var obraDB = obra.ToEntity();

                    await _context.Imagens.AddAsync(imagemDB);
                    await _context.SaveChangesAsync();

                    obraDB.ImgId = imagemDB.ImgId;

                    await _context.Obras.AddAsync(obraDB);
                    await _context.SaveChangesAsync();

                    foreach (var genero in listaGeneros)
                    {
                        await _context.ObrasGeneros.AddAsync(new ef.Models.ObrasGenero
                        {
                            GnsId = genero.Id,
                            ObsId = obraDB.ObsId
                        });
                        await _context.SaveChangesAsync();
                    }

                    await transaction.CommitAsync();
                    return true;
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "CadastrarObra(Obras obra, Imagens imagem, List<Generos> listaGeneros)");
                    await transaction.RollbackAsync();
                    return false;
                }
            }
        }
    }
}