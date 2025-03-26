using Readit.Core.Domain;

namespace Readit.Core.Services
{
    public interface IUsuarioService
    {
        Usuario UsuarioLogado { get; set; }
        List<string> ListaCapitulosSelecionados { get; set; }
        CancellationToken Token { get; }

        void CancelarConsultas();
        List<string> ValidarCampos(string nome, string apelido, string senha, string email);
    }
}