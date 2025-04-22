namespace Readit.Core.Services
{
    public interface IUtilService
    {
        string FormatarData(DateTime? data);

        void Shuffle<T>(List<T> list);

        string RemoverAcentosEFormatar(string texto);
    }
}