using Readit.API.Communication.Requests;
using Readit.API.Communication.Responses;
using Readit.API.Exception;
using Readit.Core.Repositories;

namespace Readit.API.Application.UseCases.Obra.Consultar.PorNome
{
    public class ConsultarObrasPorNomeUseCase
    {
        private readonly IObraRepository _obraRepository;

        public ConsultarObrasPorNomeUseCase(IObraRepository obraRepository)
        {
            _obraRepository = obraRepository;
        }

        public async Task<ResponseConsultarObrasPorNomeJson> Execute(RequestConsultarObrasPorNomeJson request)
        {
            Validate(request);

            var obras = await _obraRepository.BuscarObrasPorNomeAsync(request.NomeObra).ConfigureAwait(false);

            if (obras.Count == 0)
            {
                throw new NotFoundException("A obra não existe.");
            }

            return new ResponseConsultarObrasPorNomeJson
            {
                Obras = obras
            };
        }

        private void Validate(RequestConsultarObrasPorNomeJson request)
        {
            var validator = new ConsultarObrasPorNomeValidacao();

            var result = validator.Validate(request);

            if (!result.IsValid)
            {
                var errorMessages = result.Errors.Select(error => error.ErrorMessage).ToList();

                throw new ErrorOnValidationException(errorMessages);
            }
        }
    }
}