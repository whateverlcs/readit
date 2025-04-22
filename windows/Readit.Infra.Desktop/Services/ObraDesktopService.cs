using Readit.Core.Desktop.Domain;
using Readit.Core.Desktop.Services;
using Readit.Core.Domain;
using Readit.Core.Repositories;
using Readit.Core.Services;
using Readit.Infra.Desktop.Mappers;

namespace Readit.Infra.Desktop.Services
{
    public class ObraDesktopService : IObraDesktopService
    {
        private readonly IObraRepository _obraRepository;
        private readonly IImagemService _imagemService;
        private readonly IImagemDesktopService _imagemDesktopService;
        private readonly IEnumService _enumService;
        private readonly IUtilService _utilService;

        public ObraDesktopService(IObraRepository obraRepository, IImagemDesktopService imagemDesktopService, IImagemService imagemService, IEnumService enumService, IUtilService utilService)
        {
            _obraRepository = obraRepository;
            _imagemDesktopService = imagemDesktopService;
            _imagemService = imagemService;
            _enumService = enumService;
            _utilService = utilService;
        }

        public List<PostagensObrasDesktop> FormatarDadosBookmarks(List<PostagensObras> postagens)
        {
            List<PostagensObrasDesktop> listaPostagensObraDesktop = new List<PostagensObrasDesktop>();

            foreach (var postagem in postagens)
            {
                PostagensObrasDesktop postagemDesktop = postagem.DomainToDesktop();
                postagemDesktop.Image = _imagemDesktopService.ByteArrayToImage(postagem.ImageByte);
                postagemDesktop.TitleOriginal = postagem.Title.Trim();
                postagemDesktop.Title = postagem.Title.Length > 39 ? postagem.Title.Substring(0, 39).Trim() + "..." : postagem.Title.Trim();
                postagemDesktop.ImageFlag = _imagemDesktopService.ByteArrayToImage(_imagemService.ConvertImageToByteArray(_enumService.ObterFlagPorTipo(postagem.TipoNumber)));

                listaPostagensObraDesktop.Add(postagemDesktop);
            }

            return listaPostagensObraDesktop;
        }

        public async Task<DetalhesObraDesktop> FormatarDadosDetalhamentoObra(string nomeObra)
        {
            var detalhesObra = await _obraRepository.BuscarDetalhesObraAsync(nomeObra).ConfigureAwait(false);

            DetalhesObraDesktop detalhesObraDesktop = detalhesObra.DomainToDesktop();

            detalhesObraDesktop.Image = _imagemDesktopService.ByteArrayToImage(detalhesObra.ImageByte);
            detalhesObraDesktop.Rating = Math.Round(detalhesObra.Rating, 1);
            detalhesObraDesktop.RatingUsuario = Math.Round(detalhesObra.RatingUsuario, 1);
            detalhesObraDesktop.Status = _enumService.ObterStatus(detalhesObra.StatusNumber);
            detalhesObraDesktop.Type = _enumService.ObterTipo(detalhesObra.TypeNumber);
            detalhesObraDesktop.Description = detalhesObra.Description.Length > 437 ? detalhesObra.Description.Substring(0, 437).Trim() + "..." : detalhesObra.Description.Trim();
            detalhesObraDesktop.PostedBy = detalhesObra.PostedBy.Length > 16 ? detalhesObra.PostedBy.Substring(0, 16).Trim() + "..." : detalhesObra.PostedBy.Trim();
            detalhesObraDesktop.ImageFlag = _imagemDesktopService.ByteArrayToImage(_imagemService.ConvertImageToByteArray(_enumService.ObterFlagPorTipo(detalhesObra.TypeNumber)));

            return detalhesObraDesktop;
        }

