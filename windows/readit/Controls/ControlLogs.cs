using System.IO;

namespace readit.Controls
{
    public class ControlLogs
    {
#if DEBUG
        public string log = Directory.GetCurrentDirectory();
#else
        public string log = App.GetSetting("log");
#endif

        public void RealizarLogExcecao(string exception, string localException)
        {
            string err = $"{DateTime.Now} | Local: {localException} | Exceção: {exception}\n\n";
            File.WriteAllText(@$"{log}\Erros.txt", err);
        }
    }
}