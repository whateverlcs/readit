using Readit.Core.Desktop.Domain;
using Readit.Core.Domain;

namespace Readit.Infra.Desktop.Mappers
{
    public static class PostagensObraMapper
    {
        public static PostagensObrasDesktop DomainToDesktop(this PostagensObras postagemObra)
        {
            return new PostagensObrasDesktop
            {
                ObraId = postagemObra.ObraId,
                ImageByte = postagemObra.ImageByte,
                Title = postagemObra.Title,
                TitleOriginal = postagemObra.TitleOriginal,
                Status = postagemObra.Status,
                Tipo = postagemObra.Tipo,
                Descricao = postagemObra.Descricao,
                TipoNumber = postagemObra.TipoNumber,
                Genres = postagemObra.Genres,
                Rating = postagemObra.Rating,
                DataPublicacao = postagemObra.DataPublicacao,
                DataAtualizacao = postagemObra.DataAtualizacao,
                StatusNumber = postagemObra.StatusNumber,
                ChapterInfos = postagemObra.ChapterInfos
            };
        }
    }
}