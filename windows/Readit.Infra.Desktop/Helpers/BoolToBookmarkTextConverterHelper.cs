using System.Globalization;
using System.Windows.Data;

namespace Readit.Infra.Desktop.Helpers
{
    public class BoolToBookmarkTextConverterHelper : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? "Bookmarked" : "Bookmark";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString() == "Bookmarked";
        }
    }
}