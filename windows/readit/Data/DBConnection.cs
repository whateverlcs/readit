using EntityFramework_DB.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using readit.Controls;
using readit.Models;
using ef = EntityFramework_DB;

namespace readit.Data
{
    public class DBConnection
    {
        private readonly ReaditContext context = new ReaditContext();

        private ModelsTranslate md = new ModelsTranslate();

        private ControlLogs clog = new ControlLogs();

        public bool CadastrarUsuario(Usuario usuario, Imagens imagem, TipoVisualizacaoObraUsuario? tipoVisualizacaoObra)
        {
            using var transaction = context.Database.BeginTransaction();

            try
            {
                ef.Models.Usuario usuarioDB = new ef.Models.Usuario();
                ef.Models.Imagen imagemDB = new ef.Models.Imagen();
                ef.Models.TipoVisualizacaoObraUsuario tipoVisualizacaoObraUsuario = new ef.Models.TipoVisualizacaoObraUsuario();

                if (usuario.Id != 0)
                {
                    var usuarioUpdate = (from o in context.Usuarios where o.UsuId == usuario.Id select o).FirstOrDefault();

                    usuarioUpdate.UsuNome = usuario.Nome;
                    usuarioUpdate.UsuApelido = usuario.Apelido;
                    usuarioUpdate.UsuSenha = usuario.Senha;

                    if (imagem != null && imagem.Id != 0)
                    {
                        var imagemUpdate = (from i in context.Imagens where i.ImgId == imagem.Id select i).FirstOrDefault();

                        imagemUpdate.ImgImagem = imagem.Imagem;
                        imagemUpdate.ImgFormato = imagem.Formato;
                        imagemUpdate.ImgDataAtualizacao = DateTime.Now;
                        imagemDB = imagemUpdate;
                    }

                    var tipoVisualizacaoUpdate = (from tvou in context.TipoVisualizacaoObraUsuarios where tvou.UsuId == tipoVisualizacaoObra.UsuarioId select tvou).FirstOrDefault();

                    if (tipoVisualizacaoUpdate != null)
                    {
                        tipoVisualizacaoUpdate.TvoId = tipoVisualizacaoObra.TipoVisualizacaoObraId;
                    }
                    else
                    {
                        tipoVisualizacaoUpdate = md.TipoVisualizacaoObraUsuarioModelToDB(tipoVisualizacaoObra);
                    }

                    context.Entry(tipoVisualizacaoUpdate).State = tipoVisualizacaoUpdate.TvouId == 0 ? EntityState.Added : EntityState.Modified;
                    context.SaveChanges();

                    usuarioDB = usuarioUpdate;
                }
                else
                {
                    usuarioDB = md.UsuarioModelToDB(usuario);
                    imagemDB = md.ImagemModelToDB(imagem);
                }

                if (imagem != null)
                {
                    context.Entry(imagemDB).State = imagemDB.ImgId == 0 ? EntityState.Added : EntityState.Modified;
                    context.SaveChanges();

                    usuarioDB.ImgId = imagemDB.ImgId;
                }

                context.Entry(usuarioDB).State = usuarioDB.UsuId == 0 ? EntityState.Added : EntityState.Modified;
                context.SaveChanges();

                transaction.Commit();
                return true;
            }
            catch (Exception e)
            {
                clog.RealizarLogExcecao(e.ToString(), "CadastrarUsuario(Usuario usuario, Imagens imagem)");
                transaction.Rollback();
                return false;
            }
        }

        public List<Usuario> BuscarUsuarioPorEmail(string email)
        {
            try
            {
                ef.Models.Usuario[] usuDB;

                if (!string.IsNullOrEmpty(email))
                {
                    usuDB = (from u in context.Usuarios where u.UsuEmail == email select u).ToArray();
                }
                else
                {
                    usuDB = (from u in context.Usuarios select u).ToArray();
                }

                List<Usuario> listaUsuarios = new List<Usuario>();

                foreach (var usu in md.UsuariosDBToModel(usuDB.ToArray()))
                {
                    listaUsuarios.Add(usu);
                }

                return listaUsuarios;
            }
            catch (Exception e)
            {
                clog.RealizarLogExcecao(e.ToString(), "BuscarUsuarioPorEmail(string email)");
                return new List<Usuario>();
            }
        }

