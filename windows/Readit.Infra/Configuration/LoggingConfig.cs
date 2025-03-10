using Serilog;

namespace Readit.Infra.Configuration
{
    public static class LoggingConfig
    {
        public static void ConfigureLogging()
        {
            string logPath = Path.Combine(Directory.GetCurrentDirectory(), "logs", "log.txt");

            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(logPath, rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7)
                .CreateLogger();
        }
    }
}