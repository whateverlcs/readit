using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Readit.API.Application.UseCases.Preferencia.Consultar.Todos;
using Readit.API.Application.UseCases.Preferencia.Consultar.Usuario;
using Readit.API.Communication.Responses;
using Readit.Core.Services;
using System.Security.Claims;

namespace Readit.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PreferenciaController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;
        private readonly ConsultarPreferenciasUseCase _consultarPreferenciasUseCase;
        private readonly ConsultarPreferenciasUsuarioUseCase _consultarPreferenciasUsuarioUseCase;

        public PreferenciaController(IUsuarioService usuarioService, ConsultarPreferenciasUseCase consultarPreferenciasUseCase, ConsultarPreferenciasUsuarioUseCase consultarPreferenciasUsuarioUseCase)
        {
            _usuarioService = usuarioService;
            _consultarPreferenciasUseCase = consultarPreferenciasUseCase;
            _consultarPreferenciasUsuarioUseCase = consultarPreferenciasUsuarioUseCase;
        }

        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(ResponseConsultarPreferenciasJson), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseErrorMessagesJson), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseErrorMessagesJson), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetPreferencias()
        {
            var result = await _consultarPreferenciasUseCase.Execute();

            return Ok(result);
        }

        [HttpGet("usuario")]
        [Authorize]
        [ProducesResponseType(typeof(ResponseConsultarPreferenciasUsuarioJson), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseErrorMessagesJson), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetPreferenciasUsuario()
        {
            _usuarioService.UsuarioLogado = new Core.Domain.Usuario { Id = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value) };

            var result = await _consultarPreferenciasUsuarioUseCase.Execute();

            return Ok(result);
        }
    }
}