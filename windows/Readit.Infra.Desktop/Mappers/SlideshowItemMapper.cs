using Readit.Core.Desktop.Domain;
using Readit.Core.Domain;

namespace Readit.Infra.Desktop.Mappers
{
    public static class SlideshowItemMapper
    {
        public static SlideshowItemDesktop DomainToDesktop(this SlideshowItem slideshowItem)
        {
            return new SlideshowItemDesktop
            {
                Chapter = slideshowItem.Chapter,
                Title = slideshowItem.Title,
                Description = slideshowItem.Description,
                Tags = slideshowItem.Tags,
                BackgroundImageByte = slideshowItem.BackgroundImageByte,
                FocusedImageByte = slideshowItem.FocusedImageByte
            };
        }
    }
}