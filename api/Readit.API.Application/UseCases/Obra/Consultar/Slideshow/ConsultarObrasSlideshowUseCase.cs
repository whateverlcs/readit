using Readit.API.Communication.Responses;
using Readit.API.Exception;
using Readit.Core.Repositories;

namespace Readit.API.Application.UseCases.Obra.Consultar.Slideshow
{
    public class ConsultarObrasSlideshowUseCase
    {
        private readonly IObraRepository _obraRepository;

        public ConsultarObrasSlideshowUseCase(IObraRepository obraRepository)
        {
            _obraRepository = obraRepository;
        }

        public async Task<ResponseConsultarObrasSlideshowJson> Execute()
        {
            var slideshowObras = await _obraRepository.BuscarObrasSlideShowAsync().ConfigureAwait(false);

            if (slideshowObras.Count == 0)
            {
                throw new InvalidActionException("Ocorreu um erro ao realizar a consulta da obras do slideshow, tente novamente em segundos.");
            }

            ResponseConsultarObrasSlideshowJson response = new() { Slideshow = [] };

            foreach (var slideshow in slideshowObras)
            {
                response.Slideshow.Add(new Communication.Responses.Slideshow
                {
                    Nome = slideshow.Title,
                    UltimoCapitulo = slideshow.Chapter,
                    Descricao = slideshow.Description.Length > 211 ? slideshow.Description.Substring(0, 211).Trim() + "..." : slideshow.Description.Trim(),
                    Generos = slideshow.Tags,
                    Imagem = slideshow.BackgroundImageByte
                });
            }

            return response;
        }
    }
}