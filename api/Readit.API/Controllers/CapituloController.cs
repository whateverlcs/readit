using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Readit.API.Application.UseCases.Capitulo.ConsultaCompleta;
using Readit.API.Application.UseCases.Capitulo.ConsultaSimples;
using Readit.API.Application.UseCases.Capitulo.Gerenciar;
using Readit.API.Communication.Requests;
using Readit.API.Communication.Responses;
using Readit.Core.Services;
using System.Security.Claims;

namespace Readit.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CapituloController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;
        private readonly ConsultarCapituloCompletoUseCase _consultarCapituloCompletoUseCase;
        private readonly ConsultarCapituloSimplesUseCase _consultarCapituloSimplesUseCase;
        private readonly GerenciarCapituloUseCase _gerenciarCapituloUseCase;

        public CapituloController(IUsuarioService usuarioService, ConsultarCapituloCompletoUseCase consultarCapituloCompletoUseCase, ConsultarCapituloSimplesUseCase requestConsultarCapitulosSimples, GerenciarCapituloUseCase gerenciarCapituloUseCase)
        {
            _usuarioService = usuarioService;
            _consultarCapituloCompletoUseCase = consultarCapituloCompletoUseCase;
            _consultarCapituloSimplesUseCase = requestConsultarCapitulosSimples;
            _gerenciarCapituloUseCase = gerenciarCapituloUseCase;
        }

        [HttpPost("{obraId}/completo")]
        [Authorize]
        [ProducesResponseType(typeof(ResponseConsultarCapitulosCompletosJson), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseErrorMessagesJson), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseErrorMessagesJson), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseErrorMessagesJson), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCapitulosCompleto(int obraId, RequestConsultarCapitulosCompletoJson request)
        {
            var result = await _consultarCapituloCompletoUseCase.Execute(obraId, request);

            return Ok(result);
        }

        [HttpPost("{obraId}/simples")]
        [Authorize]
        [ProducesResponseType(typeof(ResponseConsultarCapitulosSimplesJson), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseErrorMessagesJson), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseErrorMessagesJson), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseErrorMessagesJson), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCapitulosSimples(int obraId, RequestConsultarCapitulosSimplesJson request)
        {
            var result = await _consultarCapituloSimplesUseCase.Execute(obraId, request);

            return Ok(result);
        }

        [HttpPost("capitulos")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ResponseErrorMessagesJson), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseErrorMessagesJson), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseErrorMessagesJson), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PostGerenciarCapitulos(RequestGerenciarCapitulosJson request)
        {
            _usuarioService.UsuarioLogado = new Core.Domain.Usuario { Id = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value) };

            await _gerenciarCapituloUseCase.Execute(request);

            return NoContent();
        }
    }
}