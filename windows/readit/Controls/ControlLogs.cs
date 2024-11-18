using System.IO;

namespace readit.Controls
{
    public class ControlLogs
    {
        public string log = App.GetSetting("log");

        public void RealizarLogExcecao(string exception, string localException)
        {
            string err = $"{DateTime.Now} | Local: {localException} | Exceção: {exception}\n\n";
            File.WriteAllText(@$"{log}\Erros.txt", err);
        }
    }
}