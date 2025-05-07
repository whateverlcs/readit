using Readit.API.Communication.Responses;
using Readit.API.Exception;
using Readit.Core.Repositories;
using Readit.Core.Services;

namespace Readit.API.Application.UseCases.Obra.Consultar.DadosObra
{
    public class ConsultarDadosObraUseCase
    {
        private readonly IObraRepository _obraRepository;
        private readonly IEnumService _enumService;

        public ConsultarDadosObraUseCase(IObraRepository obraRepository, IEnumService enumService)
        {
            _obraRepository = obraRepository;
            _enumService = enumService;
        }

        public async Task<ResponseConsultarDadosObraJson> Execute(int idObra)
        {
            await Validate(idObra);

            var dadosObra = await _obraRepository.BuscarDadosObraPorIdAsync(idObra).ConfigureAwait(false);

            if (dadosObra.ObraId == 0)
            {
                throw new InvalidActionException("Ocorreu um erro ao realizar dos dados da obra, tente novamente em segundos.");
            }

            return new ResponseConsultarDadosObraJson
            {
                IdObra = dadosObra.ObraId,
                Imagem = dadosObra.ImageByte,
                Nome = dadosObra.Title,
                Status = _enumService.ObterStatus(dadosObra.StatusNumber),
                Tipo = _enumService.ObterTipo(dadosObra.TipoNumber),
                Generos = dadosObra.Genres,
                Descricao = dadosObra.Descricao
            };
        }

        private async Task Validate(int idObra)
        {
            var obraExistente = await _obraRepository.BuscarObrasPorIdAsync(idObra);

            if (obraExistente.Count == 0)
                throw new NotFoundException("A Obra não existe no sistema.");
        }
    }
}