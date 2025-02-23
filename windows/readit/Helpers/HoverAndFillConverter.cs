using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace readit
{
    public class HoverAndFillConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2 || !(values[0] is bool isHovered) || !(values[1] is bool isFilled))
                return Brushes.Gray; // Cor padrão caso algo falhe

            if (isHovered)
                return Brushes.Gold; // Se estiver com hover, sempre fica dourado

            return isFilled ? Brushes.Yellow : Brushes.Gray; // Se estiver preenchida, amarelo; senão, cinza
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}