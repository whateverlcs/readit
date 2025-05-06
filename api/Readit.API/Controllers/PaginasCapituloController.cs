using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Readit.API.Application.UseCases.PaginasCapitulo.Consultar;
using Readit.API.Communication.Requests;
using Readit.API.Communication.Responses;

namespace Readit.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PaginasCapituloController : ControllerBase
    {
        private readonly ConsultarPaginasCapituloPorCapituloUseCase _consultarPaginasCapituloPorCapituloUseCase;

        public PaginasCapituloController(ConsultarPaginasCapituloPorCapituloUseCase consultarPaginasCapituloPorCapituloUseCase)
        {
            _consultarPaginasCapituloPorCapituloUseCase = consultarPaginasCapituloPorCapituloUseCase;
        }

        [HttpPost("consultar-por-capitulos")]
        [Authorize]
        [ProducesResponseType(typeof(ResponseConsultarPaginasCapituloPorCapituloJson), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseErrorMessagesJson), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseErrorMessagesJson), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> PostPaginasCapituloPorId(RequestConsultarPaginasCapituloPorCapituloJson request)
        {
            var result = await _consultarPaginasCapituloPorCapituloUseCase.Execute(request);

            return Ok(result);
        }
    }
}