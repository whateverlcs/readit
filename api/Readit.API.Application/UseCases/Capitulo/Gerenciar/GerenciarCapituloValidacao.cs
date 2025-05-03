using FluentValidation;
using Readit.API.Communication.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Readit.API.Application.UseCases.Capitulo.Gerenciar
{
    public class GerenciarCapituloValidacao : AbstractValidator<RequestGerenciarCapitulosJson>
    {
        public GerenciarCapituloValidacao()
        {
            RuleFor(request => request)
                .Must(x => x.ListaCapitulosObraAdicionar.Count > 0 || x.ListaCapitulosObraRemover.Count > 0)
                .WithMessage("Pelo menos uma das listas (adicionar ou remover) deve ser preenchida.");
        }
    }
}
