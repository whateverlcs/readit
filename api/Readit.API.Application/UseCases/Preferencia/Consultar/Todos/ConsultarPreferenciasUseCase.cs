using Readit.API.Communication.Responses;
using Readit.API.Exception;
using Readit.Core.Repositories;

namespace Readit.API.Application.UseCases.Preferencia.Consultar.Todos
{
    public class ConsultarPreferenciasUseCase
    {
        private readonly IPreferenciasRepository _preferenciasRepository;

        public ConsultarPreferenciasUseCase(IPreferenciasRepository preferenciasRepository)
        {
            _preferenciasRepository = preferenciasRepository;
        }

        public async Task<ResponseConsultarPreferenciasJson> Execute()
        {
            var dadosPreferencias = await _preferenciasRepository.BuscarPreferenciasAsync().ConfigureAwait(false);

            if (dadosPreferencias.Count == 0)
            {
                throw new InvalidActionException("Ocorreu um erro ao realizar a consulta das preferências, tente novamente em segundos.");
            }

            return new ResponseConsultarPreferenciasJson
            {
                Preferencias = dadosPreferencias
            };
        }
    }
}