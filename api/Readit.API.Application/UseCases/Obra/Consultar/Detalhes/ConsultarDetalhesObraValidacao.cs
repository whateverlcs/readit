using FluentValidation;
using Readit.API.Communication.Requests;

namespace Readit.API.Application.UseCases.Obra.Consultar.Detalhes
{
    public class ConsultarDetalhesObraValidacao : AbstractValidator<RequestConsultarDetalhesObraJson>
    {
        public ConsultarDetalhesObraValidacao()
        {
            RuleFor(request => request.NomeObra).NotEmpty().WithMessage("O nome da obra é obrigatória.");
        }
    }
}