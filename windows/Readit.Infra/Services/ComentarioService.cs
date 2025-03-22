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

        public ComentarioService(IComentarioRepository comentarioRepository, IImagemService imagemService, IUtilService utilService)
        {
            _comentarioRepository = comentarioRepository;
            _imagemService = imagemService;
            _utilService = utilService;
        }

        public async Task<List<Comentarios>> FormatarDadosComentarios(int idObra, int? idCapitulo)
        {
            var comentariosObra = await _comentarioRepository.BuscarComentariosObraAsync(idObra, idCapitulo).ConfigureAwait(false);

            foreach (var comentarios in comentariosObra)
            {
                comentarios.ImagemPerfil = _imagemService.ByteArrayToImage(comentarios.ImageByte);
                comentarios.TempoDecorridoFormatado = _utilService.FormatarData(comentarios.TempoDecorrido);

                foreach (var resposta in comentarios.Respostas)
                {
                    resposta.ImagemPerfil = _imagemService.ByteArrayToImage(resposta.ImageByte);
                    resposta.TempoDecorridoFormatado = _utilService.FormatarData(resposta.TempoDecorrido);
                }
            }

            return comentariosObra;
        }
    }
}