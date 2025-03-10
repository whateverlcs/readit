using Microsoft.EntityFrameworkCore;
using Readit.Core.Domain;
using Readit.Core.Repositories;
using Readit.Data.Context;
using Readit.Data.Mappers;
using Readit.Infra.Logging;
using ef = Readit.Data;

namespace Readit.Data.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly IDbContextFactory<ReaditContext> _contextFactory;
        private readonly ILoggingService _logger;

        public UsuarioRepository(IDbContextFactory<ReaditContext> contextFactory, ILoggingService logger)
        {
            _contextFactory = contextFactory;
            _logger = logger;
        }

        public async Task<List<Usuario>> BuscarUsuarioPorEmailAsync(string email)
        {
            using (var _context = _contextFactory.CreateDbContext())
            {
                try
                {
                    ef.Models.Usuario[] usuDB;

                    if (!string.IsNullOrEmpty(email))
                    {
                        usuDB = await (from u in _context.Usuarios
                                       where u.UsuEmail == email
                                       select u).ToArrayAsync();
                    }
                    else
                    {
                        usuDB = await (from u in _context.Usuarios
                                       select u).ToArrayAsync();
                    }

                    List<Usuario> listaUsuarios = new List<Usuario>();

                    foreach (var usu in usuDB.ToDomainList())
                    {
                        listaUsuarios.Add(usu);
                    }

                    return listaUsuarios;
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "BuscarUsuarioPorEmailAsync(string email)");
                    return new List<Usuario>();
                }
            }
        }

        public async Task<bool> CadastrarUsuarioAsync(Usuario usuario, Imagens imagem, TipoVisualizacaoObraUsuario? tipoVisualizacaoObra)
        {
            using (var _context = _contextFactory.CreateDbContext())
            {
                using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    ef.Models.Usuario usuarioDB = new ef.Models.Usuario();
                    ef.Models.Imagen imagemDB = new ef.Models.Imagen();
                    ef.Models.TipoVisualizacaoObraUsuario tipoVisualizacaoObraUsuario = new ef.Models.TipoVisualizacaoObraUsuario();

                    if (usuario.Id != 0)
                    {
                        var usuarioUpdate = await (from o in _context.Usuarios
                                                   where o.UsuId == usuario.Id
                                                   select o).FirstOrDefaultAsync();

                        usuarioUpdate.UsuNome = usuario.Nome;
                        usuarioUpdate.UsuApelido = usuario.Apelido;
                        usuarioUpdate.UsuSenha = usuario.Senha;

                        if (imagem != null && imagem.Id != 0)
                        {
                            var imagemUpdate = await (from i in _context.Imagens
                                                      where i.ImgId == imagem.Id
                                                      select i).FirstOrDefaultAsync();

                            imagemUpdate.ImgImagem = imagem.Imagem;
                            imagemUpdate.ImgFormato = imagem.Formato;
                            imagemUpdate.ImgDataAtualizacao = DateTime.Now;
                            imagemDB = imagemUpdate;
                        }

                        var tipoVisualizacaoUpdate = await (from tvou in _context.TipoVisualizacaoObraUsuarios
                                                            where tvou.UsuId == tipoVisualizacaoObra.UsuarioId
                                                            select tvou).FirstOrDefaultAsync();

                        if (tipoVisualizacaoUpdate != null)
                        {
                            tipoVisualizacaoUpdate.TvoId = tipoVisualizacaoObra.TipoVisualizacaoObraId;
                        }
                        else
                        {
                            tipoVisualizacaoUpdate = tipoVisualizacaoObra.ToEntity();
                        }

                        _context.Entry(tipoVisualizacaoUpdate).State = tipoVisualizacaoUpdate.TvouId == 0 ? EntityState.Added : EntityState.Modified;
                        await _context.SaveChangesAsync();

                        usuarioDB = usuarioUpdate;
                    }
                    else
                    {
                        usuarioDB = usuario.ToEntity();
                        imagemDB = imagem.ToEntity();
                    }

                    if (imagem != null)
                    {
                        _context.Entry(imagemDB).State = imagemDB.ImgId == 0 ? EntityState.Added : EntityState.Modified;
                        await _context.SaveChangesAsync();

                        usuarioDB.ImgId = imagemDB.ImgId;
                    }

                    _context.Entry(usuarioDB).State = usuarioDB.UsuId == 0 ? EntityState.Added : EntityState.Modified;
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();
                    return true;
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "CadastrarUsuarioAsync(Usuario usuario, Imagens imagem)");
                    await transaction.RollbackAsync();
                    return false;
                }
            }
        }
    }
}