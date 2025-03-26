using Microsoft.EntityFrameworkCore;
using Readit.Core.Domain;
using Readit.Core.Repositories;
using Readit.Core.Services;
using Readit.Data.Context;
using Readit.Infra.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ef = Readit.Data;
using Readit.Data.Mappers;
using Readit.Data.Models;

namespace Readit.Data.Repositories
{
    public class ComentarioRepository : IComentarioRepository
    {
        private readonly IDbContextFactory<ReaditContext> _contextFactory;
        private readonly ILoggingService _logger;
        private readonly IUsuarioService _usuarioService;

        public ComentarioRepository(IDbContextFactory<ReaditContext> contextFactory, ILoggingService logger, IUsuarioService usuarioService)
        {
            _contextFactory = contextFactory;
            _logger = logger;
            _usuarioService = usuarioService;
        }

        public async Task<List<Comentarios>> BuscarComentariosObraAsync(int idObra, int? idCapitulo)
        {
            using (var _context = _contextFactory.CreateDbContext())
            {
                try
                {
                    var comentariosDB = await (from c in _context.Comentarios
                                               join u in _context.Usuarios on c.UsuId equals u.UsuId
                                               join i in _context.Imagens on u.ImgId equals i.ImgId into imgJoin
                                               from img in imgJoin.DefaultIfEmpty()
                                               where !_context.RespostasComentarios.Any(rc => rc.ResId == c.CtsId) && ((c.ObsId == idObra && idCapitulo == null && c.CpoId == null) || (c.ObsId == idObra && idCapitulo != null && c.CpoId == idCapitulo))
                                               select new
                                               {
                                                   Id = c.CtsId,
                                                   Comentario = c.CtsComentario,
                                                   Data = c.CtsData,
                                                   DataAtualizacao = c.CtsDataAtualizacao,
                                                   ImagemUsuario = img.ImgImagem,
                                                   ApelidoUsuario = u.UsuApelido,
                                                   IdObra = c.ObsId,
                                                   IdCapitulo = c.CpoId,
                                                   IdUsuario = c.UsuId,
                                                   Likes = (from ac in _context.AvaliacoesComentarios
                                                            join a in _context.Avaliacoes on ac.AvaId equals a.AvaId
                                                            where ac.CtsId == c.CtsId && a.AvaAvaliacao == "Like"
                                                            select ac).Count(),
                                                   Dislikes = (from ac in _context.AvaliacoesComentarios
                                                               join a in _context.Avaliacoes on ac.AvaId equals a.AvaId
                                                               where ac.CtsId == c.CtsId && a.AvaAvaliacao == "Dislike"
                                                               select ac).Count(),
                                                   Respostas = (from rc in _context.RespostasComentarios
                                                                join r in _context.Comentarios on rc.ResId equals r.CtsId
                                                                join ur in _context.Usuarios on r.UsuId equals ur.UsuId
                                                                join ir in _context.Imagens on ur.ImgId equals ir.ImgId into imgResJoin
                                                                from imgRes in imgResJoin.DefaultIfEmpty()
                                                                where rc.CtsId == c.CtsId
                                                                select new
                                                                {
                                                                    Id = r.CtsId,
                                                                    Comentario = r.CtsComentario,
                                                                    Data = r.CtsData,
                                                                    DataAtualizacao = r.CtsDataAtualizacao,
                                                                    ImagemUsuario = imgRes.ImgImagem,
                                                                    ApelidoUsuario = ur.UsuApelido,
                                                                    IdObra = r.ObsId,
                                                                    IdCapitulo = r.CpoId,
                                                                    IdUsuario = r.UsuId,
                                                                    Likes = (from ac in _context.AvaliacoesComentarios
                                                                             join a in _context.Avaliacoes on ac.AvaId equals a.AvaId
                                                                             where ac.CtsId == r.CtsId && a.AvaAvaliacao == "Like"
                                                                             select ac).Count(),
                                                                    Dislikes = (from ac in _context.AvaliacoesComentarios
                                                                                join a in _context.Avaliacoes on ac.AvaId equals a.AvaId
                                                                                where ac.CtsId == r.CtsId && a.AvaAvaliacao == "Dislike"
                                                                                select ac).Count()
                                                                }).ToList()
                                               }).ToListAsync(_usuarioService.Token);

                    List<Comentarios> listaComentarios = new List<Comentarios>();

                    foreach (var comentario in ComentarioMapper.ToDomainListDynamic(comentariosDB))
                    {
                        listaComentarios.Add(comentario);
                    }

                    return listaComentarios;
                }
                catch (TaskCanceledException)
                {
                    return new List<Comentarios>();
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "BuscarComentariosObraAsync(int idObra, int? idCapitulo)");
                    return new List<Comentarios>();
                }
            }
        }

