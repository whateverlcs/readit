using FluentValidation;
using Readit.API.Communication.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Readit.API.Application.UseCases.Bookmark
{
    public class RealizarBookmarkValidacao : AbstractValidator<RequestRealizarBookmarkJson>
    {
        public RealizarBookmarkValidacao()
        {
            RuleFor(request => request.ObraId).NotEmpty().WithMessage("O Id da Obra é obrigatório.");
            RuleFor(request => request.UsuarioId).NotEmpty().WithMessage("O Id do Usuário é obrigatório.");
        }
    }
}
