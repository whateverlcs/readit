using FluentValidation;
using Readit.API.Communication.Requests;

namespace Readit.API.Application.UseCases.AvaliacaoObra.Editar
{
    public class EditarRatingValidacao : AbstractValidator<RequestAtualizarRatingJson>
    {
        public EditarRatingValidacao()
        {
            RuleFor(request => request.Rating).NotEmpty().WithMessage("O Rating é obrigatório.");
            RuleFor(request => request.Rating).ExclusiveBetween(0, 6).WithMessage("O Rating deve ter ser entre 1 a 5.");
        }
    }
}