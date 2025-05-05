using Readit.API.Communication.Responses;
using Readit.API.Exception;
using Readit.Core.Repositories;

namespace Readit.API.Application.UseCases.Genero.Consultar.PorObra
{
    public class ConsultarGenerosPorObraUseCase
    {
        private readonly IGeneroRepository _generoRepository;
        private readonly IObraRepository _obraRepository;

        public ConsultarGenerosPorObraUseCase(IGeneroRepository generoRepository, IObraRepository obraRepository)
        {
            _generoRepository = generoRepository;
            _obraRepository = obraRepository;
        }

        public async Task<ResponseConsultarGenerosJson> Execute(int? idObra)
        {
            await Validate(idObra);

            var genero = await _generoRepository.BuscarGenerosPorObraAsync(idObra).ConfigureAwait(false);

            if (genero.Count == 0)
            {
                throw new InvalidActionException("Ocorreu um erro ao realizar a consulta dos gêneros da obra, tente novamente em segundos.");
            }

            return new ResponseConsultarGenerosJson
            {
                ListaGeneros = genero
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