using Readit.Core.Domain;
using ef = Readit.Data.Models;

namespace Readit.Data.Mappers
{
    public static class GeneroMapper
    {
        public static ef.Genero ToEntity(this Generos genero)
        {
            return new ef.Genero
            {
                GnsId = genero.Id,
                GnsNome = genero.Nome,
            };
        }

        public static Generos ToDomain(this ef.Genero genero)
        {
            return new Generos
            {
                Id = genero.GnsId,
                Nome = genero.GnsNome
            };
        }

        public static List<Generos> ToDomainList(this ef.Genero[] generos)
        {
            return generos
                .Select(generoDB => new Generos
                {
                    Id = generoDB.GnsId,
                    Nome = generoDB.GnsNome,
                })
                .ToList();
        }
    }
}