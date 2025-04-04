using Readit.Core.Services;
using Serilog;

namespace Readit.Infra.Logging
{
    public class SerilogLogger : ILoggingService
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IUtilService _utilService;
        private readonly IConfiguracaoService _configuracaoService;

        public SerilogLogger(IUsuarioService usuarioService, IUtilService utilService, IConfiguracaoService configuracaoService)
        {
            _usuarioService = usuarioService;
            _utilService = utilService;
            _configuracaoService = configuracaoService;
        }

        public void LogError(Exception ex, string localException)
        {
            Log.Error(ex, $"{DateTime.Now} | Local: {localException}\n");
        }

        public void LogFilesChapterUploaded(List<string> listaCaminhoArquivos)
        {
            if (_usuarioService.ListaCapitulosSelecionados.Count > 0)
            {
                string logDirectory = _configuracaoService.GetPathFilesUploaded() + "\\capitulos enviados";

                if (!Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                }

                string pastaCriar = $"{logDirectory}\\{_utilService.RemoverAcentosEFormatar(_usuarioService.UsuarioLogado.Nome)}--{DateTime.Now:yyyy-MM-dd_HH-mm-ss}";

                Directory.CreateDirectory(pastaCriar);

                foreach (var caminho in _usuarioService.ListaCapitulosSelecionados)
                {
                    string nomeArquivo = Path.GetFileName(caminho);

                    File.Copy(caminho, pastaCriar + $"\\{nomeArquivo}");
                }
            }
        }

        public void LogUsersLogged()
        {
            Log.Information($"Usuário: {_usuarioService.UsuarioLogado.Nome}\n");
        }
    }
}