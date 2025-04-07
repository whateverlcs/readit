﻿using Serilog;

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
                .WriteTo.File(Path.Combine(logDirectory, "log.txt"), rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7)
                .WriteTo.File(Path.Combine(logDirectory, "users_logged.txt"), rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7, restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information)
                .CreateLogger();
        }
    }
}