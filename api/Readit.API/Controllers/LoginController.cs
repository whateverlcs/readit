using Microsoft.AspNetCore.Mvc;
using Readit.API.Application.UseCases.Login.FazerLogin;
using Readit.API.Communication.Requests;
using Readit.API.Communication.Responses;

namespace Readit.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly RealizarLoginUseCase _realizarLoginUseCase;

        public LoginController(RealizarLoginUseCase realizarLoginUseCase)
        {
            _realizarLoginUseCase = realizarLoginUseCase;
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResponseRegistroUsuarioJson), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseErrorMessagesJson), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DoLogin(RequestLoginJson request)
        {
            var response = await _realizarLoginUseCase.Execute(request);

            return Ok(response);
        }
    }
}