        public List<Obras> BuscarObrasPorId(int? idObra)
        {
            try
            {
                ef.Models.Obra[] obrasDB;

                if (idObra != null && idObra != 0)
                {
                    obrasDB = (from o in context.Obras where o.ObsId == idObra select o).ToArray();
                }
                else
                {
                    obrasDB = (from o in context.Obras select o).ToArray();
                }

                List<Obras> listaObras = new List<Obras>();

                foreach (var obra in md.ObrasDBToModel(obrasDB.ToArray()))
                {
                    listaObras.Add(obra);
                }

                return listaObras;
            }
            catch (Exception e)
            {
                clog.RealizarLogExcecao(e.ToString(), "BuscarObrasPorId(int? idObra)");
                return new List<Obras>();
            }
        }

        public List<CapitulosObra> BuscarCapituloObrasPorIds(List<int> numCapitulos, int idObra)
        {
            try
            {
                ef.Models.CapitulosObra[] capitulosObrasDB;

                if (numCapitulos.Count > 0 && idObra != 0)
                {
                    capitulosObrasDB = (from c in context.CapitulosObras where numCapitulos.Contains(c.CpoNumeroCapitulo) && c.ObsId == idObra select c).ToArray();
                }
                else
                {
                    capitulosObrasDB = (from c in context.CapitulosObras select c).ToArray();
                }

                List<CapitulosObra> listaCapitulosObras = new List<CapitulosObra>();

                foreach (var capObra in md.CapitulosObrasDBToModel(capitulosObrasDB.ToArray()))
                {
                    listaCapitulosObras.Add(capObra);
                }

                return listaCapitulosObras;
            }
            catch (Exception e)
            {
                clog.RealizarLogExcecao(e.ToString(), "BuscarCapituloObrasPorIds(List<int> idsObra)");
                return new List<CapitulosObra>();
            }
        }

        public List<PaginasCapitulo> BuscarPaginasCapituloPorIds(List<int> idsCap)
        {
            try
            {
                ef.Models.PaginasCapitulo[] paginasCapituloDB;

                if (idsCap.Count > 0)
                {
                    paginasCapituloDB = (from p in context.PaginasCapitulos where idsCap.Contains(p.CpoId) select p).ToArray();
                }
                else
                {
                    paginasCapituloDB = (from p in context.PaginasCapitulos select p).ToArray();
                }

                List<PaginasCapitulo> listaPaginasCapitulo = new List<PaginasCapitulo>();

                foreach (var pagCap in md.PaginasCapituloDBToModel(paginasCapituloDB.ToArray()))
                {
                    listaPaginasCapitulo.Add(pagCap);
                }

                return listaPaginasCapitulo;
            }
            catch (Exception e)
            {
                clog.RealizarLogExcecao(e.ToString(), "BuscarPaginasCapituloPorIds(List<int> idsCap)");
                return new List<PaginasCapitulo>();
            }
        }

        public List<Obras> BuscarObrasPorNome(string nomeObra)
        {
            try
            {
                ef.Models.Obra[] obrasDB;

                if (!string.IsNullOrEmpty(nomeObra))
                {
                    obrasDB = (from o in context.Obras where o.ObsNomeObra == nomeObra select o).ToArray();
                }
                else
                {
                    obrasDB = (from o in context.Obras select o).ToArray();
                }

                List<Obras> listaObras = new List<Obras>();

                foreach (var obra in md.ObrasDBToModel(obrasDB.ToArray()))
                {
                    listaObras.Add(obra);
                }

                return listaObras;
            }
            catch (Exception e)
            {
                clog.RealizarLogExcecao(e.ToString(), "BuscarObrasPorNome()");
                return new List<Obras>();
            }
        }

