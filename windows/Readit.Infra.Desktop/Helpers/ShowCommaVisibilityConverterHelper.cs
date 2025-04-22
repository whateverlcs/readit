using Readit.Core.Desktop.Domain;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Readit.Infra.Desktop.Helpers
{
    public class ShowCommaVisibilityConverterHelper : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2 && values[0] is DestaquesItemDesktop item && values[1] is string genre)
            {
                // Chama o método ShowComma da ViewModel (ou diretamente se você estiver fazendo isso na ViewModel)
                return item.IsLastGenre(genre) ? Visibility.Collapsed : Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}