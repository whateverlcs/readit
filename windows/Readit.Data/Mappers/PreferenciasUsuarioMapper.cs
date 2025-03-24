using Readit.Core.Domain;
using ef = Readit.Data.Models;

namespace Readit.Data.Mappers
{
    public static class PreferenciasUsuarioMapper
    {
        public static List<ef.Preferencia> ToEntityList(this Preferencias[] preferenciasDB)
        {
            return preferenciasDB
                .Select(pref => new ef.Preferencia
                {
                    PreId = pref.Id,
                    PrePreferencia = pref.Nome
                })
                .ToList();
        }
    }
}