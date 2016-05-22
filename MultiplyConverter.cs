using System;
using System.Globalization; 
using System.Windows.Data;

namespace gripit_client
{
    public class MultiplyConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType,
               object parameter, CultureInfo culture)
        {
            var scale = (double) values[0];
            var desiredValue = (double)values[1];

            return scale * desiredValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes,
               object parameter, CultureInfo culture)
        {
            throw new Exception("Not implemented");
        }
    }
}