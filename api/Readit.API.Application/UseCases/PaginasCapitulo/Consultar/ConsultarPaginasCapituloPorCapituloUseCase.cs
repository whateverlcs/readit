using Readit.API.Communication.Requests;
using Readit.API.Communication.Responses;
using Readit.API.Exception;
using Readit.Core.Repositories;

namespace Readit.API.Application.UseCases.PaginasCapitulo.Consultar
{
    public class ConsultarPaginasCapituloPorCapituloUseCase
    {
        private readonly IPaginaCapituloRepository _paginaCapituloRepository;

        public ConsultarPaginasCapituloPorCapituloUseCase(IPaginaCapituloRepository paginaCapituloRepository)
        {
            _paginaCapituloRepository = paginaCapituloRepository;
        }

        public async Task<ResponseConsultarPaginasCapituloPorCapituloJson> Execute(RequestConsultarPaginasCapituloPorCapituloJson request)
        {
            Validate(request);

            var paginasCapitulo = await _paginaCapituloRepository.BuscarPaginasCapituloPorIdsAsync(request.ListaIdsCapitulo).ConfigureAwait(false);

            if (paginasCapitulo.Count == 0)
            {
                throw new InvalidActionException("Ocorreu um erro ao realizar a consulta das páginas do capitulo, tente novamente em segundos.");
            }

            return new ResponseConsultarPaginasCapituloPorCapituloJson
            {
                ListaPaginasCapitulo = paginasCapitulo
            };
        }

        private void Validate(RequestConsultarPaginasCapituloPorCapituloJson request)
        {
            var validator = new ConsultarPaginasCapituloPorCapituloValidacao();

            var result = validator.Validate(request);

            if (!result.IsValid)
            {
                var errorMessages = result.Errors.Select(error => error.ErrorMessage).ToList();

                throw new ErrorOnValidationException(errorMessages);
            }
        }
    }
}