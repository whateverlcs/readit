using Readit.API.Communication.Requests;
using Readit.API.Communication.Responses;
using Readit.API.Exception;
using Readit.Core.Repositories;

namespace Readit.API.Application.UseCases.Genero.Consultar.PorNome
{
    public class ConsultarGenerosPorNomeUseCase
    {
        private readonly IGeneroRepository _generoRepository;

        public ConsultarGenerosPorNomeUseCase(IGeneroRepository generoRepository)
        {
            _generoRepository = generoRepository;
        }

        public async Task<ResponseConsultarGenerosJson> Execute(RequestConsultarGenerosPorNomeJson request)
        {
            Validate(request);

            var genero = await _generoRepository.BuscarGenerosPorNomeAsync(request.Nome).ConfigureAwait(false);

            if (genero.Count == 0)
            {
                throw new InvalidActionException("Ocorreu um erro ao realizar a consulta do gênero, tente novamente em segundos.");
            }

            return new ResponseConsultarGenerosJson
            {
                ListaGeneros = genero
            };
        }

        private void Validate(RequestConsultarGenerosPorNomeJson request)
        {
            var validator = new ConsultarGenerosPorNomeValidacao();

            var result = validator.Validate(request);

            if (!result.IsValid)
            {
                var errorMessages = result.Errors.Select(error => error.ErrorMessage).ToList();

                throw new ErrorOnValidationException(errorMessages);
            }
        }
    }
}