using Readit.API.Application.UseCases.AvaliacaoObra.Editar;
using Readit.API.Communication.Requests;
using Readit.API.Communication.Responses;
using Readit.API.Exception;
using Readit.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Readit.API.Application.UseCases.Comentarios.Cadastrar
{
    public class CadastrarComentarioUseCase
    {
        private readonly IComentarioRepository _comentarioRepository;
        private readonly IObraRepository _obraRepository;

        public CadastrarComentarioUseCase(IComentarioRepository comentarioRepository, IObraRepository obraRepository)
        {
            _comentarioRepository = comentarioRepository;
            _obraRepository = obraRepository;
        }

        public async Task<ResponseCadastrarComentarioJson> Execute(RequestCadastrarComentarioJson request)
        {
            await Validate(request);

            var sucesso = await _comentarioRepository.CadastrarComentarioAsync(new Core.Domain.Comentarios
            {
                TempoDecorrido = request.Data,
                TempoUltimaAtualizacaoDecorrido = request.DataAtualizacao,
                IdObra = request.ObraId,
                IdCapitulo = request.CapituloId == 0 ? null : request.CapituloId,
                IdUsuario = request.UsuarioId,
                ComentarioTexto = request.Comentario,
                Pai = request.ComentarioPaiId != null && request.ComentarioPaiId != 0 ? new Core.Domain.Comentarios { Id = (int)request.ComentarioPaiId } : null,
            }).ConfigureAwait(false);

            if (!sucesso.Item1)
            {
                throw new InvalidActionException("Ocorreu um erro ao realizar o cadastro do comentário, tente novamente em segundos.");
            }

            return new ResponseCadastrarComentarioJson
            {
                Sucesso = sucesso.Item1,
                IdComentario = sucesso.Item2
            };
        }

        private async Task Validate(RequestCadastrarComentarioJson request)
        {
            var validator = new CadastrarComentarioValidacao();

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
