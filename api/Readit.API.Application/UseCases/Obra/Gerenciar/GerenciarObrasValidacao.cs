using FluentValidation;
using Readit.API.Communication.Requests;

namespace Readit.API.Application.UseCases.Obra.Gerenciar
{
    public class GerenciarObrasValidacao : AbstractValidator<RequestGerenciarObrasJson>
    {
        public GerenciarObrasValidacao()
        {
            RuleFor(request => request.Obra).NotEmpty().WithMessage("A obra é obrigatória.");
            RuleFor(request => request.Imagem).NotEmpty().WithMessage("A imagem é obrigatória.");
            RuleFor(request => request.Generos).NotEmpty().WithMessage("Os generos são obrigatórios.");
        }
    }
}