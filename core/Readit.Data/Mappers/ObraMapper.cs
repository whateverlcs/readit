using Readit.Core.Domain;
using ef = Readit.Data.Models;

namespace Readit.Data.Mappers
{
    public static class ObraMapper
    {
        public static ef.Obra ToEntity(this Obras obra)
        {
            return new ef.Obra
            {
                ObsId = obra.Id,
                ObsNomeObra = obra.NomeObra,
                ObsStatus = obra.Status,
                ObsTipo = obra.Tipo,
                ObsDescricao = obra.Descricao,
                ObsDataPublicacao = obra.DataPublicacao,
                ObsDataAtualizacao = obra.DataAtualizacao,
                UsuId = obra.UsuarioId,
                ImgId = obra.ImagemId
            };
        }

        public static Obras ToDomain(this ef.Obra obra)
        {
            return new Obras
            {
                Id = obra.ObsId,
                NomeObra = obra.ObsNomeObra,
                Status = obra.ObsStatus,
                Tipo = obra.ObsTipo,
                Descricao = obra.ObsDescricao,
                DataPublicacao = obra.ObsDataPublicacao,
                DataAtualizacao = obra.ObsDataAtualizacao,
                UsuarioId = obra.UsuId,
                ImagemId = obra.ImgId
            };
        }

        public static List<Obras> ToDomainList(this ef.Obra[] obras)
        {
            return obras
                .Select(obraDB => new Obras
                {
                    Id = obraDB.ObsId,
                    NomeObra = obraDB.ObsNomeObra,
                    Status = obraDB.ObsStatus,
                    Tipo = obraDB.ObsTipo,
                    Descricao = obraDB.ObsDescricao,
                    DataAtualizacao = obraDB.ObsDataAtualizacao,
                    DataPublicacao = obraDB.ObsDataPublicacao,
                    ImagemId = obraDB.ImgId,
                    UsuarioId = obraDB.UsuId
                })
                .ToList();
        }
    }
}