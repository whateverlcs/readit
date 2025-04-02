using Readit.Core.Domain;
using Readit.Data.Models;

namespace Readit.Data.Mappers
{
    public static class PostagensObrasMapper
    {
        public static List<PostagensObras> MapUltimasAtualizacoes(IEnumerable<dynamic> obrasDB)
        {
            return obrasDB.Select(obraDB =>
            {
                var capitulos = (obraDB.Capitulos as IEnumerable<dynamic>) ?? new List<dynamic>();

                return new PostagensObras
                {
                    ObraId = obraDB.Id,
                    ImageByte = obraDB.Imagem,
                    Title = obraDB.NomeObra,
                    StatusNumber = obraDB.Status,
                    TipoNumber = Convert.ToInt32(obraDB.Tipo),
                    ChapterInfos = capitulos.Select(cap => new ChapterInfo
                    {
                        ChapterId = cap.Id,
                        ObraId = obraDB.Id,
                        Chapter = $"• Capítulo {cap.Numero:D2}",
                        TimeAgoDate = cap.Data
                    }).ToList()
                };
            }).ToList();
        }

        public static List<PostagensObras> MapBookmarks(IEnumerable<dynamic> obrasDB)
        {
            return obrasDB.Select(obraDB => new PostagensObras
            {
                ObraId = obraDB.Id,
                ImageByte = obraDB.Imagem,
                Title = obraDB.NomeObra,
                TipoNumber = Convert.ToInt32(obraDB.Tipo)
            }).ToList();
        }

        public static List<PostagensObras> MapListagemObras(IEnumerable<dynamic> obrasDB)
        {
            return obrasDB.Select(obraDB => new PostagensObras
            {
                ObraId = obraDB.Id,
                ImageByte = obraDB.Imagem,
                Title = obraDB.NomeObra,
                Rating = obraDB.Rating,
                StatusNumber = Convert.ToInt32(obraDB.Status),
                TipoNumber = Convert.ToInt32(obraDB.Tipo),
                Genres = obraDB.Genres,
                DataPublicacao = obraDB.DataPublicacao,
                DataAtualizacao = obraDB.DataAtualizacao
            }).ToList();
        }

        public static PostagensObras MapCadastroEdicaoObras(dynamic obraDB)
        {
            return new PostagensObras
            {
                ObraId = obraDB.Id,
                ImageByte = obraDB.Imagem,
                Title = obraDB.NomeObra,
                StatusNumber = Convert.ToInt32(obraDB.Status),
                TipoNumber = Convert.ToInt32(obraDB.Tipo),
                Genres = obraDB.Genres,
                Descricao = obraDB.Descricao
            };
        }
    }
}