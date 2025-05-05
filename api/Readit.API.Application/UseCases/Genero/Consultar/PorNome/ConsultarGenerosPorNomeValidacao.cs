using FluentValidation;
using Readit.API.Communication.Requests;

namespace Readit.API.Application.UseCases.Genero.Consultar.PorNome
{
    public class ConsultarGenerosPorNomeValidacao : AbstractValidator<RequestConsultarGenerosPorNomeJson>
    {
        public ConsultarGenerosPorNomeValidacao()
        {
            RuleFor(request => request.Nome).NotEmpty().WithMessage("O nome do gênero é obrigatório.");
        }
    }
}