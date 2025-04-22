using Readit.Core.Desktop.Domain;
using Readit.Core.Domain;

namespace Readit.Core.Desktop.Services
{
    public interface IObraDesktopService
    {
        List<PostagensObrasDesktop> FormatarDadosUltimasAtualizacoes(List<PostagensObras> postagens);

        List<PostagensObrasDesktop> FormatarDadosBookmarks(List<PostagensObras> postagens);

        List<PostagensObrasDesktop> FormatarDadosListagemObras(List<PostagensObras> postagens);

        List<SlideshowItemDesktop> FormatarDadosSlideshow(List<SlideshowItem> obras);

        Task<List<DestaquesItemDesktop>> FormatarDadosObrasEmDestaques();

        Task<DetalhesObraDesktop> FormatarDadosDetalhamentoObra(string nomeObra);
    }
}