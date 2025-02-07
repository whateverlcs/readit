using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                int fullStars = (int)Math.Ceiling(rating / 2); // Ex: 9.8 -> 5 estrelas preenchidas, 1.0 -> 1 estrela preenchida
                for (int i = 0; i < 5; i++)
                {
                    stars.Add(i < fullStars); // Preenche até o número de estrelas correspondente
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
