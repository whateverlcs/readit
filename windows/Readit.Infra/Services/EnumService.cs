using Readit.Core.Enums;
using Readit.Core.Services;

namespace Readit.Infra.Services
{
    public class EnumService : IEnumService
    {
        public string ObterStatus(int status)
        {
            return status switch
            {
                (int)EnumObra.StatusObra.EmAndamento => "Em Andamento",
                (int)EnumObra.StatusObra.EmHiato => "Em Hiato",
                (int)EnumObra.StatusObra.Finalizado => "Finalizado",
                (int)EnumObra.StatusObra.Cancelado => "Cancelado",
                (int)EnumObra.StatusObra.Dropado => "Dropado",
                _ => "Desconhecido"
            };
        }

        public string ObterTipo(int tipo)
        {
            return tipo switch
            {
                (int)EnumObra.TipoObra.Manhwa => "Manhwa",
                (int)EnumObra.TipoObra.Donghua => "Donghua",
                (int)EnumObra.TipoObra.Manga => "Manga",
                _ => "Desconhecido"
            };
        }

        public string ObterFlagPorTipo(int tipo)
        {
            string pathFullName = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;

            return tipo switch
            {
                (int)EnumObra.TipoObra.Manhwa => Path.Combine(pathFullName, "Resources/Images", "korea-flag.png"),
                (int)EnumObra.TipoObra.Donghua => Path.Combine(pathFullName, "Resources/Images", "china-flag.png"),
                (int)EnumObra.TipoObra.Manga => Path.Combine(pathFullName, "Resources/Images", "japan-flag.png"),
                _ => Path.Combine(pathFullName, "Resources/Images", "unknown-flag.png")
            };
        }
    }
}