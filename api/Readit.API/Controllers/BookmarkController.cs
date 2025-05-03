using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Readit.API.Application.UseCases.Bookmark;
using Readit.API.Application.UseCases.Login.FazerLogin;
using Readit.API.Application.UseCases.Registro;
using Readit.API.Communication.Requests;
using Readit.API.Communication.Responses;
using Readit.Core.Services;
using System.Security.Claims;

namespace Readit.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class BookmarkController : ControllerBase
    {
        private readonly RealizarBookmarkUseCase _realizarBookmarkUseCase;
        private readonly IUsuarioService _usuarioService;

        public BookmarkController(RealizarBookmarkUseCase realizarBookmarkUseCase, IUsuarioService usuarioService)
        {
            _realizarBookmarkUseCase = realizarBookmarkUseCase;
            _usuarioService = usuarioService;
        }

        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(ResponseRealizarBookmarkJson), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseRealizarBookmarkJson), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ResponseErrorMessagesJson), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseErrorMessagesJson), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseErrorMessagesJson), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PostBookmark(RequestRealizarBookmarkJson request)
        {
            _usuarioService.UsuarioLogado = new Core.Domain.Usuario { Id = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value) };

            var response = await _realizarBookmarkUseCase.Execute(request);

            return response.Equals("Adicionado") ? Created(string.Empty, response) : Ok(response);
        }
    }
}
