using Readit.API.Application.UseCases.Bookmark;
using Readit.API.Communication.Requests;
using Readit.API.Communication.Responses;
using Readit.API.Exception;
using Readit.Core.Repositories;
using Readit.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Readit.API.Application.UseCases.Capitulo.Gerenciar
{
    public class GerenciarCapituloUseCase
    {
        private readonly ICapituloRepository _capituloRepository;

        public GerenciarCapituloUseCase(ICapituloRepository capituloRepository)
        {
            _capituloRepository = capituloRepository;
        }

        public async Task<bool> Execute(RequestGerenciarCapitulosJson request)
        {
            Validate(request);

            var sucesso = await _capituloRepository.CadastrarRemoverCapitulosAsync(request.ListaCapitulosObraAdicionar, request.ListaCapitulosObraRemover).ConfigureAwait(false);

            if (!sucesso)
            {
                throw new InvalidActionException("Ocorreu um erro ao realizar a adição/remoção do(s) capítulo(s), tente novamente em segundos.");
            }

            return true;
        }

        private void Validate(RequestGerenciarCapitulosJson request)
        {
            var validator = new GerenciarCapituloValidacao();

            var result = validator.Validate(request);

            if (!result.IsValid)
            {
                var errorMessages = result.Errors.Select(error => error.ErrorMessage).ToList();

                throw new ErrorOnValidationException(errorMessages);
            }
        }
    }
}
