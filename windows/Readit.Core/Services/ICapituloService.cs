using Readit.Core.Domain;

namespace Readit.Core.Services
{
    public interface ICapituloService
    {
        Task<List<string>> IdentificarCapitulosExistentesBanco(List<CapitulosObra> listaCapitulosObra);

        (List<CapitulosObra>, CapitulosObra) FormatarDadosPaginasCapitulo((List<CapitulosObra>, CapitulosObra) capitulos);
    }
}