        public List<PostagensObrasDesktop> FormatarDadosListagemObras(List<PostagensObras> postagens)
        {
            List<PostagensObrasDesktop> listaPostagens = new List<PostagensObrasDesktop>();

            foreach (var postagem in postagens)
            {
                PostagensObrasDesktop postagensObrasDesktop = postagem.DomainToDesktop();

                postagensObrasDesktop.Image = _imagemDesktopService.ByteArrayToImage(postagem.ImageByte);
                postagensObrasDesktop.TitleOriginal = postagem.Title.Trim();
                postagensObrasDesktop.Title = postagem.Title.Length > 39 ? postagem.Title.Substring(0, 39).Trim() + "..." : postagem.Title.Trim();
                postagensObrasDesktop.Rating = Math.Round(postagem.Rating, 1);
                postagensObrasDesktop.Status = _enumService.ObterStatus(postagem.StatusNumber);
                postagensObrasDesktop.Tipo = _enumService.ObterTipo(postagem.TipoNumber);
                postagensObrasDesktop.ImageFlag = _imagemDesktopService.ByteArrayToImage(_imagemService.ConvertImageToByteArray(_enumService.ObterFlagPorTipo(postagem.TipoNumber)));

                listaPostagens.Add(postagensObrasDesktop);
            }

            return listaPostagens;
        }

        public async Task<List<DestaquesItemDesktop>> FormatarDadosObrasEmDestaques()
        {
            var obrasEmDestaque = await _obraRepository.BuscarObrasEmDestaqueAsync().ConfigureAwait(false);

            List<DestaquesItemDesktop> listaDestaquesItemDesktop = new List<DestaquesItemDesktop>();

            foreach (var obra in obrasEmDestaque)
            {
                DestaquesItemDesktop destaqueItemDesktop = obra.DomainToDesktop();

                destaqueItemDesktop.Rating = Math.Round(obra.Rating, 1);
                destaqueItemDesktop.Image = _imagemDesktopService.ByteArrayToImage(obra.ImageByte);
                destaqueItemDesktop.ImageFlag = _imagemDesktopService.ByteArrayToImage(_imagemService.ConvertImageToByteArray(_enumService.ObterFlagPorTipo(obra.TipoNumber)));

                listaDestaquesItemDesktop.Add(destaqueItemDesktop);
            }

            return listaDestaquesItemDesktop;
        }

        public List<PostagensObrasDesktop> FormatarDadosUltimasAtualizacoes(List<PostagensObras> postagens)
        {
            List<PostagensObrasDesktop> listaPostagensObrasDesktop = new List<PostagensObrasDesktop>();

            foreach (var postagem in postagens)
            {
                PostagensObrasDesktop postagemObraDesktop = postagem.DomainToDesktop();

                postagemObraDesktop.Status = _enumService.ObterStatus(postagem.StatusNumber);
                postagemObraDesktop.Image = _imagemDesktopService.ByteArrayToImage(postagem.ImageByte);
                postagemObraDesktop.ImageFlag = _imagemDesktopService.ByteArrayToImage(_imagemService.ConvertImageToByteArray(_enumService.ObterFlagPorTipo(postagem.TipoNumber)));

                foreach (var capInfo in postagemObraDesktop.ChapterInfos)
                {
                    capInfo.TimeAgo = _utilService.FormatarData(capInfo.TimeAgoDate);
                }

                listaPostagensObrasDesktop.Add(postagemObraDesktop);
            }

            return listaPostagensObrasDesktop;
        }

        public List<SlideshowItemDesktop> FormatarDadosSlideshow(List<SlideshowItem> obras)
        {
            List<SlideshowItemDesktop> listaSlideshowDesktop = new List<SlideshowItemDesktop>();

            foreach (var obra in obras)
            {
                SlideshowItemDesktop slideshowDesktop = obra.DomainToDesktop();

                slideshowDesktop.BackgroundImage = _imagemDesktopService.ByteArrayToImage(obra.BackgroundImageByte);
                slideshowDesktop.FocusedImage = _imagemDesktopService.ByteArrayToImage(obra.FocusedImageByte);
                slideshowDesktop.Description = obra.Description.Length > 211 ? obra.Description.Substring(0, 211).Trim() + "..." : obra.Description.Trim();

                listaSlideshowDesktop.Add(slideshowDesktop);
            }

            return listaSlideshowDesktop;
        }
    }
}