        public async Task<(bool, int)> CadastrarComentarioAsync(Comentarios comentario)
        {
            using (var _context = _contextFactory.CreateDbContext())
            {
                using var transaction = await _context.Database.BeginTransactionAsync(_usuarioService.Token);

                try
                {
                    var comentarioDB = comentario.ToEntity();

                    _context.Entry(comentarioDB).State = EntityState.Added;
                    await _context.SaveChangesAsync(_usuarioService.Token);

                    if (comentario.Pai != null)
                    {
                        ef.Models.RespostasComentario respostaComentarioDB = new ef.Models.RespostasComentario
                        {
                            CtsId = comentario.Pai.Id,
                            ResId = comentarioDB.CtsId,
                        };

                        _context.Entry(respostaComentarioDB).State = EntityState.Added;
                        await _context.SaveChangesAsync(_usuarioService.Token);
                    }

                    await transaction.CommitAsync(_usuarioService.Token);
                    return (true, comentarioDB.CtsId);
                }
                catch (TaskCanceledException)
                {
                    return (false, 0);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "CadastrarComentarioAsync(Comentarios comentario)");
                    await transaction.RollbackAsync(_usuarioService.Token);
                    return (false, 0);
                }
            }
        }

        public async Task<bool> ConsultarLikesDeslikesUsuarioAsync(Comentarios comentario, string tipoAvaliacao)
        {
            using (var _context = _contextFactory.CreateDbContext())
            {
                if (_usuarioService.UsuarioLogado == null)
                    return false;

                try
                {
                    var avaliacaoUsuario = await (from ac in _context.AvaliacoesComentarios
                                                  join a in _context.Avaliacoes on ac.AvaId equals a.AvaId
                                                  where a.AvaAvaliacao == tipoAvaliacao && ac.CtsId == comentario.Id && ac.UsuId == _usuarioService.UsuarioLogado.Id
                                                  select ac).FirstOrDefaultAsync(_usuarioService.Token);

                    return avaliacaoUsuario == null ? true : false;
                }
                catch (TaskCanceledException)
                {
                    return false;
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "ConsultarLikesDeslikesUsuarioAsync(Comentarios comentario, string tipoAvaliacao)");
                    return false;
                }
            }
        }

        public async Task<bool> CadastrarRemoverAvaliacaoComentarioAsync(Comentarios comentario, string tipoAvaliacao, string tipoAcao)
        {
            using (var _context = _contextFactory.CreateDbContext())
            {
                using var transaction = await _context.Database.BeginTransactionAsync(_usuarioService.Token);

                try
                {
                    if (_usuarioService.UsuarioLogado == null)
                        return false;

                    var avaliacao = await (from a in _context.Avaliacoes
                                           where a.AvaAvaliacao == tipoAvaliacao
                                           select a).FirstOrDefaultAsync(_usuarioService.Token);

                    ef.Models.AvaliacoesComentario avComent = new AvaliacoesComentario();

                    if (tipoAcao.Equals("Adicionar"))
                    {
                        avComent.CtsId = comentario.Id;
                        avComent.AvaId = avaliacao.AvaId;
                        avComent.UsuId = _usuarioService.UsuarioLogado.Id;
                    }
                    else
                    {
                        avComent = await (from ac in _context.AvaliacoesComentarios
                                          where ac.AvaId == avaliacao.AvaId && ac.UsuId == _usuarioService.UsuarioLogado.Id && ac.CtsId == comentario.Id
                                          select ac).FirstOrDefaultAsync(_usuarioService.Token);
                    }

                    _context.Entry(avComent).State = tipoAcao.Equals("Adicionar") ? EntityState.Added : EntityState.Deleted;
                    await _context.SaveChangesAsync(_usuarioService.Token);

                    await transaction.CommitAsync(_usuarioService.Token);
                    return true;
                }
                catch (TaskCanceledException)
                {
                    return false;
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "CadastrarRemoverAvaliacaoComentarioAsync(Comentarios comentario, string tipoAvaliacao, string tipoAcao)");
                    await transaction.RollbackAsync(_usuarioService.Token);
                    return false;
                }
            }
        }
    }
}