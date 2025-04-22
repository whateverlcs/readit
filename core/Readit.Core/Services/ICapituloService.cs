using Readit.Core.Domain;

namespace Readit.Core.Services
{
    public interface ICapituloService
    {
        Task<List<string>> IdentificarCapitulosExistentesBanco(List<CapitulosObra> listaCapitulosObra);
    }
}