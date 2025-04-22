using Readit.Core.Desktop.Domain;
using Readit.Core.Domain;

namespace Readit.Infra.Desktop.Mappers
{
    public static class DestaquesItemMapper
    {
        public static DestaquesItemDesktop DomainToDesktop(this DestaquesItem destaquesItem)
        {
            return new DestaquesItemDesktop
            {
                Rank = destaquesItem.Rank,
                ImageByte = destaquesItem.ImageByte,
                Title = destaquesItem.Title,
                Genres = destaquesItem.Genres,
                Rating = destaquesItem.Rating,
                Filter = destaquesItem.Filter,
                TipoNumber = destaquesItem.TipoNumber
            };
        }
    }
}