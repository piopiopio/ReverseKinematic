using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace ReverseKinematic
{
    public class BaseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)value-15;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}


