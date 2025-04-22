using Readit.Core.Domain;

namespace Readit.Core.Services
{
    public interface IUsuarioService
    {
        Usuario UsuarioLogado { get; set; }

        /// <summary>
        /// Lista de capítulos inseridos pelo usuário
        /// </summary>
        List<string> ListaCapitulosSelecionados { get; set; }

        /// <summary>
        /// Utilizado para os casos em que houver uma consulta em andamento e para evitar algum problema, ele realiza o cancelamento dessas consultas.
        /// </summary>
        CancellationToken Token { get; }

        void CancelarConsultas();

        /// <summary>
        /// Valida os campos de cadastro de usuário.
        /// </summary>
        /// <returns>Caso haja algum erro, retorna os erros encontrados.</returns>
        List<string> ValidarCampos(string nome, string apelido, string senha, string email);
    }
}