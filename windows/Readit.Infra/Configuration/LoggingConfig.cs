using Serilog;
using Serilog.Events;

namespace Readit.Infra.Configuration
{
    public static class LoggingConfig
    {
        /// <summary>
        /// Realiza a configuração do Serilog e os arquivos de log que irão existir
        /// </summary>
        public static void ConfigureLogging()
        {
            string logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "logs");

            Log.Logger = new LoggerConfiguration()
            .WriteTo.File(
                Path.Combine(logDirectory, "errors.txt"),
                restrictedToMinimumLevel: LogEventLevel.Error,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7)

            .WriteTo.Logger(lc => lc
                .MinimumLevel.Information()
                .Filter.ByIncludingOnly(evt =>
                    evt.Properties.ContainsKey("LogType") &&
                    evt.Properties["LogType"].ToString() == "\"UserLogged\"")
                .WriteTo.File(
                    Path.Combine(logDirectory, "users_logged.txt"),
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 7,
                    outputTemplate: "{Message}{NewLine}"))
            .CreateLogger();
        }
    }
}