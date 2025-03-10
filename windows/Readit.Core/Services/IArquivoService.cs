using Readit.Core.Domain;

namespace Readit.Core.Services
{
    public interface IArquivoService
    {
        List<CapitulosObra> IdentificarArquivosInseridos(int obraId);

        void DeletarArquivosPastaTemporaria(DirectoryInfo di);

        void CriarPastaControle();

        List<string> ExtrairDadosFrasesLoading();
    }
}