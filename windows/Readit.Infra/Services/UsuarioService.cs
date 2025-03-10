using Readit.Core.Domain;
using Readit.Core.Services;
using System.Net.Mail;

namespace Readit.Infra.Services
{
    public class UsuarioService : IUsuarioService
    {
        public Usuario UsuarioLogado { get; set; } = null!;
        public List<string> ListaCapitulosSelecionados { get; set; } = new List<string>();

        public List<string> ValidarCampos(string nome, string apelido, string senha, string email)
        {
            List<string> erros = [];

            if (!string.IsNullOrEmpty(nome) && nome.Length > 100) { erros.Add("O Nome Completo não pode ser maior do que 100 caracteres."); }
            if (!string.IsNullOrEmpty(apelido) && apelido.Length > 50) { erros.Add("O Apelido não pode ser maior do que 50 caracteres."); }
            if (!string.IsNullOrEmpty(senha) && senha.Length > 255) { erros.Add("A Senha não pode ser maior do que 255 caracteres."); }
            if (!string.IsNullOrEmpty(email) && email.Length > 100) { erros.Add("O E-mail não pode ser maior do que 100 caracteres."); }
            try { if (!string.IsNullOrEmpty(email)) { var mailAddress = new MailAddress(email); } } catch (FormatException) { erros.Add("O E-mail inserido não é um e-mail válido."); }

            return erros;
        }
    }
}