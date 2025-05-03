using FluentValidation;
using Readit.API.Communication.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Readit.API.Application.UseCases.Comentarios.Avaliacao.Gerenciar
{
    public class GerenciarAvaliacoesValidacao : AbstractValidator<RequestGerenciarAvaliacoesComentarioJson>
    {
        public GerenciarAvaliacoesValidacao()
        {
            RuleFor(request => request.ComentarioId).NotEmpty().WithMessage("O id do comentário é obrigatório.");
            RuleFor(request => request.TipoAvaliacao).NotEmpty().WithMessage("O tipo de avaliação é obrigatório.");
            RuleFor(request => request.TipoAcao).NotEmpty().WithMessage("O tipo de ação é obrigatório.");
        }
    }
}
