using FluentValidation;
using Readit.API.Communication.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Readit.API.Application.UseCases.Capitulo.ConsultaCompleta
{
    public class ConsultarCapituloCompletoValidacao : AbstractValidator<RequestConsultarCapitulosCompletoJson>
    {
        public ConsultarCapituloCompletoValidacao()
        {
            RuleFor(request => request.CapituloId).NotEmpty().WithMessage("O Id do capítulo é obrigatório.");
            RuleFor(request => request.IncluirNumeroCapitulos).NotEmpty().WithMessage("A flag de inclusão de número de capítulos é obrigatória.");
            RuleFor(request => request.IncluirPaginasCapitulo).NotEmpty().WithMessage("A flag de inclusão das páginas do capítulo é obrigatória.");
        }
    }
}
