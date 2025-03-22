using Readit.Core.Domain;

namespace Readit.Core.Repositories
{
    public interface IComentarioRepository
    {
        public Task<List<Comentarios>> BuscarComentariosObraAsync(int idObra, int? capituloId);

        public Task<(bool, int)> CadastrarComentarioAsync(Comentarios comentario);

        public Task<bool> ConsultarLikesDeslikesUsuarioAsync(Comentarios comentario, string tipoAvaliacao);

        public Task<bool> CadastrarRemoverAvaliacaoComentarioAsync(Comentarios comentario, string tipoAvaliacao, string tipoAcao);
    }
}