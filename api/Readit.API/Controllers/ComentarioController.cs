using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Readit.API.Application.UseCases.Capitulo.ConsultaSimples;
using Readit.API.Application.UseCases.Comentarios.Avaliacao.Consultar;
using Readit.API.Application.UseCases.Comentarios.Avaliacao.Gerenciar;
using Readit.API.Application.UseCases.Comentarios.Cadastrar;
using Readit.API.Application.UseCases.Comentarios.Consultar;
using Readit.API.Application.UseCases.Comentarios.Editar;
using Readit.API.Application.UseCases.Comentarios.Excluir;
using Readit.API.Communication.Requests;
using Readit.API.Communication.Responses;
using Readit.Core.Services;
using System.Security.Claims;

namespace Readit.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ComentarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;
        private readonly ConsultarComentariosUseCase _consultarComentariosUseCase;
        private readonly ConsultarAvaliacoesUseCase _consultarAvaliacoesUseCase;
        private readonly CadastrarComentarioUseCase _cadastrarComentarioUseCase;
        private readonly EditarComentarioUseCase _editarComentarioUseCase;
        private readonly ExcluirComentarioUseCase _excluirComentarioUseCase;
        private readonly GerenciarAvaliacoesUseCase _gerenciarAvaliacoesUseCase;

        public ComentarioController(IUsuarioService usuarioService, ConsultarComentariosUseCase consultarComentariosUseCase, ConsultarAvaliacoesUseCase consultarAvaliacoesUseCase, CadastrarComentarioUseCase cadastrarComentarioUseCase, EditarComentarioUseCase editarComentarioUseCase, ExcluirComentarioUseCase excluirComentarioUseCase, GerenciarAvaliacoesUseCase gerenciarAvaliacoesUseCase)
        {
            _usuarioService = usuarioService;
            _consultarComentariosUseCase = consultarComentariosUseCase;
            _consultarAvaliacoesUseCase = consultarAvaliacoesUseCase;
            _cadastrarComentarioUseCase = cadastrarComentarioUseCase;
            _editarComentarioUseCase = editarComentarioUseCase;
            _excluirComentarioUseCase = excluirComentarioUseCase;
            _gerenciarAvaliacoesUseCase = gerenciarAvaliacoesUseCase;
        }

        [HttpGet("{obraId}")]
        [Authorize]
        [ProducesResponseType(typeof(ResponseConsultarComentariosJson), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseErrorMessagesJson), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseErrorMessagesJson), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseErrorMessagesJson), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetComentarios(int obraId, int? idCapitulo)
        {
            var result = await _consultarComentariosUseCase.Execute(obraId, idCapitulo);

            return Ok(result);
        }

        [HttpPost("consultar-avaliacao")]
        [Authorize]
        [ProducesResponseType(typeof(ResponseConsultarAvaliacoesJson), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseErrorMessagesJson), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseErrorMessagesJson), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseErrorMessagesJson), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAvaliacoes(RequestConsultarAvaliacoesJson request)
        {
            _usuarioService.UsuarioLogado = new Core.Domain.Usuario { Id = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value) };

            var result = await _consultarAvaliacoesUseCase.Execute(request);

            return Ok(result);
        }

        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(ResponseCadastrarComentarioJson), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ResponseErrorMessagesJson), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseErrorMessagesJson), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseErrorMessagesJson), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PostComentarios(RequestCadastrarComentarioJson request)
        {
            var result = await _cadastrarComentarioUseCase.Execute(request);

            return Created(string.Empty, result);
        }

        [HttpPut]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ResponseErrorMessagesJson), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseErrorMessagesJson), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseErrorMessagesJson), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutComentarios(RequestEditarComentarioJson request)
        {
            await _editarComentarioUseCase.Execute(request);

            return NoContent();
        }

        [HttpDelete("{idComentario}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ResponseErrorMessagesJson), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseErrorMessagesJson), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseErrorMessagesJson), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteComentarios(int idComentario)
        {
            await _excluirComentarioUseCase.Execute(idComentario);

            return NoContent();
        }

        [HttpPost("gerenciar-avaliacao")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ResponseErrorMessagesJson), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseErrorMessagesJson), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseErrorMessagesJson), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PostGerenciarAvaliacao(RequestGerenciarAvaliacoesComentarioJson request)
        {
            _usuarioService.UsuarioLogado = new Core.Domain.Usuario { Id = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value) };

            await _gerenciarAvaliacoesUseCase.Execute(request);

            return NoContent();
        }
    }
}
