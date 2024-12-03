using System.IO;
using System.Net.Mail;

namespace readit.Controls
{
    public class ControlPrincipal
    {
        public List<string> ValidarCampos(string nome, string apelido, string senha, string email)
        {
            List<string> erros = [];

            if (!string.IsNullOrEmpty(nome) && nome.Length > 100) { erros.Add("O Nome Completo não pode ser maior do que 100 caracteres."); }
            if (!string.IsNullOrEmpty(apelido) && apelido.Length > 50) { erros.Add("O Apelido não pode ser maior do que 50 caracteres."); }
            if (!string.IsNullOrEmpty(senha) && senha.Length > 255) { erros.Add("A Senha não pode ser maior do que 255 caracteres."); }
            if (!string.IsNullOrEmpty(email) && email.Length > 100) { erros.Add("O Email não pode ser maior do que 100 caracteres."); }
            try { if (!string.IsNullOrEmpty(email)) { var mailAddress = new MailAddress(email); } } catch (FormatException) { erros.Add("O e-mail inserido não é um e-mail válido."); }

            return erros;
        }

        public byte[] ConvertImageToByteArray(string imagePath)
        {
            return File.ReadAllBytes(imagePath);
        }
    }
}