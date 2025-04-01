using Readit.Core.Domain;
using Readit.Core.Repositories;
using Readit.Core.Services;

namespace Readit.Infra.Services
{
    public class ComentarioService : IComentarioService
    {
        private readonly IComentarioRepository _comentarioRepository;
        private readonly IImagemService _imagemService;
        private readonly IUtilService _utilService;
        private readonly IUsuarioService _usuarioService;

        public ComentarioService(IComentarioRepository comentarioRepository, IImagemService imagemService, IUtilService utilService, IUsuarioService usuarioService)
        {
            _comentarioRepository = comentarioRepository;
            _imagemService = imagemService;
            _utilService = utilService;
            _usuarioService = usuarioService;
        }

        public async Task<List<Comentarios>> FormatarDadosComentarios(int idObra, int? idCapitulo)
        {
            var comentariosObra = await _comentarioRepository.BuscarComentariosObraAsync(idObra, idCapitulo).ConfigureAwait(false);

            foreach (var comentarios in comentariosObra)
            {
                comentarios.ImagemPerfil = _imagemService.ByteArrayToImage(comentarios.ImageByte);
                comentarios.TempoDecorridoFormatado = _utilService.FormatarData(comentarios.TempoUltimaAtualizacaoDecorrido ?? comentarios.TempoDecorrido);
                comentarios.IsUsuarioOuAdministrador = _usuarioService.UsuarioLogado.Id == comentarios.IdUsuario || _usuarioService.UsuarioLogado.Administrador;

                foreach (var resposta in comentarios.Respostas)
                {
                    resposta.ImagemPerfil = _imagemService.ByteArrayToImage(resposta.ImageByte);
                    resposta.TempoDecorridoFormatado = _utilService.FormatarData(resposta.TempoUltimaAtualizacaoDecorrido ?? resposta.TempoDecorrido);
                    resposta.IsUsuarioOuAdministrador = _usuarioService.UsuarioLogado.Id == resposta.IdUsuario || _usuarioService.UsuarioLogado.Administrador;
                    resposta.Pai = comentarios;
                }
            }

            return comentariosObra;
        }
    }
}