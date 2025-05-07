using Readit.API.Communication.Requests;
using Readit.API.Communication.Responses;
using Readit.API.Exception;
using Readit.Core.Repositories;
using Readit.Core.Services;

namespace Readit.API.Application.UseCases.Obra.Consultar.Detalhes
{
    public class ConsultarDetalhesObraUseCase
    {
        private readonly IObraRepository _obraRepository;
        private readonly IEnumService _enumService;
        private readonly IImagemService _imagemService;

        public ConsultarDetalhesObraUseCase(IObraRepository obraRepository, IEnumService enumService, IImagemService imagemService)
        {
            _obraRepository = obraRepository;
            _enumService = enumService;
            _imagemService = imagemService;
        }

        public async Task<ResponseConsultarDetalhesObraJson> Execute(RequestConsultarDetalhesObraJson request)
        {
            Validate(request);

            var detalhesObra = await _obraRepository.BuscarDetalhesObraAsync(request.NomeObra).ConfigureAwait(false);

            if (detalhesObra.ObraId == 0)
            {
                throw new InvalidActionException("Ocorreu um erro ao realizar a consulta dos detalhes da obra, tente novamente em segundos.");
            }

            return new ResponseConsultarDetalhesObraJson
            {
                IdObra = detalhesObra.ObraId,
                Nome = detalhesObra.Title,
                Descricao = detalhesObra.Description.Length > 437 ? detalhesObra.Description.Substring(0, 437).Trim() + "..." : detalhesObra.Description.Trim(),
                Status = _enumService.ObterStatus(detalhesObra.StatusNumber),
                Tipo = _enumService.ObterTipo(detalhesObra.TypeNumber),
                DataPublicacao = detalhesObra.SeriesReleasedDate,
                DataAtualizacao = detalhesObra.SeriesLastUpdatedDate,
                PostadoPor = detalhesObra.PostedBy.Length > 16 ? detalhesObra.PostedBy.Substring(0, 16).Trim() + "..." : detalhesObra.PostedBy.Trim(),
                Rating = Math.Round(detalhesObra.Rating, 1),
                Views = detalhesObra.Views,
                Imagem = detalhesObra.ImageByte,
                ImagemFlag = _imagemService.ConvertImageToByteArray(_enumService.ObterFlagPorTipo(detalhesObra.TypeNumber)),
                Generos = detalhesObra.Tags,
                Bookmark = detalhesObra.Bookmark,
                RatingUsuario = Math.Round(detalhesObra.RatingUsuario, 1),
                Capitulos = detalhesObra.ChapterInfos
            };
        }

        private void Validate(RequestConsultarDetalhesObraJson request)
        {
            var validator = new ConsultarDetalhesObraValidacao();

            var result = validator.Validate(request);

            if (!result.IsValid)
            {
                var errorMessages = result.Errors.Select(error => error.ErrorMessage).ToList();

                throw new ErrorOnValidationException(errorMessages);
            }
        }
    }
}