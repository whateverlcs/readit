using Readit.API.Communication.Requests;
using Readit.API.Exception;
using Readit.Core.Repositories;

namespace Readit.API.Application.UseCases.Obra.Gerenciar
{
    public class GerenciarObrasUseCase
    {
        private readonly IObraRepository _obraRepository;

        public GerenciarObrasUseCase(IObraRepository obraRepository)
        {
            _obraRepository = obraRepository;
        }

        public async Task<bool> Execute(RequestGerenciarObrasJson request)
        {
            Validate(request);

            var sucesso = await _obraRepository.CadastrarEditarObraAsync(request.Obra, request.Imagem, request.Generos).ConfigureAwait(false);

            if (!sucesso)
            {
                throw new InvalidActionException("Ocorreu um erro ao realizar a adição/edição da(s) obra(s), tente novamente em segundos.");
            }

            return true;
        }

        private void Validate(RequestGerenciarObrasJson request)
        {
            var validator = new GerenciarObrasValidacao();

            var result = validator.Validate(request);

            if (!result.IsValid)
            {
                var errorMessages = result.Errors.Select(error => error.ErrorMessage).ToList();

                throw new ErrorOnValidationException(errorMessages);
            }
        }
    }
}