using Readit.API.Communication.Responses;
using Readit.API.Exception;
using Readit.Core.Repositories;
using Readit.Core.Services;

namespace Readit.API.Application.UseCases.Obra.Consultar.Bookmark
{
    public class ConsultarBookmarkObrasUseCase
    {
        private readonly IObraRepository _obraRepository;
        private readonly IEnumService _enumService;
        private readonly IImagemService _imagemService;

        public ConsultarBookmarkObrasUseCase(IObraRepository obraRepository, IEnumService enumService, IImagemService imagemService)
        {
            _obraRepository = obraRepository;
            _enumService = enumService;
            _imagemService = imagemService;
        }

        public async Task<ResponseConsultarBookmarkObrasJson> Execute()
        {
            var bookmarkObras = await _obraRepository.BuscarObrasBookmarksAsync().ConfigureAwait(false);

            if (bookmarkObras.Count == 0)
            {
                throw new InvalidActionException("Ocorreu um erro ao realizar a consulta dos bookmarks, tente novamente em segundos.");
            }

            ResponseConsultarBookmarkObrasJson response = new() { Bookmarks = [] };

            foreach (var bookmark in bookmarkObras)
            {
                response.Bookmarks.Add(new Communication.Responses.Bookmarks
                {
                    IdObra = bookmark.ObraId,
                    Nome = bookmark.Title,
                    NomeAbreviado = bookmark.Title.Length > 39 ? bookmark.Title.Substring(0, 39).Trim() + "..." : bookmark.Title.Trim(),
                    Imagem = bookmark.ImageByte,
                    ImagemFlag = _imagemService.ConvertImageToByteArray(_enumService.ObterFlagPorTipo(bookmark.TipoNumber)),
                    Tipo = _enumService.ObterTipo(bookmark.TipoNumber)
                });
            }

            return response;
        }
    }
}