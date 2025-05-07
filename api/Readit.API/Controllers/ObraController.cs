using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Readit.API.Application.UseCases.Obra.Consultar.Bookmark;
using Readit.API.Application.UseCases.Obra.Consultar.DadosObra;
using Readit.API.Application.UseCases.Obra.Consultar.Destaques;
using Readit.API.Application.UseCases.Obra.Consultar.Detalhes;
using Readit.API.Application.UseCases.Obra.Consultar.Listagem;
using Readit.API.Application.UseCases.Obra.Consultar.PorNome;
using Readit.API.Application.UseCases.Obra.Consultar.Slideshow;
using Readit.API.Application.UseCases.Obra.Consultar.Todas;
using Readit.API.Application.UseCases.Obra.Consultar.UltimasAtualizacoes;
using Readit.API.Application.UseCases.Obra.Gerenciar;
using Readit.API.Communication.Requests;
using Readit.API.Communication.Responses;
using Readit.Core.Services;
using System.Security.Claims;

namespace Readit.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ObraController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;
        private readonly ConsultarDadosObraUseCase _consultarDadosObraUseCase;
        private readonly ConsultarDetalhesObraUseCase _consultarDetalhesObraUseCase;
        private readonly ConsultarListagemObrasUseCase _consultarListagemObrasUseCase;
        private readonly ConsultarBookmarkObrasUseCase _consultarBookmarkObrasUseCase;
        private readonly ConsultarDestaquesObrasUseCase _consultarDestaquesObrasUseCase;
        private readonly ConsultarObrasUseCase _consultarObrasUseCase;
        private readonly ConsultarObrasPorNomeUseCase _consultarObrasPorNomeUseCase;
        private readonly ConsultarObrasSlideshowUseCase _consultarObrasSlideshowUseCase;
        private readonly ConsultarUltimasAtualizacoesUseCase _consultarUltimasAtualizacoesUseCase;
        private readonly GerenciarObrasUseCase _gerenciarObrasUseCase;

        public ObraController(IUsuarioService usuarioService, ConsultarDadosObraUseCase consultarDadosObraUseCase, ConsultarDetalhesObraUseCase consultarDetalhesObraUseCase, ConsultarListagemObrasUseCase consultarListagemObrasUseCase, ConsultarBookmarkObrasUseCase consultarBookmarkObrasUseCase, ConsultarDestaquesObrasUseCase consultarDestaquesObrasUseCase, ConsultarObrasUseCase consultarObrasUseCase, ConsultarObrasPorNomeUseCase consultarObrasPorNomeUseCase, ConsultarObrasSlideshowUseCase consultarObrasSlideshowUseCase, ConsultarUltimasAtualizacoesUseCase consultarUltimasAtualizacoesUseCase, GerenciarObrasUseCase gerenciarObrasUseCase)
        {
            _usuarioService = usuarioService;
            _consultarDadosObraUseCase = consultarDadosObraUseCase;
            _consultarDetalhesObraUseCase = consultarDetalhesObraUseCase;
            _consultarListagemObrasUseCase = consultarListagemObrasUseCase;
            _consultarBookmarkObrasUseCase = consultarBookmarkObrasUseCase;
            _consultarDestaquesObrasUseCase = consultarDestaquesObrasUseCase;
            _consultarObrasUseCase = consultarObrasUseCase;
            _consultarObrasPorNomeUseCase = consultarObrasPorNomeUseCase;
            _consultarObrasSlideshowUseCase = consultarObrasSlideshowUseCase;
            _consultarUltimasAtualizacoesUseCase = consultarUltimasAtualizacoesUseCase;
            _gerenciarObrasUseCase = gerenciarObrasUseCase;
        }

        [HttpGet("{obraId}/dados")]
        [Authorize]
        [ProducesResponseType(typeof(ResponseConsultarDadosObraJson), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseErrorMessagesJson), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseErrorMessagesJson), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseErrorMessagesJson), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDados(int obraId)
        {
            var result = await _consultarDadosObraUseCase.Execute(obraId);

            return Ok(result);
        }

        [HttpPost("consultar-detalhes")]
        [Authorize]
        [ProducesResponseType(typeof(ResponseConsultarDetalhesObraJson), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseErrorMessagesJson), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseErrorMessagesJson), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseErrorMessagesJson), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PostDetalhes(RequestConsultarDetalhesObraJson request)
        {
            _usuarioService.UsuarioLogado = new Core.Domain.Usuario { Id = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value) };

            var result = await _consultarDetalhesObraUseCase.Execute(request);

            return Ok(result);
        }

        [HttpGet("listagem")]
        [Authorize]
        [ProducesResponseType(typeof(ResponseConsultarListagemObrasJson), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseErrorMessagesJson), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseErrorMessagesJson), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetListagem()
        {
            var result = await _consultarListagemObrasUseCase.Execute();

            return Ok(result);
        }

        [HttpGet("bookmarks")]
        [Authorize]
        [ProducesResponseType(typeof(ResponseConsultarBookmarkObrasJson), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseErrorMessagesJson), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseErrorMessagesJson), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetBookmarks()
        {
            _usuarioService.UsuarioLogado = new Core.Domain.Usuario { Id = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value) };

            var result = await _consultarBookmarkObrasUseCase.Execute();

            return Ok(result);
        }

        [HttpGet("destaques")]
        [Authorize]
        [ProducesResponseType(typeof(ResponseConsultarDestaquesObrasJson), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseErrorMessagesJson), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseErrorMessagesJson), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetDestaques()
        {
            var result = await _consultarDestaquesObrasUseCase.Execute();

            return Ok(result);
        }

        [HttpGet("{obraId}")]
        [Authorize]
        [ProducesResponseType(typeof(ResponseConsultarObrasJson), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseErrorMessagesJson), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseErrorMessagesJson), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseErrorMessagesJson), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetObras(int? obraId)
        {
            var result = await _consultarObrasUseCase.Execute(obraId);

            return Ok(result);
        }

        [HttpPost("consultar-por-nome")]
        [Authorize]
        [ProducesResponseType(typeof(ResponseConsultarObrasPorNomeJson), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseErrorMessagesJson), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseErrorMessagesJson), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseErrorMessagesJson), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PostObrasPorNome(RequestConsultarObrasPorNomeJson request)
        {
            var result = await _consultarObrasPorNomeUseCase.Execute(request);

            return Ok(result);
        }

        [HttpGet("slideshow")]
        [Authorize]
        [ProducesResponseType(typeof(ResponseConsultarObrasSlideshowJson), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseErrorMessagesJson), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseErrorMessagesJson), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetSlideshow()
        {
            _usuarioService.UsuarioLogado = new Core.Domain.Usuario { Id = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value) };

            var result = await _consultarObrasSlideshowUseCase.Execute();

            return Ok(result);
        }

        [HttpGet("ultimas-atualizacoes")]
        [Authorize]
        [ProducesResponseType(typeof(ResponseConsultarUltimasAtualizacoesJson), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseErrorMessagesJson), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseErrorMessagesJson), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetUltimasAtualizacoes()
        {
            _usuarioService.UsuarioLogado = new Core.Domain.Usuario { Id = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value) };

            var result = await _consultarUltimasAtualizacoesUseCase.Execute();

            return Ok(result);
        }

        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ResponseErrorMessagesJson), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseErrorMessagesJson), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseErrorMessagesJson), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PostObras(RequestGerenciarObrasJson request)
        {
            await _gerenciarObrasUseCase.Execute(request);

            return NoContent();
        }
    }
}