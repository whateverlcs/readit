using Readit.API.Communication.Responses;
using Readit.API.Exception;
using Readit.Core.Repositories;

namespace Readit.API.Application.UseCases.Imagem
{
    public class ConsultarImagensUseCase
    {
        private readonly IImagemRepository _imagemRepository;

        public ConsultarImagensUseCase(IImagemRepository imagemRepository)
        {
            _imagemRepository = imagemRepository;
        }

        public async Task<ResponseConsultarImagensJson> Execute(int idImagem)
        {
            await Validate(idImagem);

            var imagem = await _imagemRepository.BuscarImagemPorIdAsync(idImagem).ConfigureAwait(false);

            if (imagem.Count == 0)
            {
                throw new InvalidActionException("Ocorreu um erro ao realizar a consulta da imagem, tente novamente em segundos.");
            }

            return new ResponseConsultarImagensJson
            {
                Imagens = imagem
            };
        }

        private async Task Validate(int idImagem)
        {
            var imagemExistente = await _imagemRepository.BuscarImagemPorIdAsync(idImagem);

            if (idImagem == 0 || imagemExistente.Count == 0)
            {
                throw new NotFoundException("O id da imagem não existe");
            }
        }
    }
}