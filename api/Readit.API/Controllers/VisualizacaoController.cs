using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Readit.API.Application.UseCases.Visualizacao;
using Readit.API.Communication.Requests;
using Readit.API.Communication.Responses;

namespace Readit.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class VisualizacaoController : ControllerBase
    {
        private readonly GerenciarVisualizacaoUseCase _gerenciarVisualizacaoUseCase;

        public VisualizacaoController(GerenciarVisualizacaoUseCase gerenciarVisualizacaoUseCase)
        {
            _gerenciarVisualizacaoUseCase = gerenciarVisualizacaoUseCase;
        }

        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ResponseErrorMessagesJson), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseErrorMessagesJson), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseErrorMessagesJson), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PostVisualizacao(RequestGerenciarVisualizacaoJson request)
        {
            await _gerenciarVisualizacaoUseCase.Execute(request);

            return NoContent();
        }
    }
}