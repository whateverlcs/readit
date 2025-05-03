using Readit.API.Application.UseCases.AvaliacaoObra.Editar;
using Readit.API.Communication.Requests;
using Readit.API.Communication.Responses;
using Readit.API.Exception;
using Readit.Core.Repositories;
using Readit.Core.Security.Cryptography;
using Readit.Core.Services;
using Readit.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Readit.API.Application.UseCases.Bookmark
{
    public class RealizarBookmarkUseCase
    {
        private readonly IBookmarkRepository _bookmarkRepository;
        private readonly IObraRepository _obraRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IUsuarioService _usuarioService;

        public RealizarBookmarkUseCase(IBookmarkRepository bookmarkRepository, IObraRepository obraRepository, IUsuarioRepository usuarioRepository, IUsuarioService usuarioService)
        {
            _bookmarkRepository = bookmarkRepository;
            _obraRepository = obraRepository;
            _usuarioRepository = usuarioRepository;
            _usuarioService = usuarioService;
        }

        public async Task<ResponseRealizarBookmarkJson> Execute(RequestRealizarBookmarkJson request)
        {
            await Validate(request);

            var sucesso = await _bookmarkRepository.CadastrarRemoverBookmarkAsync(new Core.Domain.BookmarksUsuario
            {
                ObraId = request.ObraId,
                UsuarioId = request.UsuarioId,
            }).ConfigureAwait(false);

            if (!sucesso.Item1)
            {
                throw new InvalidActionException($"Ocorreu um erro ao realizar a {(sucesso.Item2 == "Removido" ? "remoção" : "adição")} do bookmark, tente novamente em segundos.");
            }

            return new ResponseRealizarBookmarkJson
            {
                Message = sucesso.Item2
            };
        }

        private async Task Validate(RequestRealizarBookmarkJson request)
        {
            var validator = new RealizarBookmarkValidacao();

            var result = validator.Validate(request);

            var obraExistente = await _obraRepository.BuscarObrasPorIdAsync(request.ObraId);

            if (obraExistente.Count == 0)
                throw new NotFoundException("A Obra não existe no sistema.");

            var usuarioExistente = await _usuarioRepository.BuscarUsuarioPorIdAsync(request.UsuarioId);

            if (usuarioExistente.Count == 0)
                throw new NotFoundException("O usuário não existe no sistema.");

            if (_usuarioService.UsuarioLogado.Id != request.UsuarioId)
                throw new ForbiddenOperationException("Você só pode modificar seus próprios recursos");

            if (!result.IsValid)
            {
                var errorMessages = result.Errors.Select(error => error.ErrorMessage).ToList();

                throw new ErrorOnValidationException(errorMessages);
            }
        }
    }
}
