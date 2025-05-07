using Readit.API.Communication.Responses;
using Readit.API.Exception;
using Readit.Core.Repositories;
using Readit.Core.Services;

namespace Readit.API.Application.UseCases.Obra.Consultar.Listagem
{
    public class ConsultarListagemObrasUseCase
    {
        private readonly IObraRepository _obraRepository;
        private readonly IEnumService _enumService;
        private readonly IImagemService _imagemService;

        public ConsultarListagemObrasUseCase(IObraRepository obraRepository, IEnumService enumService, IImagemService imagemService)
        {
            _obraRepository = obraRepository;
            _enumService = enumService;
            _imagemService = imagemService;
        }

        public async Task<ResponseConsultarListagemObrasJson> Execute()
        {
            var listagemObras = await _obraRepository.BuscarListagemObrasAsync().ConfigureAwait(false);

            if (listagemObras.Count == 0)
            {
                throw new InvalidActionException("Ocorreu um erro ao realizar a consulta da listagem das obras, tente novamente em segundos.");
            }

            ResponseConsultarListagemObrasJson response = new() { Listagem = [] };

            foreach (var listagem in listagemObras)
            {
                response.Listagem.Add(new Communication.Responses.Listagem
                {
                    IdObra = listagem.ObraId,
                    Nome = listagem.Title,
                    NomeAbreviado = listagem.Title.Length > 39 ? listagem.Title.Substring(0, 39).Trim() + "..." : listagem.Title.Trim(),
                    Imagem = listagem.ImageByte,
                    ImagemFlag = _imagemService.ConvertImageToByteArray(_enumService.ObterFlagPorTipo(listagem.TipoNumber)),
                    Rating = Math.Round(listagem.Rating, 1),
                    Status = _enumService.ObterStatus(listagem.StatusNumber),
                    Tipo = _enumService.ObterTipo(listagem.TipoNumber),
                    Generos = listagem.Genres,
                    DataPublicacao = listagem.DataPublicacao,
                    DataAtualizacao = listagem.DataAtualizacao
                });
            }

            return response;
        }
    }
}