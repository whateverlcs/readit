using FluentValidation;
using Readit.API.Communication.Requests;

namespace Readit.API.Application.UseCases.Genero.Gerenciar
{
    public class GerenciarGenerosValidacao : AbstractValidator<RequestGerenciarGeneroJson>
    {
        public GerenciarGenerosValidacao()
        {
            RuleFor(request => request.Nome).NotEmpty().WithMessage("O nome do gênero é obrigatório.");
        }
    }
}