        public List<Generos> BuscarGenerosPorNome(string nomeGenero)
        {
            try
            {
                ef.Models.Genero[] generosDB;

                if (!string.IsNullOrEmpty(nomeGenero))
                {
                    generosDB = (from g in context.Generos where g.GnsNome == nomeGenero select g).ToArray();
                }
                else
                {
                    generosDB = (from g in context.Generos select g).ToArray();
                }

                List<Generos> listaGeneros = new List<Generos>();

                foreach (var genero in md.GenerosDBToModel(generosDB.ToArray()))
                {
                    listaGeneros.Add(genero);
                }

                return listaGeneros;
            }
            catch (Exception e)
            {
                clog.RealizarLogExcecao(e.ToString(), "BuscarGenerosPorNome(string nomeGenero)");
                return new List<Generos>();
            }
        }

        public List<Generos> BuscarGenerosPorObra(int? idObra)
        {
            try
            {
                ef.Models.Genero[] generosDB;

                if (idObra != null && idObra != 0)
                {
                    generosDB = (from g in context.Generos
                                 join og in context.ObrasGeneros on g.GnsId equals og.GnsId
                                 where og.ObsId == idObra
                                 select g).ToArray();
                }
                else
                {
                    generosDB = (from g in context.Generos select g).ToArray();
                }

                List<Generos> listaGeneros = new List<Generos>();

                foreach (var genero in md.GenerosDBToModel(generosDB.ToArray()))
                {
                    listaGeneros.Add(genero);
                }

                return listaGeneros;
            }
            catch (Exception e)
            {
                clog.RealizarLogExcecao(e.ToString(), "BuscarGenerosPorObra(int? idObra)");
                return new List<Generos>();
            }
        }

        public List<Imagens> BuscarImagemPorId(int idImagem)
        {
            try
            {
                ef.Models.Imagen[] imagensDB;

                if (idImagem != 0)
                {
                    imagensDB = (from i in context.Imagens where i.ImgId == idImagem select i).ToArray();
                }
                else
                {
                    imagensDB = (from i in context.Imagens select i).ToArray();
                }

                List<Imagens> listaImagens = new List<Imagens>();

                foreach (var img in md.ImagensDBToModel(imagensDB.ToArray()))
                {
                    listaImagens.Add(img);
                }

                return listaImagens;
            }
            catch (Exception e)
            {
                clog.RealizarLogExcecao(e.ToString(), "BuscarImagemPorId(int idImagem)");
                return new List<Imagens>();
            }
        }

        public List<TipoVisualizacaoObra> BuscarTiposVisualizacaoObraPorId(int idTipoVisualizacao)
        {
            try
            {
                ef.Models.TipoVisualizacaoObra[] tipoVisualizacaoObraDB;

                if (idTipoVisualizacao != 0)
                {
                    tipoVisualizacaoObraDB = (from tvo in context.TipoVisualizacaoObras where tvo.TvoId == idTipoVisualizacao select tvo).ToArray();
                }
                else
                {
                    tipoVisualizacaoObraDB = (from tvo in context.TipoVisualizacaoObras select tvo).ToArray();
                }

                List<TipoVisualizacaoObra> listaTiposVisualizacaoObra = new List<TipoVisualizacaoObra>();

                foreach (var tvo in md.TipoVisualizacaoObraDBToModel(tipoVisualizacaoObraDB.ToArray()))
                {
                    listaTiposVisualizacaoObra.Add(tvo);
                }

                return listaTiposVisualizacaoObra;
            }
            catch (Exception e)
            {
                clog.RealizarLogExcecao(e.ToString(), "BuscarTiposVisualizacaoObraPorId(int idTipoVisualizacao)");
                return new List<TipoVisualizacaoObra>();
            }
        }

