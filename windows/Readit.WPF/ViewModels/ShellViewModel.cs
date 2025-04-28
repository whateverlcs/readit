using Caliburn.Micro;
using Readit.Core.Services;
using System.IO;

namespace Readit.WPF.ViewModels
{
    public class ShellViewModel : Conductor<IScreen>.Collection.OneActive
    {
        private readonly IDatabaseService _databaseService;
        private readonly IArquivoService _arquivoService;

        public ShellViewModel(IDatabaseService databaseService, IArquivoService arquivoService)
        {
            _databaseService = databaseService;
            _arquivoService = arquivoService;
            ActiveView.Parent = this;
        }

        protected override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            await base.OnActivateAsync(cancellationToken);

            _arquivoService.CriarPastaControle();

            if (!await _databaseService.TestarConexaoDBAsync())
            {
                await ActiveView.OpenItem<ErroViewModel>(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "alert.png"), "Não foi possível realizar a conexão com o banco de dados.");
                return;
            }

            await ActiveView.OpenItem<LoginViewModel>();
        }
    }
}