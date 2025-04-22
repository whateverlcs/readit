using Readit.Core.Desktop.Domain;
using Readit.Core.Desktop.Services;
using Readit.Core.Repositories;
using Readit.Core.Services;
using Readit.Infra.Desktop.Mappers;

namespace Readit.Infra.Desktop.Services
{
    public class ComentarioDesktopService : IComentarioDesktopService
    {
        private readonly IComentarioRepository _comentarioRepository;
        private readonly IImagemDesktopService _imagemDesktopService;
        private readonly IUtilService _utilService;
        private readonly IUsuarioService _usuarioService;

        public ComentarioDesktopService(IComentarioRepository comentarioRepository, IImagemDesktopService imagemDesktopService, IUtilService utilService, IUsuarioService usuarioService)
        {
            _comentarioRepository = comentarioRepository;
            _imagemDesktopService = imagemDesktopService;
            _utilService = utilService;
            _usuarioService = usuarioService;
        }

        public async Task<List<ComentariosDesktop>> FormatarDadosComentarios(int idObra, int? idCapitulo)
        {
            var comentariosObra = await _comentarioRepository.BuscarComentariosObraAsync(idObra, idCapitulo).ConfigureAwait(false);

            List<ComentariosDesktop> listaComentariosDesktop = new List<ComentariosDesktop>();
            foreach (var comentarios in comentariosObra)
            {
                ComentariosDesktop comentarioDesktop = comentarios.DomainToDesktop();

                comentarioDesktop.ImagemPerfil = _imagemDesktopService.ByteArrayToImage(comentarios.ImageByte);
                comentarioDesktop.TempoDecorridoFormatado = _utilService.FormatarData(comentarios.TempoUltimaAtualizacaoDecorrido ?? comentarios.TempoDecorrido);
                comentarioDesktop.IsUsuarioOuAdministrador = _usuarioService.UsuarioLogado.Id == comentarios.IdUsuario || _usuarioService.UsuarioLogado.Administrador;

                foreach (var resposta in comentarios.Respostas)
                {
                    ComentariosDesktop respostaDesktop = resposta.DomainToDesktop();

                    respostaDesktop.ImagemPerfil = _imagemDesktopService.ByteArrayToImage(resposta.ImageByte);
                    respostaDesktop.TempoDecorridoFormatado = _utilService.FormatarData(resposta.TempoUltimaAtualizacaoDecorrido ?? resposta.TempoDecorrido);
                    respostaDesktop.IsUsuarioOuAdministrador = _usuarioService.UsuarioLogado.Id == resposta.IdUsuario || _usuarioService.UsuarioLogado.Administrador;

                    respostaDesktop.Pai = comentarioDesktop;

                    comentarioDesktop.Respostas.Add(respostaDesktop);
                }

                listaComentariosDesktop.Add(comentarioDesktop);
            }

            return listaComentariosDesktop;
        }
    }
}