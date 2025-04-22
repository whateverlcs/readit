using Readit.Core.Domain;

namespace Readit.Data.Mappers
{
    public static class SlideshowMapper
    {
        public static List<SlideshowItem> ToDomainList(this IEnumerable<dynamic> obrasDB)
        {
            return obrasDB
                .Select(obraDB => new SlideshowItem
                {
                    Title = obraDB.NomeObra,
                    Chapter = $"Capítulo: {obraDB.UltimoCapitulo.ToString("D2")}",
                    Description = obraDB.Descricao,
                    Tags = obraDB.Generos,
                    BackgroundImageByte = obraDB.Imagem,
                    FocusedImageByte = obraDB.Imagem,
                })
                .ToList();
        }
    }
}