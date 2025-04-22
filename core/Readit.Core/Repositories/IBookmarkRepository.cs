using Readit.Core.Domain;

namespace Readit.Core.Repositories
{
    public interface IBookmarkRepository
    {
        public Task<(bool, string)> CadastrarRemoverBookmarkAsync(BookmarksUsuario bookmarkUsuario);
    }
}