        public List<TipoVisualizacaoObraUsuario> BuscarTiposVisualizacaoObraUsuarioPorId(int idUsuario)
        {
            try
            {
                ef.Models.TipoVisualizacaoObraUsuario[] tipoVisualizacaoObraUsuarioDB;

                if (idUsuario != 0)
                {
                    tipoVisualizacaoObraUsuarioDB = (from tvou in context.TipoVisualizacaoObraUsuarios where tvou.UsuId == idUsuario select tvou).ToArray();
                }
                else
                {
                    tipoVisualizacaoObraUsuarioDB = (from tvou in context.TipoVisualizacaoObraUsuarios select tvou).ToArray();
                }

                List<TipoVisualizacaoObraUsuario> listaTiposVisualizacaoObraUsuario = new List<TipoVisualizacaoObraUsuario>();

                foreach (var tvou in md.TipoVisualizacaoObraUsuarioDBToModel(tipoVisualizacaoObraUsuarioDB.ToArray()))
                {
                    listaTiposVisualizacaoObraUsuario.Add(tvou);
                }

                return listaTiposVisualizacaoObraUsuario;
            }
            catch (Exception e)
            {
                clog.RealizarLogExcecao(e.ToString(), "BuscarTiposVisualizacaoObraUsuarioPorId(int idUsuario)");
                return new List<TipoVisualizacaoObraUsuario>();
            }
        }

        public List<SlideshowItem> BuscarObrasSlideShow()
        {
            try
            {
                var obrasDB = (from o in context.Obras
                               join i in context.Imagens on o.ImgId equals i.ImgId
                               join og in context.ObrasGeneros on o.ObsId equals og.ObsId
                               join g in context.Generos on og.GnsId equals g.GnsId
                               join cpo in context.CapitulosObras on o.ObsId equals cpo.ObsId
                               join a in context.AvaliacoesObras on o.ObsId equals a.ObsId
                               group new { o, i, cpo, g, a } by new { o.ObsId, o.ObsNomeObra, o.ObsDescricao, i.ImgImagem } into obraGroup
                               orderby obraGroup.Average(x => x.a.AvoNota) descending // Ordenar pelas maiores médias de avaliação
                               select new
                               {
                                   NomeObra = obraGroup.Key.ObsNomeObra,
                                   UltimoCapitulo = obraGroup.Max(x => x.cpo.CpoNumeroCapitulo),
                                   Descricao = obraGroup.Key.ObsDescricao,
                                   Generos = obraGroup.Select(x => x.g.GnsNome).Distinct().ToArray(),
                                   Imagem = obraGroup.Key.ImgImagem
                               }).Take(5).ToArray();

                List<SlideshowItem> listaObras = new List<SlideshowItem>();

                foreach (var obra in md.ObrasSlideShowDBToModel(obrasDB.ToArray()))
                {
                    listaObras.Add(obra);
                }

                return listaObras;
            }
            catch (Exception e)
            {
                clog.RealizarLogExcecao(e.ToString(), "BuscarObrasSlideShow()");
                return new List<SlideshowItem>();
            }
        }

        public List<PostagensObras> BuscarObrasUltimasAtualizacoes()
        {
            try
            {
                var obrasDB = (from o in context.Obras
                               join i in context.Imagens on o.ImgId equals i.ImgId
                               join cpo in context.CapitulosObras on o.ObsId equals cpo.ObsId
                               group new { o, i, cpo } by new { o.ObsId, o.ObsNomeObra, o.ObsStatus, i.ImgImagem } into obraGroup
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
                               }).ToArray();

                List<PostagensObras> listaObras = new List<PostagensObras>();

                foreach (var obra in md.ObrasUltimasAtualizacoesDBToModel(obrasDB.ToArray()))
                {
                    listaObras.Add(obra);
                }

                return listaObras;
            }
            catch (Exception e)
            {
                clog.RealizarLogExcecao(e.ToString(), "BuscarObrasUltimasAtualizacoes()");
                return new List<PostagensObras>();
            }
        }

