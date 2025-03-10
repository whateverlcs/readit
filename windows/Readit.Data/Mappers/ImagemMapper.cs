using Readit.Core.Domain;
using ef = Readit.Data.Models;

namespace Readit.Data.Mappers
{
    public static class ImagemMapper
    {
        public static ef.Imagen ToEntity(this Imagens imagem)
        {
            return new ef.Imagen
            {
                ImgId = imagem.Id,
                ImgFormato = imagem.Formato,
                ImgDataInclusao = imagem.DataInclusao,
                ImgDataAtualizacao = imagem.DataAtualizacao,
                ImgImagem = imagem.Imagem,
                ImgTipo = imagem.Tipo
            };
        }

        public static Imagens ToDomain(this ef.Imagen imagem)
        {
            return new Imagens
            {
                Id = imagem.ImgId,
                Formato = imagem.ImgFormato,
                DataInclusao = imagem.ImgDataInclusao,
                DataAtualizacao = imagem.ImgDataAtualizacao,
                Imagem = imagem.ImgImagem,
                Tipo = imagem.ImgTipo
            };
        }

        public static List<Imagens> ToDomainList(this ef.Imagen[] imagem)
        {
            return imagem
                .Select(img => new Imagens
                {
                    Id = img.ImgId,
                    Imagem = img.ImgImagem,
                    Formato = img.ImgFormato,
                    DataInclusao = img.ImgDataInclusao,
                    DataAtualizacao = img.ImgDataAtualizacao,
                    Tipo = img.ImgTipo,
                })
                .ToList();
        }
    }
}