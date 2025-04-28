using Readit.API.Communication.Requests;
using Readit.API.Exception;
using Readit.Core.Repositories;

namespace Readit.API.Application.UseCases.AvaliacaoObra.Editar
{
    public class EditarRatingUseCase
    {
        private readonly IAvaliacaoObraRepository _avaliacaoRepository;
        private readonly IObraRepository _obraRepository;

        public EditarRatingUseCase(IAvaliacaoObraRepository avaliacaoObraRepository, IObraRepository obraRepository)
        {
            _avaliacaoRepository = avaliacaoObraRepository;
            _obraRepository = obraRepository;
        }

        public async Task<bool> Execute(RequestAtualizarRatingJson request)
        {
            await Validate(request);

            bool sucesso = await _avaliacaoRepository.AtualizarRatingAsync(request.IdObra, request.Rating).ConfigureAwait(false);

            if (!sucesso)
            {
                throw new InvalidActionException("Ocorreu um erro ao realizar a atualização do rating da obra, tente novamente em segundos.");
            }

            return sucesso;
        }

        private async Task Validate(RequestAtualizarRatingJson request)
        {
            var validator = new EditarRatingValidacao();

            var result = validator.Validate(request);

            var obraExistente = await _obraRepository.BuscarObrasPorIdAsync(request.IdObra);

            if (obraExistente.Count == 0)
                throw new NotFoundException("A Obra não existe no sistema.");

            if (!result.IsValid)
            {
                var errorMessages = result.Errors.Select(error => error.ErrorMessage).ToList();

                throw new ErrorOnValidationException(errorMessages);
            }
        }
    }
}