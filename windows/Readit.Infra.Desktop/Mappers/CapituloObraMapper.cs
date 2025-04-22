using Readit.Core.Desktop.Domain;
using Readit.Core.Domain;

namespace Readit.Infra.Desktop.Mappers
{
    public static class CapituloObraMapper
    {
        public static CapitulosObraDesktop DomainToDesktop(this CapitulosObra capObra)
        {
            return new CapitulosObraDesktop
            {
                Id = capObra.Id,
                NomeObra = capObra.NomeObra,
                NumeroCapitulo = capObra.NumeroCapitulo,
                NumeroCapituloDisplay = capObra.NumeroCapituloDisplay,
                CaminhoArquivo = capObra.CaminhoArquivo,
                DataPublicacao = capObra.DataPublicacao,
                DataAtualizacao = capObra.DataAtualizacao,
                UsuarioId = capObra.UsuarioId,
                ObraId = capObra.ObraId,
                ListaPaginas = capObra.ListaPaginas
            };
        }
    }
}