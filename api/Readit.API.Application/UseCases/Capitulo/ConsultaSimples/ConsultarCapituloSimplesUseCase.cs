using Readit.API.Application.UseCases.Capitulo.ConsultaCompleta;
using Readit.API.Communication.Requests;
using Readit.API.Communication.Responses;
using Readit.API.Exception;
using Readit.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Readit.API.Application.UseCases.Capitulo.ConsultaSimples
{
    public class ConsultarCapituloSimplesUseCase
    {
        private readonly ICapituloRepository _capituloRepository;
        private readonly IObraRepository _obraRepository;

        public ConsultarCapituloSimplesUseCase(ICapituloRepository capituloRepository, IObraRepository obraRepository)
        {
            _capituloRepository = capituloRepository;
            _obraRepository = obraRepository;
        }

        public async Task<ResponseConsultarCapitulosSimplesJson> Execute(int idObra, RequestConsultarCapitulosSimplesJson request)
        {
            await Validate(idObra, request);

            var dadosCapitulo = await _capituloRepository.BuscarCapituloObrasPorIdsAsync(request.ListaNumerosCapitulos, idObra).ConfigureAwait(false);

            return new ResponseConsultarCapitulosSimplesJson
            {
                ListaCapitulosObras = dadosCapitulo
            };
        }

        private async Task Validate(int idObra, RequestConsultarCapitulosSimplesJson request)
        {
            var validator = new ConsultarCapituloSimplesValidacao();

            var result = validator.Validate(request);

            var obraExistente = await _obraRepository.BuscarObrasPorIdAsync(idObra);

            if (obraExistente.Count == 0)
                throw new NotFoundException("A Obra não existe no sistema.");

            if (!result.IsValid)
            {
                var errorMessages = result.Errors.Select(error => error.ErrorMessage).ToList();

                throw new ErrorOnValidationException(errorMessages);
            }
        }
    }
}
