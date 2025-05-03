using FluentValidation;
using Readit.API.Communication.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Readit.API.Application.UseCases.Comentarios.Cadastrar
{
    public class CadastrarComentarioValidacao : AbstractValidator<RequestCadastrarComentarioJson>
    {
        public CadastrarComentarioValidacao()
        {
            RuleFor(request => request.Data).NotEmpty().WithMessage("A data é obrigatória.");
            RuleFor(request => request.ObraId).NotEmpty().WithMessage("O id da obra é obrigatório.");
            RuleFor(request => request.UsuarioId).NotEmpty().WithMessage("O id do usuário é obrigatório.");
            RuleFor(request => request.Comentario).NotEmpty().WithMessage("O comentário é obrigatório.");
        }
    }
}
