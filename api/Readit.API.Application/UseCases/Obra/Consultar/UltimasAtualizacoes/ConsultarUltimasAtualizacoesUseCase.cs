using Readit.API.Communication.Responses;
using Readit.API.Exception;
using Readit.Core.Repositories;
using Readit.Core.Services;

namespace Readit.API.Application.UseCases.Obra.Consultar.UltimasAtualizacoes
{
    public class ConsultarUltimasAtualizacoesUseCase
    {
        private readonly IObraRepository _obraRepository;
        private readonly IEnumService _enumService;
        private readonly IImagemService _imagemService;
        private readonly IUtilService _utilService;

        public ConsultarUltimasAtualizacoesUseCase(IObraRepository obraRepository, IEnumService enumService, IImagemService imagemService, IUtilService utilService)
        {
            _obraRepository = obraRepository;
            _enumService = enumService;
            _imagemService = imagemService;
            _utilService = utilService;
        }

        public async Task<ResponseConsultarUltimasAtualizacoesJson> Execute()
        {
            var ultimasAtualizacoesObras = await _obraRepository.BuscarObrasUltimasAtualizacoesAsync().ConfigureAwait(false);

            if (ultimasAtualizacoesObras.Count == 0)
            {
                throw new InvalidActionException("Ocorreu um erro ao realizar a consulta das últimas atualizações das obras, tente novamente em segundos.");
            }

            ResponseConsultarUltimasAtualizacoesJson response = new() { UltimasAtualizacoes = [] };

            foreach (var uao in ultimasAtualizacoesObras)
            {
                var ultimasAtt = new Communication.Responses.UltimasAtualizacoes
                {
                    IdObra = uao.ObraId,
                    Nome = uao.Title,
                    Imagem = uao.ImageByte,
                    ImagemFlag = _imagemService.ConvertImageToByteArray(_enumService.ObterFlagPorTipo(uao.TipoNumber)),
                    Status = _enumService.ObterStatus(uao.StatusNumber),
                    Tipo = _enumService.ObterTipo(uao.TipoNumber),
                };

                foreach (var cap in uao.ChapterInfos)
                {
                    cap.TimeAgo = _utilService.FormatarData(cap.TimeAgoDate);
                }

                ultimasAtt.Capitulos = uao.ChapterInfos;

                response.UltimasAtualizacoes.Add(ultimasAtt);
            }

            return response;
        }
    }
}