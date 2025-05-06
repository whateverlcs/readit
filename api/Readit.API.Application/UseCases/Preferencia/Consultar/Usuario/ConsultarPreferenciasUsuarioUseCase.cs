using Readit.API.Communication.Responses;
using Readit.Core.Repositories;

namespace Readit.API.Application.UseCases.Preferencia.Consultar.Usuario
{
    public class ConsultarPreferenciasUsuarioUseCase
    {
        private readonly IPreferenciasRepository _preferenciasRepository;

        public ConsultarPreferenciasUsuarioUseCase(IPreferenciasRepository preferenciasRepository)
        {
            _preferenciasRepository = preferenciasRepository;
        }

        public async Task<ResponseConsultarPreferenciasUsuarioJson> Execute()
        {
            var dadosPreferenciasUsuario = await _preferenciasRepository.BuscarPreferenciasUsuarioAsync().ConfigureAwait(false);

            return new ResponseConsultarPreferenciasUsuarioJson
            {
                PreferenciasUsuario = dadosPreferenciasUsuario
            };
        }
    }
}