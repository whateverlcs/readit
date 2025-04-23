using Microsoft.Extensions.Configuration;
using Readit.Core.Services;

namespace Readit.Infra.Services
{
    public class ConfiguracaoService : IConfiguracaoService
    {
        private readonly IConfiguration _configuration;

        public ConfiguracaoService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetConnectionString()
        {
            return _configuration.GetConnectionString("ConnectionString");
        }

        public string GetLoginAdministrador()
        {
            return _configuration["Configuration:loginAdm"];
        }

        public string GetPathFilesUploaded()
        {
            return _configuration["Configuration:pathUploadedFiles"];
        }

        public string GetTempPath()
        {
            return _configuration["Configuration:temp"];
        }

        public string GetAPIKey()
        {
            return _configuration["Jwt:Key"];
        }
    }
}