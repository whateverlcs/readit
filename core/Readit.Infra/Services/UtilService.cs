using Readit.Core.Services;
using System.Globalization;
using System.Text;

namespace Readit.Infra.Services
{
    public class UtilService : IUtilService
    {
        public string FormatarData(DateTime? data)
        {
            if (data == null) return "Desconhecido";
            var diferenca = DateTime.Now - data.Value;

            if (diferenca.TotalMinutes < 60)
                return $"{(int)diferenca.TotalMinutes} min(s) atrás";
            if (diferenca.TotalHours < 24)
                return $"{(int)diferenca.TotalHours}h atrás";
            if (diferenca.TotalDays < 7)
                return $"{(int)diferenca.TotalDays} dia(s) atrás";

            return data.Value.ToString("dd/MM/yyyy");
        }

        public string RemoverAcentosEFormatar(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return texto;

            var textoNormalizado = texto.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (var c in textoNormalizado)
            {
                var categoria = CharUnicodeInfo.GetUnicodeCategory(c);
                if (categoria != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }

            var semAcentos = sb.ToString().Normalize(NormalizationForm.FormC);
            return semAcentos.Replace(' ', '-');
        }

        public void Shuffle<T>(List<T> list)
        {
            Random rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}