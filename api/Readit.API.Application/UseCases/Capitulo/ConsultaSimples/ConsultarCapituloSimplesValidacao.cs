using FluentValidation;
using Readit.API.Communication.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Readit.API.Application.UseCases.Capitulo.ConsultaSimples
{
    public class ConsultarCapituloSimplesValidacao : AbstractValidator<RequestConsultarCapitulosSimplesJson>
    {
        public ConsultarCapituloSimplesValidacao()
        {
            RuleFor(request => request.ListaNumerosCapitulos).NotEmpty().WithMessage("A lista de capítulos é obrigatória.");
        }
    }
}
