using Readit.Core.Domain;

namespace Readit.Core.Services
{
    public interface IObraService
    {
        List<PostagensObras> FormatarDadosUltimasAtualizacoes(List<PostagensObras> postagens);

        List<PostagensObras> FormatarDadosBookmarks(List<PostagensObras> postagens);

        List<PostagensObras> FormatarDadosListagemObras(List<PostagensObras> postagens);

        Task<List<DestaquesItem>> FormatarDadosObrasEmDestaques();

        Task<DetalhesObra> FormatarDadosDetalhamentoObra(string nomeObra);
    }
}