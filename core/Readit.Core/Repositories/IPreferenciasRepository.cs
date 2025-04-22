using Readit.Core.Domain;

namespace Readit.Core.Repositories
{
    public interface IPreferenciasRepository
    {
        public Task<List<PreferenciasUsuario>> BuscarPreferenciasUsuarioAsync();

        public Task<List<Preferencias>> BuscarPreferenciasAsync();
    }
}