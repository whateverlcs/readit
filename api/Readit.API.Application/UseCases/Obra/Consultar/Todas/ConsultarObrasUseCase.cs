using Readit.API.Communication.Responses;
using Readit.API.Exception;
using Readit.Core.Repositories;

namespace Readit.API.Application.UseCases.Obra.Consultar.Todas
{
    public class ConsultarObrasUseCase
    {
        private readonly IObraRepository _obraRepository;

        public ConsultarObrasUseCase(IObraRepository obraRepository)
        {
            _obraRepository = obraRepository;
        }

        public async Task<ResponseConsultarObrasJson> Execute(int? idObra)
        {
            await Validate(idObra);

            var obras = await _obraRepository.BuscarObrasPorIdAsync(idObra).ConfigureAwait(false);

            if (obras.Count == 0)
            {
                throw new InvalidActionException("Ocorreu um erro ao realizar a consulta do gênero, tente novamente em segundos.");
            }

            return new ResponseConsultarObrasJson
            {
                Obras = obras
            };
        }

        private async Task Validate(int? idObra)
        {
            if (idObra != null && idObra != 0)
            {
                var obraExistente = await _obraRepository.BuscarObrasPorIdAsync(idObra);

                if (obraExistente.Count == 0)
                    throw new NotFoundException("A Obra não existe no sistema.");
            }
        }
    }
}