        public List<PostagensObras> BuscarObrasBookmarks()
        {
            try
            {
                var obrasDB = (from o in context.Obras
                               join i in context.Imagens on o.ImgId equals i.ImgId
                               join bu in context.BookmarksUsuarios on o.ObsId equals bu.ObsId
                               group new { o, i, bu } by new { o.ObsId, o.ObsNomeObra, i.ImgImagem, bu.UsuId } into obraGroup
                               where obraGroup.Key.UsuId == Global.UsuarioLogado.Id
                               select new
                               {
                                   Id = obraGroup.Key.ObsId,
                                   NomeObra = obraGroup.Key.ObsNomeObra,
                                   Imagem = obraGroup.Key.ImgImagem
                               }).ToArray();

                List<PostagensObras> listaObras = new List<PostagensObras>();

                foreach (var obra in md.ObrasBookmarksDBToModel(obrasDB.ToArray()))
                {
                    listaObras.Add(obra);
                }

                return listaObras;
            }
            catch (Exception e)
            {
                clog.RealizarLogExcecao(e.ToString(), "BuscarObrasBookmarks()");
                return new List<PostagensObras>();
            }
        }

        public List<DestaquesItem> BuscarObrasEmDestaque()
        {
            try
            {
                var filtros = new Dictionary<string, Func<DateTime?>>
                {
                    { "Semanal", () => DateTime.Now.AddDays(-7) },
                    { "Mensal", () => DateTime.Now.AddMonths(-1) },
                    { "Todos", () => null }
                };

                List<DestaquesItem> todasObras = new();
                int totalObrasComAvaliacao = context.AvaliacoesObras.Select(a => a.ObsId).Distinct().Count();

                // Consulta única para reduzir acessos ao banco
                var obrasQuery = (from o in context.Obras
                                  join i in context.Imagens on o.ImgId equals i.ImgId
                                  join og in context.ObrasGeneros on o.ObsId equals og.ObsId
                                  join g in context.Generos on og.GnsId equals g.GnsId
                                  join a in context.AvaliacoesObras on o.ObsId equals a.ObsId
                                  group new { o, i, g, a } by new { o.ObsId, o.ObsNomeObra, i.ImgImagem } into obraGroup
                                  select new
                                  {
                                      obraGroup.Key.ObsNomeObra,
                                      obraGroup.Key.ImgImagem,
                                      Rating = obraGroup.Average(x => x.a.AvoNota),
                                      Genres = obraGroup.Select(x => x.g.GnsNome).Distinct().ToList(),
                                      DataAvaliacao = obraGroup.Max(x => x.a.AvoDataAvaliacao) // Última avaliação da obra
                                  }).ToList();

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
                            .Take(10 - topObras.Count)
                            .ToList();

                        // Conversão necessária antes de adicionar a lista
                        topObras.AddRange(query.Select(o => new DestaquesItem
                        {
                            Title = o.ObsNomeObra,
                            ImageByte = o.ImgImagem,
                            Rating = o.Rating,
                            Genres = o.Genres
                        }));

                        // Removendo obras repetidas e garantindo diversidade
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
                clog.RealizarLogExcecao(e.ToString(), "BuscarObrasEmDestaque()");
                return new List<DestaquesItem>();
            }
        }

