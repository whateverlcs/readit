using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Readit.API.Application.UseCases.Genero.Consultar.PorNome;
using Readit.API.Application.UseCases.Genero.Consultar.PorObra;
using Readit.API.Application.UseCases.Genero.Gerenciar;
using Readit.API.Communication.Requests;
using Readit.API.Communication.Responses;

namespace Readit.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class GeneroController : ControllerBase
    {
        private readonly ConsultarGenerosPorNomeUseCase _consultarGenerosPorNomeUseCase;
        private readonly ConsultarGenerosPorObraUseCase _consultarGenerosPorObraUseCase;
        private readonly GerenciarGenerosUseCase _gerenciarGenerosUseCase;

        public GeneroController(ConsultarGenerosPorNomeUseCase consultarGenerosPorNomeUseCase, ConsultarGenerosPorObraUseCase consultarGenerosPorObraUseCase, GerenciarGenerosUseCase gerenciarGenerosUseCase)
        {
            _consultarGenerosPorNomeUseCase = consultarGenerosPorNomeUseCase;
            _consultarGenerosPorObraUseCase = consultarGenerosPorObraUseCase;
            _gerenciarGenerosUseCase = gerenciarGenerosUseCase;
        }

        [HttpPost("consultar-nome")]
        [Authorize]
        [ProducesResponseType(typeof(ResponseConsultarGenerosJson), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseErrorMessagesJson), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseErrorMessagesJson), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> PostGenerosPorNome(RequestConsultarGenerosPorNomeJson request)
        {
            var result = await _consultarGenerosPorNomeUseCase.Execute(request);

            return Ok(result);
        }

        [HttpGet("{obraId}/consultar")]
        [Authorize]
        [ProducesResponseType(typeof(ResponseConsultarGenerosJson), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseErrorMessagesJson), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseErrorMessagesJson), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseErrorMessagesJson), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PostGenerosPorObra(int? obraId)
        {
            var result = await _consultarGenerosPorObraUseCase.Execute(obraId);

            return Ok(result);
        }

        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ResponseErrorMessagesJson), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseErrorMessagesJson), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseErrorMessagesJson), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PostGeneros(RequestGerenciarGeneroJson request)
        {
            await _gerenciarGenerosUseCase.Execute(request);

            return NoContent();
        }
    }
}