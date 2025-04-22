using Readit.Core.Domain;

namespace Readit.Core.Repositories
{
    public interface IUsuarioRepository
    {
        public Task<bool> CadastrarUsuarioAsync(Usuario usuario, Imagens imagem, List<Preferencias> listaPreferencias);

        public Task<List<Usuario>> BuscarUsuarioPorEmailAsync(string email);
    }
}