namespace Readit.Core.Services
{
    public interface IEnumService
    {
        string ObterStatus(int status);

        string ObterTipo(int tipo);
        string ObterFlagPorTipo(int tipo);
    }
}