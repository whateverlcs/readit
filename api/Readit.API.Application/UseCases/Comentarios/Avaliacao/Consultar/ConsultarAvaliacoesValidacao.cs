using FluentValidation;
using Readit.API.Communication.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Readit.API.Application.UseCases.Comentarios.Avaliacao.Consultar
{
    public class ConsultarAvaliacoesValidacao : AbstractValidator<RequestConsultarAvaliacoesJson>
    {
        public ConsultarAvaliacoesValidacao()
        {
            RuleFor(request => request.IdComentario).NotEmpty().WithMessage("O Id do comentário é obrigatório.");
            RuleFor(request => request.TipoAvaliacao).NotEmpty().WithMessage("O tipo de avaliação é obrigatório.");
        }
    }
}
