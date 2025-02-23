using System.Globalization;
using System.Windows.Data;

namespace readit
{
    public class StarConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double rating)
            {
                var stars = new List<bool>();

                // Preencher estrelas baseadas na nota
                for (int i = 0; i < 5; i++)
                {
                    stars.Add(i < rating); // Preenche até o número de estrelas correspondente
                }

                return stars;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}