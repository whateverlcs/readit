using Readit.Core.Desktop.Domain;
using Readit.Core.Desktop.Services;
using Readit.Core.Domain;
using Readit.Infra.Desktop.Mappers;

namespace Readit.Infra.Desktop.Services
{
    public class CapituloDesktopService : ICapituloDesktopService
    {
        private readonly IImagemDesktopService _imagemDesktopService;

        public CapituloDesktopService(IImagemDesktopService imagemDesktopService)
        {
            _imagemDesktopService = imagemDesktopService;
        }

        public (List<CapitulosObraDesktop>, CapitulosObraDesktop) FormatarDadosPaginasCapitulo((List<CapitulosObra>, CapitulosObra) capitulos)
        {
            capitulos.Item1 = [.. capitulos.Item1.OrderBy(x => x.NumeroCapitulo)];

            List<CapitulosObraDesktop> listaCapDesktop = new List<CapitulosObraDesktop>();

            foreach (var capObra in capitulos.Item1)
            {
                listaCapDesktop.Add(capObra.DomainToDesktop());
            }

            CapitulosObraDesktop cap = capitulos.Item2.DomainToDesktop();

            List<PaginasCapituloDesktop> listaPagDesktop = new List<PaginasCapituloDesktop>();

            foreach (var pg in capitulos.Item2.ListaPaginas)
            {
                listaPagDesktop.Add(new PaginasCapituloDesktop
                {
                    Id = pg.Id,
                    NumeroPagina = pg.NumeroPagina,
                    Pagina = pg.Pagina,
                    Tamanho = pg.Tamanho,
                    CapituloId = pg.CapituloId,
                    PaginaImage = _imagemDesktopService.ByteArrayToImage(pg.Pagina)
                });
            }

            cap.ListaPaginasDesktop = listaPagDesktop;

            return (listaCapDesktop, cap);
        }
    }
}