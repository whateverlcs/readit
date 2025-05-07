using Readit.API.Communication.Responses;
using Readit.API.Exception;
using Readit.Core.Repositories;
using Readit.Core.Services;

namespace Readit.API.Application.UseCases.Obra.Consultar.Destaques
{
    public class ConsultarDestaquesObrasUseCase
    {
        private readonly IObraRepository _obraRepository;
        private readonly IEnumService _enumService;
        private readonly IImagemService _imagemService;

        public ConsultarDestaquesObrasUseCase(IObraRepository obraRepository, IEnumService enumService, IImagemService imagemService)
        {
            _obraRepository = obraRepository;
            _enumService = enumService;
            _imagemService = imagemService;
        }

        public async Task<ResponseConsultarDestaquesObrasJson> Execute()
        {
            var destaquesObras = await _obraRepository.BuscarObrasEmDestaqueAsync().ConfigureAwait(false);

            if (destaquesObras.Count == 0)
            {
                throw new InvalidActionException("Ocorreu um erro ao realizar a consulta dos destaques das obras, tente novamente em segundos.");
            }

            ResponseConsultarDestaquesObrasJson response = new() { Destaques = [] };

            foreach (var destaque in destaquesObras)
            {
                response.Destaques.Add(new Communication.Responses.Destaques
                {
                    Rank = destaque.Rank,
                    Nome = destaque.Title,
                    Imagem = destaque.ImageByte,
                    ImagemFlag = _imagemService.ConvertImageToByteArray(_enumService.ObterFlagPorTipo(destaque.TipoNumber)),
                    Tipo = _enumService.ObterTipo(destaque.TipoNumber),
                    Generos = destaque.Genres,
                    Filter = destaque.Filter,
                    Rating = Math.Round(destaque.Rating, 1)
                });
            }

            return response;
        }
    }
}