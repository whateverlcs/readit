namespace Readit.Core.Services
{
    public interface IConfiguracaoService
    {
        string GetConnectionString();

        string GetLoginAdministrador();

        string GetTempPath();
    }
}