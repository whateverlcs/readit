using Readit.Core.Domain;
using ef = Readit.Data.Models;

namespace Readit.Data.Mappers
{
    public static class BookmarksUsuarioMapper
    {
        public static ef.BookmarksUsuario ToEntity(this BookmarksUsuario bookmark)
        {
            return new ef.BookmarksUsuario
            {
                BkuId = bookmark.Id,
                UsuId = bookmark.UsuarioId,
                ObsId = bookmark.ObraId,
            };
        }

        public static BookmarksUsuario ToDomain(this ef.BookmarksUsuario bookmark)
        {
            return new BookmarksUsuario
            {
                Id = bookmark.BkuId,
                ObraId = bookmark.ObsId,
                UsuarioId = bookmark.UsuId
            };
        }
    }
}