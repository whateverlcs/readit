using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Readit.API.Application.UseCases.Imagem;
using Readit.API.Communication.Responses;

namespace Readit.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ImagemController : ControllerBase
    {
        private readonly ConsultarImagensUseCase _consultarImagensUseCase;

        public ImagemController(ConsultarImagensUseCase consultarImagensUseCase)
        {
            _consultarImagensUseCase = consultarImagensUseCase;
        }

        [HttpGet("{imagemId}")]
        [Authorize]
        [ProducesResponseType(typeof(ResponseConsultarImagensJson), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseErrorMessagesJson), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseErrorMessagesJson), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseErrorMessagesJson), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetImagens(int imagemId)
        {
            var result = await _consultarImagensUseCase.Execute(imagemId);

            return Ok(result);
        }
    }
}