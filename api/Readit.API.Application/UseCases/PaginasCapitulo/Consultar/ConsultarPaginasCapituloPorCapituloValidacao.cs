using FluentValidation;
using Readit.API.Communication.Requests;

namespace Readit.API.Application.UseCases.PaginasCapitulo.Consultar
{
    public class ConsultarPaginasCapituloPorCapituloValidacao : AbstractValidator<RequestConsultarPaginasCapituloPorCapituloJson>
    {
        public ConsultarPaginasCapituloPorCapituloValidacao()
        {
            RuleFor(request => request.ListaIdsCapitulo).NotEmpty().WithMessage("A lista de ids de capítulo é obrigatória.");
        }
    }
}