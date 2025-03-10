namespace Readit.Core.Services
{
    public interface IDatabaseService
    {
        Task<bool> TestarConexaoDBAsync();
    }
}