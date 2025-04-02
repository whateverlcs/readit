using Readit.Core.Domain;

namespace Readit.Core.Repositories
{
    public interface IObraRepository
    {
        public Task<List<Obras>> BuscarObrasPorIdAsync(int? idObra);

        public Task<PostagensObras> BuscarDadosObraPorIdAsync(int idObra);

        public Task<List<Obras>> BuscarObrasPorNomeAsync(string nomeObra);

        public Task<List<SlideshowItem>> BuscarObrasSlideShowAsync();

        public Task<List<PostagensObras>> BuscarObrasUltimasAtualizacoesAsync();

        public Task<List<PostagensObras>> BuscarObrasBookmarksAsync();

        public Task<List<PostagensObras>> BuscarListagemObrasAsync();

        public Task<List<DestaquesItem>> BuscarObrasEmDestaqueAsync();

        public Task<DetalhesObra> BuscarDetalhesObraAsync(string nomeObra);

        public Task<bool> CadastrarEditarObraAsync(Obras obra, Imagens imagem, List<Generos> listaGeneros);
    }
}