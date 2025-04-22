using Readit.Core.Desktop.Domain;
using Readit.Core.Domain;

namespace Readit.Infra.Desktop.Mappers
{
    public static class DetalhesObraMapper
    {
        public static DetalhesObraDesktop DomainToDesktop(this DetalhesObra detalhesObra)
        {
            return new DetalhesObraDesktop
            {
                ObraId = detalhesObra.ObraId,
                ImageByte = detalhesObra.ImageByte,
                Title = detalhesObra.Title,
                Tags = detalhesObra.Tags,
                Rating = detalhesObra.Rating,
                RatingUsuario = detalhesObra.RatingUsuario,
                Description = detalhesObra.Description,
                Status = detalhesObra.Status,
                StatusNumber = detalhesObra.StatusNumber,
                Type = detalhesObra.Type,
                TypeNumber = detalhesObra.TypeNumber,
                PostedBy = detalhesObra.PostedBy,
                SeriesReleasedDate = detalhesObra.SeriesReleasedDate,
                SeriesLastUpdatedDate = detalhesObra.SeriesLastUpdatedDate,
                Views = detalhesObra.Views,
                Bookmark = detalhesObra.Bookmark,
                ChapterInfos = detalhesObra.ChapterInfos
            };
        }
    }
}