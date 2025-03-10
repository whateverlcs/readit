using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Readit.Infra.Helpers
{
    public class BoolToColorConverterHelper : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Caso 1: Conversão para estrelas (bool)
            if (value is bool isFilled && parameter == null)
            {
                return isFilled ? Brushes.Gold : Brushes.Gray; // Estrela preenchida ou não preenchida
            }

            // Caso 2: Comparação de filtros (string)
            if (value is string selectedFilter && parameter is string currentFilter)
            {
                return selectedFilter == currentFilter
                    ? new SolidColorBrush(Color.FromRgb(20, 33, 61))  // Filtro selecionado
                    : Brushes.Black; // Filtro não selecionado
            }

            return Brushes.Transparent; // Valor padrão para casos não tratados
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}