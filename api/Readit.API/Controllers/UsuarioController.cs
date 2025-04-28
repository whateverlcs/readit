using Microsoft.AspNetCore.Mvc;
using Readit.API.Application.UseCases.Registro;
using Readit.API.Communication.Requests;
using Readit.API.Communication.Responses;

namespace Readit.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly RegistroUsuarioUseCase _registroUsuarioUseCase;

        public UsuarioController(RegistroUsuarioUseCase registroUsuarioUseCase)
        {
            _registroUsuarioUseCase = registroUsuarioUseCase;
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResponseRegistroUsuarioJson), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ResponseErrorMessagesJson), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register(RequestUsuarioJson request)
        {
            var response = await _registroUsuarioUseCase.Execute(request);

            return Created(string.Empty, response);
        }
    }
}