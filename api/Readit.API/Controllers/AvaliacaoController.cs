using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Readit.API.Application.UseCases.AvaliacaoObra.Consultar;
using Readit.API.Application.UseCases.AvaliacaoObra.Editar;
using Readit.API.Communication.Requests;
using Readit.API.Communication.Responses;
using Readit.Core.Services;
using System.Security.Claims;

namespace Readit.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AvaliacaoController : ControllerBase
    {
        private readonly ConsultarRatingUseCase _consultaRatingUseCase;
        private readonly EditarRatingUseCase _editarRatingUseCase;
        private readonly IUsuarioService _usuarioService;

        public AvaliacaoController(ConsultarRatingUseCase consultaRatingUseCase, EditarRatingUseCase editarRatingUseCase, IUsuarioService usuarioService)
        {
            _consultaRatingUseCase = consultaRatingUseCase;
            _editarRatingUseCase = editarRatingUseCase;
            _usuarioService = usuarioService;
        }

        [HttpGet("{obraId}")]
        [Authorize]
        [ProducesResponseType(typeof(ResponseConsultarRatingJson), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseErrorMessagesJson), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseErrorMessagesJson), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseErrorMessagesJson), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetRating(int obraId)
        {
            var request = new RequestConsultarRatingJson
            {
                IdObra = obraId
            };

            var result = await _consultaRatingUseCase.Execute(request);

            return Ok(result);
        }

        [HttpPut("{obraId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ResponseErrorMessagesJson), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseErrorMessagesJson), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseErrorMessagesJson), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutRating(int obraId, double rating)
        {
            _usuarioService.UsuarioLogado = new Core.Domain.Usuario { Id = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value) };

            var request = new RequestAtualizarRatingJson
            {
                IdObra = obraId,
                Rating = rating
            };

            await _editarRatingUseCase.Execute(request);

            return NoContent();
        }
    }
}