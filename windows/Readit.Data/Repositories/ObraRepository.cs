using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Readit.Core.Domain;
using Readit.Core.Repositories;
using Readit.Core.Services;
using Readit.Data.Context;
using Readit.Data.Mappers;
using Readit.Data.Models;
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

        public async Task<PostagensObras> BuscarDadosObraPorIdAsync(int idObra)
        {
            try
            {
                using (var connection = new SqlConnection(_contextFactory.CreateDbContext().Database.GetConnectionString()))
                {
                    await connection.OpenAsync(_usuarioService.Token);

                    var sql = @"
                        SELECT
                            o.obs_id AS Id,
                            o.obs_nomeObra AS NomeObra,
                            i.img_imagem AS Imagem,
                            o.obs_status AS Status,
                            o.obs_tipo AS Tipo,
                            o.obs_descricao AS Descricao,
                            (
                                SELECT STRING_AGG(g.gns_nome, ',')
                                FROM ObrasGeneros og
                                JOIN Generos g ON og.gns_id = g.gns_id
                                WHERE og.obs_id = @IdObra
                            ) AS Genres
                        FROM Obras o
                        JOIN Imagens i ON o.img_id = i.img_id
                        WHERE o.obs_id = @IdObra";

                    var parameters = new { IdObra = idObra };

                    var obraDB = await connection.QueryFirstOrDefaultAsync(sql, parameters);

                    if (obraDB == null)
                        return new PostagensObras();

                    var resultado = new
                    {
                        obraDB.Id,
                        obraDB.NomeObra,
                        obraDB.Imagem,
                        obraDB.Status,
                        obraDB.Tipo,
                        obraDB.Descricao,
                        Genres = ((string)obraDB.Genres)?.Split(',')?.ToList() ?? new List<string>()
                    };

                    return PostagensObrasMapper.MapCadastroEdicaoObras(resultado);
                }
            }
            catch (TaskCanceledException)
            {
                return new PostagensObras();
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"BuscarDadosObraPorIdAsync({idObra})");
                return new PostagensObras();
            }
        }

        public async Task<DetalhesObra> BuscarDetalhesObraAsync(string nomeObra)
        {
            if (_usuarioService.UsuarioLogado == null)
                return new DetalhesObra();

            using (var _context = _contextFactory.CreateDbContext())
            {
                try
                {
                    var obraDB = await _context.Obras
                        .AsNoTracking()
                        .Where(o => o.ObsNomeObra == nomeObra)
                        .Select(o => new
                        {
                            o.ObsId,
                            o.ObsNomeObra,
                            o.ObsDescricao,
                            o.ObsStatus,
                            o.ObsTipo,
                            o.ObsDataPublicacao,
                            o.ObsDataAtualizacao,

                            Usuario = _context.Usuarios
                                .Where(u => u.UsuId == o.UsuId)
                                .Select(u => u.UsuApelido)
                                .FirstOrDefault(),

                            Imagem = _context.Imagens
                                .Where(i => i.ImgId == o.ImgId)
                                .Select(i => i.ImgImagem)
                                .FirstOrDefault(),

                            MediaNota = _context.AvaliacoesObras
                                .Where(a => a.ObsId == o.ObsId)
                                .Average(a => (double?)a.AvoNota) ?? 0,

                            Views = _context.VisualizacoesObras
                                .Where(v => v.ObsId == o.ObsId)
                                .Sum(v => (int?)v.VsoViews) ?? 0,

                            Generos = _context.ObrasGeneros
                                .Where(og => og.ObsId == o.ObsId)
                                .Join(_context.Generos, og => og.GnsId, g => g.GnsId, (og, g) => g.GnsNome)
                                .Distinct()
                                .ToArray(),

                            Bookmark = _context.BookmarksUsuarios
                                .Any(b => b.ObsId == o.ObsId && b.UsuId == _usuarioService.UsuarioLogado.Id),

                            AvaliacaoUsuario = _context.AvaliacoesObras
                                .Where(a => a.ObsId == o.ObsId && a.UsuId == _usuarioService.UsuarioLogado.Id)
                                .Select(a => (double?)a.AvoNota)
                                .FirstOrDefault() ?? 0,

                            Capitulos = _context.CapitulosObras
                                .Where(c => c.ObsId == o.ObsId)
                                .OrderByDescending(c => c.CpoNumeroCapitulo)
                                .Select(c => new
                                {
                                    Id = c.CpoId,
                                    Numero = c.CpoNumeroCapitulo,
                                    DataPublicacao = c.CpoDataPublicacao.HasValue
                                        ? c.CpoDataPublicacao.Value.ToString("MMMM d, yyyy")
                                        : null
                                })
                                .ToArray()
                        })
                        .FirstOrDefaultAsync(_usuarioService.Token);

                    if (obraDB == null)
                        return new DetalhesObra();

                    var obraFormatada = new
                    {
                        IdObra = obraDB.ObsId,
                        NomeObra = obraDB.ObsNomeObra,
                        Descricao = obraDB.ObsDescricao,
                        Status = obraDB.ObsStatus,
                        Tipo = obraDB.ObsTipo,
                        DataPublicacao = obraDB.ObsDataPublicacao.HasValue
                            ? obraDB.ObsDataPublicacao.Value.ToString("MMMM d, yyyy")
                            : null,
                        DataAtualizacao = obraDB.ObsDataAtualizacao.HasValue
                            ? obraDB.ObsDataAtualizacao.Value.ToString("MMMM d, yyyy")
                            : null,
                        NomeUsuario = obraDB.Usuario,
                        obraDB.MediaNota,
                        obraDB.Views,
                        obraDB.Imagem,
                        obraDB.Generos,
                        obraDB.Bookmark,
                        obraDB.AvaliacaoUsuario,
                        obraDB.Capitulos
                    };

                    return DetalhesObraMapper.ToDomain(obraFormatada);
                }
                catch (TaskCanceledException)
                {
                    return new DetalhesObra();
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
            try
            {
                using (var connection = new SqlConnection(_contextFactory.CreateDbContext().Database.GetConnectionString()))
                {
                    await connection.OpenAsync(_usuarioService.Token);

                    var sql = @"
                        SELECT
                            o.obs_id AS Id,
                            o.obs_nomeObra AS NomeObra,
                            o.obs_status AS Status,
                            o.obs_tipo AS Tipo,
                            o.obs_dataPublicacao AS DataPublicacao,
                            o.obs_dataAtualizacao AS DataAtualizacao,
                            i.img_imagem AS Imagem,
                            (
                                SELECT AVG(CAST(a.avo_nota AS FLOAT))
                                FROM AvaliacoesObra a
                                WHERE a.obs_id = o.obs_id
                            ) AS Rating,
                            (
                                SELECT STRING_AGG(g.gns_nome, ',')
                                FROM ObrasGeneros og
                                JOIN Generos g ON og.gns_id = g.gns_id
                                WHERE og.obs_id = o.obs_id
                            ) AS Genres
                        FROM Obras o
                        LEFT JOIN Imagens i ON o.img_id = i.img_id
                        ORDER BY Rating DESC";

                    var obrasDB = await connection.QueryAsync(sql);

                    var resultados = obrasDB.Select(obra => new
                    {
                        obra.Id,
                        obra.NomeObra,
                        obra.Status,
                        obra.Tipo,
                        DataPublicacao = (DateTime?)obra.DataPublicacao,
                        DataAtualizacao = (DateTime?)obra.DataAtualizacao,
                        obra.Imagem,
                        Rating = (double?)obra.Rating ?? 0,
                        Genres = ((string)obra.Genres)?.Split(',')?.ToList() ?? new List<string>()
                    }).ToArray();

                    return PostagensObrasMapper.MapListagemObras(resultados);
                }
            }
            catch (TaskCanceledException)
            {
                return new List<PostagensObras>();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "BuscarListagemObrasAsync()");
                return new List<PostagensObras>();
            }
        }

        public async Task<List<PostagensObras>> BuscarObrasBookmarksAsync()
        {
            if (_usuarioService.UsuarioLogado == null)
                return new List<PostagensObras>();

            try
            {
                using (var connection = new SqlConnection(_contextFactory.CreateDbContext().Database.GetConnectionString()))
                {
                    await connection.OpenAsync(_usuarioService.Token);

                    var sql = @"
                        SELECT
                            o.obs_id AS Id,
                            o.obs_nomeObra AS NomeObra,
                            i.img_imagem AS Imagem,
                            o.obs_tipo AS Tipo
                        FROM Obras o
                        JOIN Imagens i ON o.img_id = i.img_id
                        JOIN BookmarksUsuario bu ON o.obs_id = bu.obs_id
                        WHERE bu.usu_id = @UsuarioId
                        GROUP BY o.obs_id, o.obs_nomeObra, o.obs_tipo, i.img_imagem, bu.usu_id";

                    var parameters = new { UsuarioId = _usuarioService.UsuarioLogado.Id };

                    var obrasDB = await connection.QueryAsync(sql, parameters);

                    var resultados = obrasDB.Select(obra => new
                    {
                        obra.Id,
                        obra.NomeObra,
                        obra.Imagem,
                        obra.Tipo
                    }).ToArray();

                    return PostagensObrasMapper.MapBookmarks(resultados);
                }
            }
            catch (TaskCanceledException)
            {
                return new List<PostagensObras>();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "BuscarObrasBookmarksAsync()");
                return new List<PostagensObras>();
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

                    using (var connection = new SqlConnection(_contextFactory.CreateDbContext().Database.GetConnectionString()))
                    {
                        await connection.OpenAsync(_usuarioService.Token);

                        var countSql = @"
                            SELECT COUNT(DISTINCT a.obs_id)
                            FROM AvaliacoesObra a
                            WHERE NOT EXISTS (
                                SELECT 1 FROM ObrasGeneros og
                                JOIN Generos g ON og.gns_id = g.gns_id
                                WHERE og.obs_id = a.obs_id
                                AND g.gns_nome IN @preferenciasUsuario
                            )";

                        var totalObrasComAvaliacao = await connection.ExecuteScalarAsync<int>(countSql, new { preferenciasUsuario });

                        var obrasSql = @"
                            SELECT
                                o.obs_nomeObra AS ObsNomeObra,
                                i.img_imagem AS ImgImagem,
                                AVG(a.avo_nota) AS Rating,
                                MAX(a.avo_dataAvaliacao) AS DataAvaliacao,
                                o.obs_tipo AS Tipo,
                                (
                                    SELECT STRING_AGG(g.gns_nome, ',')
                                    FROM ObrasGeneros og
                                    JOIN Generos g ON og.gns_id = g.gns_id
                                    WHERE og.obs_id = o.obs_id
                                ) AS Genres
                            FROM Obras o
                            JOIN Imagens i ON o.img_id = i.img_id
                            JOIN ObrasGeneros og ON o.obs_id = og.obs_id
                            JOIN Generos g ON og.gns_id = g.gns_id
                            JOIN AvaliacoesObra a ON o.obs_id = a.obs_id
                            WHERE NOT EXISTS (
                                SELECT 1 FROM ObrasGeneros og2
                                JOIN Generos g2 ON og2.gns_id = g2.gns_id
                                WHERE og2.obs_id = o.obs_id
                                AND g2.gns_nome IN @preferenciasUsuario
                            )
                            GROUP BY o.obs_id, o.obs_nomeObra, i.img_imagem, o.obs_tipo";

                        var obrasQuery = (await connection.QueryAsync(obrasSql, new { preferenciasUsuario }))
                            .Select(o => new
                            {
                                o.ObsNomeObra,
                                ImgImagem = (byte[])o.ImgImagem,
                                o.Rating,
                                Genres = ((string)o.Genres)?.Split(',')?.ToList() ?? new List<string>(),
                                o.DataAvaliacao,
                                o.Tipo
                            }).ToList();

                        List<DestaquesItem> todasObras = new();

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
                                    Genres = o.Genres,
                                    TipoNumber = o.Tipo
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
                                Filter = filtro.Key,
                                TipoNumber = o.TipoNumber
                            }));
                        }

                        return todasObras;
                    }
                }
                catch (TaskCanceledException)
                {
                    return new List<DestaquesItem>();
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
                        obrasDB = await (from o in _context.Obras where o.ObsId == idObra select o).ToArrayAsync(_usuarioService.Token);
                    }
                    else
                    {
                        obrasDB = await (from o in _context.Obras select o).ToArrayAsync(_usuarioService.Token);
                    }

                    List<Obras> listaObras = new List<Obras>();

                    foreach (var obra in obrasDB.ToDomainList())
                    {
                        listaObras.Add(obra);
                    }

                    return listaObras;
                }
                catch (TaskCanceledException)
                {
                    return new List<Obras>();
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
                        obrasDB = await (from o in _context.Obras where o.ObsNomeObra == nomeObra select o).ToArrayAsync(_usuarioService.Token);
                    }
                    else
                    {
                        obrasDB = await (from o in _context.Obras select o).ToArrayAsync(_usuarioService.Token);
                    }

                    List<Obras> listaObras = new List<Obras>();

                    foreach (var obra in obrasDB.ToDomainList())
                    {
                        listaObras.Add(obra);
                    }

                    return listaObras;
                }
                catch (TaskCanceledException)
                {
                    return new List<Obras>();
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
            try
            {
                var preferenciasUsuario = (await _preferenciaRepository.BuscarPreferenciasUsuarioAsync())
                                       .ConvertAll(p => p.Preferencia);

                using (var connection = new SqlConnection(_contextFactory.CreateDbContext().Database.GetConnectionString()))
                {
                    await connection.OpenAsync(_usuarioService.Token);

                    var sql = @"
                        WITH ObrasFiltradas AS (
                            SELECT o.obs_id, o.obs_nomeObra, o.obs_descricao, o.img_id
                            FROM Obras o
                            WHERE NOT EXISTS (
                                SELECT 1
                                FROM ObrasGeneros og
                                JOIN Generos g ON og.gns_id = g.gns_id
                                WHERE og.obs_id = o.obs_id
                                AND g.gns_nome IN @preferenciasUsuario
                            )
                            AND EXISTS (
                                SELECT 1
                                FROM CapitulosObra c
                                WHERE c.obs_id = o.obs_id
                            )
                        )
                        SELECT TOP 5
                            o.obs_nomeObra AS NomeObra,
                            o.obs_descricao AS Descricao,
                            i.img_imagem AS Imagem,
                            (
                                SELECT STRING_AGG(g.gns_nome, ', ')
                                FROM ObrasGeneros og
                                JOIN Generos g ON og.gns_id = g.gns_id
                                WHERE og.obs_id = o.obs_id
                            ) AS Generos,
                            (
                                SELECT MAX(c.cpo_numeroCapitulo)
                                FROM CapitulosObra c
                                WHERE c.obs_id = o.obs_id
                            ) AS UltimoCapitulo,
                            (
                                SELECT AVG(a.avo_nota)
                                FROM AvaliacoesObra a
                                WHERE a.obs_id = o.obs_id
                            ) AS MediaAvaliacoes
                        FROM ObrasFiltradas o
                        LEFT JOIN Imagens i ON o.img_id = i.img_id
                        ORDER BY MediaAvaliacoes DESC";

                    var obrasDB = await connection.QueryAsync(sql, new { preferenciasUsuario });

                    var resultados = obrasDB.Select(obra => new
                    {
                        obra.NomeObra,
                        obra.Descricao,
                        obra.Imagem,
                        Generos = ((string)obra.Generos)?.Split(", ") ?? Array.Empty<string>(),
                        obra.UltimoCapitulo
                    }).ToList();

                    return resultados.ToDomainList();
                }
            }
            catch (TaskCanceledException)
            {
                return new List<SlideshowItem>();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "BuscarObrasSlideShow()");
                return new List<SlideshowItem>();
            }
        }

        public async Task<List<PostagensObras>> BuscarObrasUltimasAtualizacoesAsync()
        {
            try
            {
                var preferenciasUsuario = (await _preferenciaRepository.BuscarPreferenciasUsuarioAsync())
                                       .ConvertAll(p => p.Preferencia);

                using (var connection = new SqlConnection(_contextFactory.CreateDbContext().Database.GetConnectionString()))
                {
                    await connection.OpenAsync(_usuarioService.Token);

                    var sql = @"
                        WITH ObrasFiltradas AS (
                            SELECT o.obs_id, o.obs_nomeObra, o.obs_status, o.obs_tipo, o.img_id
                            FROM Obras o
                            WHERE NOT EXISTS (
                                SELECT 1
                                FROM ObrasGeneros og
                                JOIN Generos g ON og.gns_id = g.gns_id
                                WHERE og.obs_id = o.obs_id
                                AND g.gns_nome IN @preferenciasUsuario
                            )
                        ),
                        UltimasAtualizacoes AS (
                            SELECT
                                o.obs_id,
                                MAX(c.cpo_dataPublicacao) AS UltimaAtualizacao
                            FROM ObrasFiltradas o
                            LEFT JOIN CapitulosObra c ON o.obs_id = c.obs_id
                            GROUP BY o.obs_id
                        )
                        SELECT
                            o.obs_id AS Id,
                            o.obs_nomeObra AS NomeObra,
                            o.obs_status AS Status,
                            o.obs_tipo AS Tipo,
                            i.img_imagem AS Imagem,
                            u.UltimaAtualizacao,
                            (
                                SELECT TOP 3
                                    c.cpo_id AS [Id],
                                    c.cpo_numeroCapitulo AS [Numero],
                                    c.cpo_dataPublicacao AS [Data]
                                FROM CapitulosObra c
                                WHERE c.obs_id = o.obs_id
                                ORDER BY c.cpo_numeroCapitulo DESC
                                FOR JSON PATH
                            ) AS CapitulosJson
                        FROM ObrasFiltradas o
                        JOIN UltimasAtualizacoes u ON o.obs_id = u.obs_id
                        LEFT JOIN Imagens i ON o.img_id = i.img_id
                        ORDER BY u.UltimaAtualizacao DESC";

                    var obrasDB = await connection.QueryAsync<dynamic>(sql, new { preferenciasUsuario });

                    var resultados = obrasDB.Select(obra => new
                    {
                        obra.Id,
                        obra.NomeObra,
                        obra.Status,
                        obra.Tipo,
                        obra.Imagem,
                        obra.UltimaAtualizacao,
                        Capitulos = ((IEnumerable<dynamic>)JsonConvert.DeserializeObject(obra.CapitulosJson ?? "[]"))
                        .Select(c => new
                        {
                            c.Id,
                            c.Numero,
                            c.Data
                        })
                    }).ToArray();

                    return PostagensObrasMapper.MapUltimasAtualizacoes(resultados);
                }
            }
            catch (TaskCanceledException)
            {
                return new List<PostagensObras>();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "BuscarObrasUltimasAtualizacoes()");
                return new List<PostagensObras>();
            }
        }

        public async Task<bool> CadastrarEditarObraAsync(Obras obra, Imagens imagem, List<Generos> listaGeneros)
        {
            using (var _context = _contextFactory.CreateDbContext())
            {
                using var transaction = await _context.Database.BeginTransactionAsync(_usuarioService.Token);

                try
                {
                    Obra? obraDB = null;
                    Imagen? imagemDB = null;
                    List<int> listaGenerosAdicionar = [];
                    List<ObrasGenero> listaGenerosRemover = [];

                    if (obra.Id != 0)
                    {
                        obraDB = await (from o in _context.Obras
                                        where o.ObsId == obra.Id
                                        select o).FirstAsync(_usuarioService.Token);

                        obraDB.ObsNomeObra = obra.NomeObra;
                        obraDB.ObsStatus = obra.Status;
                        obraDB.ObsTipo = obra.Tipo;
                        obraDB.ObsDescricao = obra.Descricao;
                        obraDB.ObsDataAtualizacao = DateTime.Now;

                        imagemDB = await (from i in _context.Imagens
                                          where i.ImgId == obra.ImagemId
                                          select i).FirstAsync(_usuarioService.Token);

                        imagemDB.ImgImagem = imagem.Imagem;
                        imagemDB.ImgFormato = imagem.Formato;
                        imagemDB.ImgTipo = imagem.Tipo;
                        imagemDB.ImgDataAtualizacao = DateTime.Now;

                        var generosDB = await (from og in _context.ObrasGeneros
                                               where obra.Id == og.ObsId
                                               select og).ToArrayAsync(_usuarioService.Token);

                        listaGenerosAdicionar = listaGeneros.Where(x => !generosDB.Select(g => g.GnsId).Contains(x.Id)).Select(g => g.Id).ToList();
                        listaGenerosRemover = generosDB.Where(x => !listaGeneros.Select(g => g.Id).Contains(x.GnsId)).ToList();
                    }
                    else
                    {
                        imagemDB = imagem.ToEntity();
                        obraDB = obra.ToEntity();
                    }

                    _context.Entry(imagemDB).State = imagemDB.ImgId == 0 ? EntityState.Added : EntityState.Modified;
                    await _context.SaveChangesAsync(_usuarioService.Token);

                    obraDB.ImgId = imagemDB.ImgId;

                    _context.Entry(obraDB).State = obraDB.ObsId == 0 ? EntityState.Added : EntityState.Modified;
                    await _context.SaveChangesAsync(_usuarioService.Token);

                    listaGenerosAdicionar = obra.Id == 0 ? listaGeneros.Select(g => g.Id).ToList() : listaGenerosAdicionar;

                    foreach (var genero in listaGenerosAdicionar)
                    {
                        var obrasGenero = new ef.Models.ObrasGenero
                        {
                            GnsId = genero,
                            ObsId = obraDB.ObsId
                        };

                        _context.Entry(obrasGenero).State = EntityState.Added;
                        await _context.SaveChangesAsync(_usuarioService.Token);
                    }

                    foreach (var obraGenero in listaGenerosRemover)
                    {
                        _context.Entry(obraGenero).State = EntityState.Deleted;
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
                    _logger.LogError(e, "CadastrarEditarObraAsync(Obras obra, Imagens imagem, List<Generos> listaGeneros)");
                    await transaction.RollbackAsync(_usuarioService.Token);
                    return false;
                }
            }
        }
    }
}