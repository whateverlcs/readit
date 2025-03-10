using Readit.Core.Domain;

namespace Readit.Data.Mappers
{
    public static class DetalhesObraMapper
    {
        public static DetalhesObra ToDomain(dynamic obraDB)
        {
            return new DetalhesObra
            {
                ObraId = obraDB.IdObra,
                Title = obraDB.NomeObra,
                Description = obraDB.Descricao,
                StatusNumber = Convert.ToInt32(obraDB.Status),
                TypeNumber = Convert.ToInt32(obraDB.Tipo),
                SeriesReleasedDate = obraDB.DataPublicacao,
                SeriesLastUpdatedDate = obraDB.DataAtualizacao,
                PostedBy = obraDB.NomeUsuario,
                Rating = obraDB.MediaNota,
                Views = obraDB.Views,
                ImageByte = obraDB.Imagem,
                Tags = obraDB.Generos,
                Bookmark = obraDB.Bookmark,
                ChapterInfos = (obraDB.Capitulos as IEnumerable<dynamic>)?.Select(cap => new ChapterInfo
                {
                    ChapterId = cap.Id,
                    ObraId = obraDB.IdObra,
                    Chapter = $"Capítulo {cap.Numero:D2}",
                    TimeAgo = cap.DataPublicacao
                }).ToList() ?? new List<ChapterInfo>()
            };
        }
    }
}