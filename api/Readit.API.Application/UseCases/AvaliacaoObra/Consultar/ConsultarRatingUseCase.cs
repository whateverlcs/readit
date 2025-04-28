using Readit.API.Communication.Requests;
using Readit.API.Communication.Responses;
using Readit.API.Exception;
using Readit.Core.Repositories;

namespace Readit.API.Application.UseCases.AvaliacaoObra.Consultar
{
    public class ConsultarRatingUseCase
    {
        private readonly IAvaliacaoObraRepository _avaliacaoRepository;
        private readonly IObraRepository _obraRepository;

        public ConsultarRatingUseCase(IAvaliacaoObraRepository avaliacaoObraRepository, IObraRepository obraRepository)
        {
            _avaliacaoRepository = avaliacaoObraRepository;
            _obraRepository = obraRepository;
        }

        public async Task<ResponseConsultarRatingJson> Execute(RequestConsultarRatingJson request)
        {
            var obraExistente = await _obraRepository.BuscarObrasPorIdAsync(request.IdObra);

            if (obraExistente.Count == 0)
                throw new NotFoundException("A Obra não existe no sistema.");

            double rating = await _avaliacaoRepository.BuscarRatingObraAsync(request.IdObra).ConfigureAwait(false);

            return new ResponseConsultarRatingJson
            {
                Rating = Math.Round(rating, 1)
            };
        }
    }
}