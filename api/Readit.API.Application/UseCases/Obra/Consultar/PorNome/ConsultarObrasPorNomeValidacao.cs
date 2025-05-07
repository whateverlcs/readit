using FluentValidation;
using Readit.API.Communication.Requests;

namespace Readit.API.Application.UseCases.Obra.Consultar.PorNome
{
    public class ConsultarObrasPorNomeValidacao : AbstractValidator<RequestConsultarObrasPorNomeJson>
    {
        public ConsultarObrasPorNomeValidacao()
        {
            RuleFor(request => request.NomeObra).NotEmpty().WithMessage("O nome da obra é obrigatória.");
        }
    }
}