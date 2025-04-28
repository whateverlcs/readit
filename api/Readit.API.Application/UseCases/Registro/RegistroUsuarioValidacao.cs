using FluentValidation;
using Readit.API.Communication.Requests;

namespace Readit.API.Application.UseCases.Registro
{
    public class RegistroUsuarioValidacao : AbstractValidator<RequestUsuarioJson>
    {
        public RegistroUsuarioValidacao()
        {
            RuleFor(request => request.Nome).NotEmpty().WithMessage("O nome é obrigatório.");
            RuleFor(request => request.Apelido).NotEmpty().WithMessage("O apelido é obrigatório.");
            RuleFor(request => request.Email).EmailAddress().WithMessage("O email não é valido.");
            RuleFor(request => request.Senha).NotEmpty().WithMessage("A senha é obrigatória.");
            When(request => !string.IsNullOrEmpty(request.Senha), () =>
            {
                RuleFor(request => request.Senha.Length).GreaterThanOrEqualTo(6).WithMessage("Senha deve ter mais que 6 caracteres.");
            });
        }
    }
}