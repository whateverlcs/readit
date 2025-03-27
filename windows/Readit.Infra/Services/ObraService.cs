using Readit.Core.Domain;
using Readit.Core.Repositories;
using Readit.Core.Services;

namespace Readit.Infra.Services
{
    public class ObraService : IObraService
    {
        private readonly IObraRepository _obraRepository;
        private readonly IImagemService _imagemService;
        private readonly IEnumService _enumService;
        private readonly IUtilService _utilService;

        public ObraService(IObraRepository obraRepository, IImagemService imagemService, IEnumService enumService, IUtilService utilService)
        {
            _obraRepository = obraRepository;
            _imagemService = imagemService;
            _enumService = enumService;
            _utilService = utilService;
        }

        public List<PostagensObras> FormatarDadosBookmarks(List<PostagensObras> postagens)
        {
            foreach (var postagem in postagens)
            {
                postagem.Image = _imagemService.ByteArrayToImage(postagem.ImageByte);
                postagem.TitleOriginal = postagem.Title.Trim();
                postagem.Title = postagem.Title.Length > 39 ? postagem.Title.Substring(0, 39).Trim() + "..." : postagem.Title.Trim();
            }

            return postagens;
        }

        public async Task<DetalhesObra> FormatarDadosDetalhamentoObra(string nomeObra)
        {
            var detalhesObra = await _obraRepository.BuscarDetalhesObraAsync(nomeObra).ConfigureAwait(false);

            detalhesObra.Image = _imagemService.ByteArrayToImage(detalhesObra.ImageByte);
            detalhesObra.Rating = Math.Round(detalhesObra.Rating, 1);
            detalhesObra.RatingUsuario = Math.Round(detalhesObra.RatingUsuario, 1);
            detalhesObra.Status = _enumService.ObterStatus(detalhesObra.StatusNumber);
            detalhesObra.Type = _enumService.ObterTipo(detalhesObra.TypeNumber);
            detalhesObra.Description = detalhesObra.Description.Length > 437 ? detalhesObra.Description.Substring(0, 437).Trim() + "..." : detalhesObra.Description.Trim();
            detalhesObra.PostedBy = detalhesObra.PostedBy.Length > 16 ? detalhesObra.PostedBy.Substring(0, 16).Trim() + "..." : detalhesObra.PostedBy.Trim();

            return detalhesObra;
        }

        public List<PostagensObras> FormatarDadosListagemObras(List<PostagensObras> postagens)
        {
            foreach (var postagem in postagens)
            {
                postagem.Image = _imagemService.ByteArrayToImage(postagem.ImageByte);
                postagem.TitleOriginal = postagem.Title.Trim();
                postagem.Title = postagem.Title.Length > 39 ? postagem.Title.Substring(0, 39).Trim() + "..." : postagem.Title.Trim();
                postagem.Rating = Math.Round(postagem.Rating, 1);
                postagem.Status = _enumService.ObterStatus(postagem.StatusNumber);
                postagem.Tipo = _enumService.ObterTipo(postagem.TipoNumber);
            }

            return postagens;
        }

        public async Task<List<DestaquesItem>> FormatarDadosObrasEmDestaques()
        {
            var obrasEmDestaque = await _obraRepository.BuscarObrasEmDestaqueAsync().ConfigureAwait(false);

            foreach (var obra in obrasEmDestaque)
            {
                obra.Image = _imagemService.ByteArrayToImage(obra.ImageByte);
                obra.Rating = Math.Round(obra.Rating, 1);
            }

            return obrasEmDestaque;
        }

        public List<PostagensObras> FormatarDadosUltimasAtualizacoes(List<PostagensObras> postagens)
        {
            foreach (var postagem in postagens)
            {
                postagem.Status = _enumService.ObterStatus(postagem.StatusNumber);
                postagem.Image = _imagemService.ByteArrayToImage(postagem.ImageByte);

                foreach (var capInfo in postagem.ChapterInfos)
                {
                    capInfo.TimeAgo = _utilService.FormatarData(capInfo.TimeAgoDate);
                }
            }

            return postagens;
        }
    }
}