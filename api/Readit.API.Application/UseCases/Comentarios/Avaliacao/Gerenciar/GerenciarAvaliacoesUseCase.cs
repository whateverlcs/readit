using Readit.API.Application.UseCases.Comentarios.Editar;
using Readit.API.Communication.Requests;
using Readit.API.Exception;
using Readit.Core.Repositories;
using Readit.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Readit.API.Application.UseCases.Comentarios.Avaliacao.Gerenciar
{
    public class GerenciarAvaliacoesUseCase
    {
        private readonly IComentarioRepository _comentarioRepository;

        public GerenciarAvaliacoesUseCase(IComentarioRepository comentarioRepository)
        {
            _comentarioRepository = comentarioRepository;
        }

        public async Task<bool> Execute(RequestGerenciarAvaliacoesComentarioJson request)
        {
            await Validate(request);

            var sucesso = await _comentarioRepository.CadastrarRemoverAvaliacaoComentarioAsync(new Core.Domain.Comentarios { Id = request.ComentarioId }, request.TipoAvaliacao, request.TipoAcao).ConfigureAwait(false);

            if (!sucesso)
            {
                throw new InvalidActionException("Ocorreu um erro ao realizar a adição/exclusão da avaliação no comentário, tente novamente em segundos.");
            }

            return sucesso;
        }

        private async Task Validate(RequestGerenciarAvaliacoesComentarioJson request)
        {
            var validator = new GerenciarAvaliacoesValidacao();

            var result = validator.Validate(request);

            var comentarioExistente = await _comentarioRepository.BuscarComentariosPorIdAsync(request.ComentarioId);

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
