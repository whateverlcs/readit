using FluentValidation;
using Readit.API.Communication.Requests;

namespace Readit.API.Application.UseCases.Visualizacao
{
    public class GerenciarVisualizacaoValidacao : AbstractValidator<RequestGerenciarVisualizacaoJson>
    {
        public GerenciarVisualizacaoValidacao()
        {
            RuleFor(request => request.NomeObra).NotEmpty().WithMessage("O nome da obra é obrigatória.");
        }
    }
}