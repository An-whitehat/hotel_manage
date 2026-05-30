using System;
using System.Globalization;
using System.Windows.Data;

namespace QLKS.Helpers
{
    public class PercentToWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal percent && parameter is string param
                && double.TryParse(param, out double maxWidth))
            {
                return (double)(percent / 100m) * maxWidth;
            }
            return 0.0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}