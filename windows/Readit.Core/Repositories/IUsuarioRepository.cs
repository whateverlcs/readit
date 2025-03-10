using Readit.Core.Domain;

namespace Readit.Core.Repositories
{
    public interface IUsuarioRepository
    {
        public Task<bool> CadastrarUsuarioAsync(Usuario usuario, Imagens imagem, TipoVisualizacaoObraUsuario? tipoVisualizacaoObra);

        public Task<List<Usuario>> BuscarUsuarioPorEmailAsync(string email);
    }
}