        public List<PostagensObras> BuscarListagemObras()
        {
            try
            {
                var obrasDB = (from o in context.Obras
                               join i in context.Imagens on o.ImgId equals i.ImgId
                               join og in context.ObrasGeneros on o.ObsId equals og.ObsId
                               join g in context.Generos on og.GnsId equals g.GnsId
                               join a in context.AvaliacoesObras on o.ObsId equals a.ObsId into avaliacoes
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
                               }).OrderByDescending(o => o.Rating).ToArray();

                List<PostagensObras> listaObras = new List<PostagensObras>();

                foreach (var obra in md.ListagemObrasDBToModel(obrasDB.ToArray()))
                {
                    listaObras.Add(obra);
                }

                return listaObras;
            }
            catch (Exception e)
            {
                clog.RealizarLogExcecao(e.ToString(), "BuscarListagemObras()");
                return new List<PostagensObras>();
            }
        }

        public DetalhesObra BuscarDetalhesObra(string nomeObra)
        {
            try
            {
                var obrasDB = (from o in context.Obras
                               join i in context.Imagens on o.ImgId equals i.ImgId
                               join u in context.Usuarios on o.UsuId equals u.UsuId
                               join ao in context.AvaliacoesObras on o.ObsId equals ao.ObsId into avaliacoes
                               from avg in avaliacoes.DefaultIfEmpty()
                               join vo in context.VisualizacoesObras on o.ObsId equals vo.ObsId into visualizacoes
                               from vs in visualizacoes.DefaultIfEmpty()
                               join og in context.ObrasGeneros on o.ObsId equals og.ObsId
                               join g in context.Generos on og.GnsId equals g.GnsId
                               join cpo in context.CapitulosObras on o.ObsId equals cpo.ObsId
                               join bku in context.BookmarksUsuarios on new { o.ObsId, UsuId = Global.UsuarioLogado.Id } equals new { bku.ObsId, bku.UsuId } into bookmarks
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
                                   MediaNota = obraGroup.Average(x => x.avg != null ? x.avg.AvoNota : 0), // Verifica se a nota não é nula
                                   Views = obraGroup.Select(x => x.vs != null ? x.vs.VsoViews : 0).FirstOrDefault(), // Verifica se há visualizações
                                   Imagem = obraGroup.Key.ImgImagem,
                                   Generos = obraGroup.Select(x => x.g.GnsNome).Distinct().ToArray(),
                                   Bookmark = obraGroup.Any(x => x.bkm != null),
                                   Capitulos = (from cpo in context.CapitulosObras
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
                               }).FirstOrDefault();

                return md.DetalhesObraDBToModel(obrasDB);
            }
            catch (Exception e)
            {
                clog.RealizarLogExcecao(e.ToString(), "BuscarDetalhesObra(string nomeObra)");
                return new DetalhesObra();
            }
        }

        public bool CadastrarObra(Obras obra, Imagens imagem, List<Generos> listaGeneros)
        {
            using var transaction = context.Database.BeginTransaction();

            try
            {
                var imagemDB = md.ImagemModelToDB(imagem);
                var obraDB = md.ObraModelToDB(obra);

                context.Imagens.Add(imagemDB);
                context.SaveChanges();

                obraDB.ImgId = imagemDB.ImgId;

                context.Obras.Add(obraDB);
                context.SaveChanges();

                foreach (var genero in listaGeneros)
                {
                    context.ObrasGeneros.Add(new ef.Models.ObrasGenero
                    {
                        GnsId = genero.Id,
                        ObsId = obraDB.ObsId
                    });
                    context.SaveChanges();
                }

                transaction.Commit();
                return true;
            }
            catch (Exception e)
            {
                clog.RealizarLogExcecao(e.ToString(), "CadastrarObra(Obras obra, Imagens imagem, List<Generos> listaGeneros)");
                transaction.Rollback();
                return false;
            }
        }

        public bool CadastrarGenero(Generos genero)
        {
            using var transaction = context.Database.BeginTransaction();

            try
            {
                var generoDB = md.GeneroModelToDB(genero);

                context.Entry(generoDB).State = generoDB.GnsId == 0 ? EntityState.Added : EntityState.Modified;
                context.SaveChanges();

                transaction.Commit();
                return true;
            }
            catch (Exception e)
            {
                clog.RealizarLogExcecao(e.ToString(), "CadastrarGenero(Generos genero)");
                transaction.Rollback();
                return false;
            }
        }

        public (bool, string) CadastrarRemoverBookmark(BookmarksUsuario bookmarkUsuario)
        {
            using var transaction = context.Database.BeginTransaction();

            try
            {
                var bookmarkDB = md.BookmarksUsuarioModelToDB(bookmarkUsuario);

                ef.Models.BookmarksUsuario bkuDB = (from bku in context.BookmarksUsuarios where bku.ObsId == bookmarkDB.ObsId && bku.UsuId == bookmarkDB.UsuId select bku).FirstOrDefault();

                var bkEntry = bkuDB != null ? bkuDB : bookmarkDB;

                context.Entry(bkEntry).State = bkuDB != null ? EntityState.Deleted : EntityState.Added;
                context.SaveChanges();

                transaction.Commit();
                return (true, (bkuDB != null ? "Removido" : "Adicionado"));
            }
            catch (Exception e)
            {
                clog.RealizarLogExcecao(e.ToString(), "CadastrarRemoverBookmark(BookmarksUsuario bookmarkUsuario)");
                transaction.Rollback();
                return (false, "");
            }
        }

        public bool AtualizarViewObra(string nomeObra)
        {
            using var transaction = context.Database.BeginTransaction();

            try
            {
                ef.Models.Obra obraDB = (from o in context.Obras where o.ObsNomeObra == nomeObra select o).FirstOrDefault();
                ef.Models.VisualizacoesObra voDB = (from vo in context.VisualizacoesObras where vo.ObsId == obraDB.ObsId select vo).FirstOrDefault();

                if (voDB == null)
                {
                    voDB = new ef.Models.VisualizacoesObra
                    {
                        ObsId = obraDB.ObsId,
                        VsoViews = 1
                    };
                }
                else
                {
                    voDB.VsoViews++;
                }

                context.Entry(voDB).State = voDB.VsoId != 0 ? EntityState.Modified : EntityState.Added;
                context.SaveChanges();

                transaction.Commit();
                return true;
            }
            catch (Exception e)
            {
                clog.RealizarLogExcecao(e.ToString(), "AtualizarViewObra(int idObra)");
                transaction.Rollback();
                return false;
            }
        }

        public bool AtualizarRating(int obraId, double rating)
        {
            using var transaction = context.Database.BeginTransaction();

            try
            {
                ef.Models.AvaliacoesObra avDB = (from av in context.AvaliacoesObras where av.ObsId == obraId && av.UsuId == Global.UsuarioLogado.Id select av).FirstOrDefault();

                if (avDB == null)
                {
                    avDB = new ef.Models.AvaliacoesObra
                    {
                        ObsId = obraId,
                        UsuId = Global.UsuarioLogado.Id,
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

                context.Entry(avDB).State = avDB.AvoId != 0 ? EntityState.Modified : EntityState.Added;
                context.SaveChanges();

                transaction.Commit();
                return true;
            }
            catch (Exception e)
            {
                clog.RealizarLogExcecao(e.ToString(), "AtualizarRating(int obraId, double rating)");
                transaction.Rollback();
                return false;
            }
        }

        public bool CadastrarCapitulos(List<CapitulosObra> listaCapitulosObra)
        {
            using var transaction = context.Database.BeginTransaction();

            try
            {
                foreach (var capObra in listaCapitulosObra)
                {
                    var capObraDB = md.CapitulosObraDBToModel(capObra);

                    context.Entry(capObraDB).State = capObraDB.CpoId == 0 ? EntityState.Added : EntityState.Modified;
                    context.SaveChanges();

                    foreach (var paginaObra in capObra.ListaPaginas)
                    {
                        paginaObra.CapituloId = capObraDB.CpoId;

                        var pagObraDB = md.PaginasCapituloDBToModel(paginaObra);

                        context.Entry(pagObraDB).State = pagObraDB.PgcId == 0 ? EntityState.Added : EntityState.Modified;
                        context.SaveChanges();
                    }
                }

                int idObra = listaCapitulosObra.First().ObraId;

                ef.Models.Obra obraDB = (from o in context.Obras where o.ObsId == idObra select o).First();

                obraDB.ObsDataAtualizacao = DateTime.Now;

                context.Entry(obraDB).State = EntityState.Modified;
                context.SaveChanges();

                transaction.Commit();
                return true;
            }
            catch (Exception e)
            {
                clog.RealizarLogExcecao(e.ToString(), "CadastrarCapitulos(List<CapitulosObra> listaCapitulosObra)");
                transaction.Rollback();
                return false;
            }
        }

        public void RealizarConexaoDB()
        {
            try
            {
                ef.Global.ConnectionString = App.config.GetConnectionString("ConnectionString");
            }
            catch (Exception e)
            {
                clog.RealizarLogExcecao(e.ToString(), "RealizarConexaoDB()");
            }
        }

        public bool TestarConexaoDB()
        {
            try
            {
                return context.Database.CanConnect();
            }
            catch (Exception e)
            {
                clog.RealizarLogExcecao(e.ToString(), "TestarConexaoDB()");
                return false;
            }
        }
    }
}