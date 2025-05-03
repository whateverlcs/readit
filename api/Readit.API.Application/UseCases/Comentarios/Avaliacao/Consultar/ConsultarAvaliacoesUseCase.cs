using Readit.API.Application.UseCases.Capitulo.ConsultaCompleta;
using Readit.API.Communication.Requests;
using Readit.API.Communication.Responses;
using Readit.API.Exception;
using Readit.Core.Repositories;
using Readit.Data.Models;
using Readit.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Readit.API.Application.UseCases.Comentarios.Avaliacao.Consultar
{
    public class ConsultarAvaliacoesUseCase
    {
        private readonly IComentarioRepository _comentarioRepository;

        public ConsultarAvaliacoesUseCase(IComentarioRepository comentarioRepository)
        {
            _comentarioRepository = comentarioRepository;
        }

        public async Task<ResponseConsultarAvaliacoesJson> Execute(RequestConsultarAvaliacoesJson request)
        {
            await Validate(request);

            var avaliacaoExistente = await _comentarioRepository.ConsultarLikesDeslikesUsuarioAsync(new Core.Domain.Comentarios { Id = request.IdComentario }, request.TipoAvaliacao).ConfigureAwait(false);

            return new ResponseConsultarAvaliacoesJson
            {
                PodeRealizarAvaliacao = avaliacaoExistente
            };
        }

        private async Task Validate(RequestConsultarAvaliacoesJson request)
        {
            var validator = new ConsultarAvaliacoesValidacao();

            var result = validator.Validate(request);

            var comentarioExistente = await _comentarioRepository.BuscarComentariosPorIdAsync(request.IdComentario);

            if (comentarioExistente.Count == 0)
                throw new NotFoundException("O comentário não existe no sistema.");

            if (!result.IsValid)
            {
                var errorMessages = result.Errors.Select(error => error.ErrorMessage).ToList();

                throw new ErrorOnValidationException(errorMessages);
            }
        }
    }
}
