using System;
using System.Globalization;
using System.Windows.Data;

namespace ContosoMobile
{
    /// <summary>
    /// Converts a string to its upper case
    /// </summary>
    public class ToCapsConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (String)value.ToString().ToUpper();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (String)value.ToString().ToUpper();
        }
    }
}