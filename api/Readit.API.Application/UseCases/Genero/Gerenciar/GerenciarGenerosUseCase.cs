using Readit.API.Communication.Requests;
using Readit.API.Exception;
using Readit.Core.Repositories;

namespace Readit.API.Application.UseCases.Genero.Gerenciar
{
    public class GerenciarGenerosUseCase
    {
        private readonly IGeneroRepository _generoRepository;

        public GerenciarGenerosUseCase(IGeneroRepository generoRepository)
        {
            _generoRepository = generoRepository;
        }

        public async Task<bool> Execute(RequestGerenciarGeneroJson request)
        {
            await Validate(request);

            var sucesso = await _generoRepository.CadastrarGeneroAsync(new Core.Domain.Generos { Id = request.Id, Nome = request.Nome }).ConfigureAwait(false);

            if (!sucesso)
            {
                throw new InvalidActionException("Ocorreu um erro ao realizar a adição/edição do genêro, tente novamente em segundos.");
            }

            return true;
        }

        private async Task Validate(RequestGerenciarGeneroJson request)
        {
            var validator = new GerenciarGenerosValidacao();

            var result = validator.Validate(request);

            if (request.Id == 0)
            {
                var generoExistente = await _generoRepository.BuscarGenerosPorNomeAsync(request.Nome);

                if (generoExistente.Count == 0)
                    throw new NotFoundException("O gênero já existe no sistema.");
            }

            if (!result.IsValid)
            {
                var errorMessages = result.Errors.Select(error => error.ErrorMessage).ToList();

                throw new ErrorOnValidationException(errorMessages);
            }
        }
    }
}