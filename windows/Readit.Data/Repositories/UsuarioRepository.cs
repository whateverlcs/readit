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
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly IDbContextFactory<ReaditContext> _contextFactory;
        private readonly ILoggingService _logger;
        private readonly IUsuarioService _usuarioService;

        public UsuarioRepository(IDbContextFactory<ReaditContext> contextFactory, ILoggingService logger, IUsuarioService usuarioService)
        {
            _contextFactory = contextFactory;
            _logger = logger;
            _usuarioService = usuarioService;
        }

        public async Task<List<Usuario>> BuscarUsuarioPorEmailAsync(string email)
        {
            using (var _context = _contextFactory.CreateDbContext())
            {
                try
                {
                    var usuDB = await (from u in _context.Usuarios
                                       join i in _context.Imagens on u.ImgId equals i.ImgId
                                       where u.UsuEmail == email
                                       select new
                                       {
                                           Id = u.UsuId,
                                           Nome = u.UsuNome,
                                           Apelido = u.UsuApelido,
                                           Email = u.UsuEmail,
                                           Senha = u.UsuSenha,
                                           Administrador = u.UsuAdministrador,
                                           IdImagem = u.ImgId,
                                           Imagem = i.ImgImagem
                                       }).ToArrayAsync();

                    List<Usuario> listaUsuarios = new List<Usuario>();

                    foreach (var usu in UsuarioMapper.ToDomainListDynamic(usuDB))
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

        public async Task<bool> CadastrarUsuarioAsync(Usuario usuario, Imagens imagem, List<Preferencias> listaPreferencias)
        {
            using (var _context = _contextFactory.CreateDbContext())
            {
                using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    ef.Models.Usuario usuarioDB = new ef.Models.Usuario();
                    ef.Models.Imagen imagemDB = new ef.Models.Imagen();

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

                    if (listaPreferencias != null)
                    {
                        var preferenciasAtuaisUsuarioDB = await (from pf in _context.PreferenciasUsuarios
                                                  where pf.UsuId == _usuarioService.UsuarioLogado.Id
                                                  select pf).ToArrayAsync();

                        var novasPreferenciasDB = listaPreferencias.ToArray().ToEntityList();

                        var adicionarPreferencias = novasPreferenciasDB.Where(x => !preferenciasAtuaisUsuarioDB.Select(y => y.PreId).Contains(x.PreId)).ToList();
                        var removerPreferencias = preferenciasAtuaisUsuarioDB.Where(x => !novasPreferenciasDB.Select(y => y.PreId).Contains(x.PreId)).ToList();

                        if (adicionarPreferencias.Count > 0)
                        {
                            foreach (var pref in adicionarPreferencias)
                            {
                                _context.Entry(new ef.Models.PreferenciasUsuario
                                {
                                    PreId = pref.PreId,
                                    UsuId = _usuarioService.UsuarioLogado.Id
                                }).State = EntityState.Added;
                                await _context.SaveChangesAsync();
                            }
                        }

                        if (removerPreferencias.Count > 0)
                        {
                            foreach(var pref in removerPreferencias)
                            {
                                _context.Entry(pref).State = EntityState.Deleted;
                                await _context.SaveChangesAsync();
                            }
                        }
                    }

                    _context.Entry(usuarioDB).State = usuarioDB.UsuId == 0 ? EntityState.Added : EntityState.Modified;
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();
                    return true;
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "CadastrarUsuarioAsync(Usuario usuario, Imagens imagem, List<Preferencias> listaPreferencias)");
                    await transaction.RollbackAsync();
                    return false;
                }
            }
        }
    }
}