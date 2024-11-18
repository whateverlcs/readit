using Microsoft.Extensions.Configuration;
using System.IO;
using System.Windows;

namespace readit
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static IConfigurationRoot config;

        public App()
        {
            config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
        }

        public static string GetSetting(string key)
        {
            return config[$"Configuration:{key}"];
        }
    }
}