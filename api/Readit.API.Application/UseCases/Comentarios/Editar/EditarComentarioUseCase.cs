using Readit.API.Application.UseCases.Comentarios.Cadastrar;
using Readit.API.Communication.Requests;
using Readit.API.Communication.Responses;
using Readit.API.Exception;
using Readit.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Readit.API.Application.UseCases.Comentarios.Editar
{
    public class EditarComentarioUseCase
    {
        private readonly IComentarioRepository _comentarioRepository;
        private readonly IObraRepository _obraRepository;

        public EditarComentarioUseCase(IComentarioRepository comentarioRepository, IObraRepository obraRepository)
        {
            _comentarioRepository = comentarioRepository;
            _obraRepository = obraRepository;
        }

        public async Task<bool> Execute(RequestEditarComentarioJson request)
        {
            await Validate(request);

            var sucesso = await _comentarioRepository.EditarComentarioAsync(new Core.Domain.Comentarios
            {
                Id = request.Id,
                TempoDecorrido = request.Data,
                TempoUltimaAtualizacaoDecorrido = request.DataAtualizacao,
                IdObra = request.ObraId,
                IdCapitulo = request.CapituloId == 0 ? null : request.CapituloId,
                IdUsuario = request.UsuarioId,
                ComentarioTexto = request.Comentario
            }).ConfigureAwait(false);

            if (!sucesso)
            {
                throw new InvalidActionException("Ocorreu um erro ao realizar a edição do comentário, tente novamente em segundos.");
            }

            return sucesso;
        }

        private async Task Validate(RequestEditarComentarioJson request)
        {
            var validator = new EditarComentarioValidacao();

            var result = validator.Validate(request);

            var obraExistente = await _obraRepository.BuscarObrasPorIdAsync(request.ObraId);

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
