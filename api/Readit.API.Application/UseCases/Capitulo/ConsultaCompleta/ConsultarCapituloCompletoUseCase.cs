using Readit.API.Application.UseCases.AvaliacaoObra.Editar;
using Readit.API.Communication.Requests;
using Readit.API.Communication.Responses;
using Readit.API.Exception;
using Readit.Core.Repositories;
using Readit.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Readit.API.Application.UseCases.Capitulo.ConsultaCompleta
{
    public class ConsultarCapituloCompletoUseCase
    {
        private readonly ICapituloRepository _capituloRepository;
        private readonly IObraRepository _obraRepository;

        public ConsultarCapituloCompletoUseCase(ICapituloRepository capituloRepository, IObraRepository obraRepository)
        {
            _capituloRepository = capituloRepository;
            _obraRepository = obraRepository;
        }

        public async Task<ResponseConsultarCapitulosCompletosJson> Execute(int idObra, RequestConsultarCapitulosCompletoJson request)
        {
            await Validate(idObra, request);

            var dadosCapitulo = await _capituloRepository.BuscarCapituloObrasPorIdAsync(idObra, request.CapituloId, request.IncluirNumeroCapitulos, request.IncluirPaginasCapitulo).ConfigureAwait(false);

            if (dadosCapitulo.Item1.Count == 0 && dadosCapitulo.Item2.Id == 0)
            {
                throw new InvalidActionException("Ocorreu um erro ao realizar a consulta dos capítulos completos, tente novamente em segundos.");
            }

            return new ResponseConsultarCapitulosCompletosJson
            {
                ListaCapitulosObras = dadosCapitulo.Item1,
                CapituloObra = dadosCapitulo.Item2
            };
        }

        private async Task Validate(int idObra, RequestConsultarCapitulosCompletoJson request)
        {
            var validator = new ConsultarCapituloCompletoValidacao();

            var result = validator.Validate(request);

            var obraExistente = await _obraRepository.BuscarObrasPorIdAsync(idObra);

            if (obraExistente.Count == 0)
                throw new NotFoundException("A Obra não existe no sistema.");

            var capituloExistente = await _capituloRepository.BuscarCapituloObraPorIdAsync(request.CapituloId);

            if (capituloExistente.Count == 0)
                throw new NotFoundException("O capítulo não existe no sistema.");

            if (!result.IsValid)
            {
                var errorMessages = result.Errors.Select(error => error.ErrorMessage).ToList();

                throw new ErrorOnValidationException(errorMessages);
            }
        }
    }
}
