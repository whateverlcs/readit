using Readit.API.Communication.Requests;
using Readit.API.Exception;
using Readit.Core.Repositories;

namespace Readit.API.Application.UseCases.Visualizacao
{
    public class GerenciarVisualizacaoUseCase
    {
        private readonly IVisualizacaoObraRepository _visualizacaoObraRepository;

        public GerenciarVisualizacaoUseCase(IVisualizacaoObraRepository visualizacaoObraRepository)
        {
            _visualizacaoObraRepository = visualizacaoObraRepository;
        }

        public async Task<bool> Execute(RequestGerenciarVisualizacaoJson request)
        {
            Validate(request);

            var sucesso = await _visualizacaoObraRepository.AtualizarViewObraAsync(request.NomeObra).ConfigureAwait(false);

            if (!sucesso)
            {
                throw new InvalidActionException("Ocorreu um erro ao realizar a atualização das visualizações da obra, tente novamente em segundos.");
            }

            return true;
        }

        private void Validate(RequestGerenciarVisualizacaoJson request)
        {
            var validator = new GerenciarVisualizacaoValidacao();

            var result = validator.Validate(request);

            if (!result.IsValid)
            {
                var errorMessages = result.Errors.Select(error => error.ErrorMessage).ToList();

                throw new ErrorOnValidationException(errorMessages);
            }
        }
    }
}