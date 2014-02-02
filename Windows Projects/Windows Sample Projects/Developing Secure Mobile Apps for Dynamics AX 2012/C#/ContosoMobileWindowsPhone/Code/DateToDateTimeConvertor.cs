using System;
using System.Globalization;
using System.Windows.Data;

namespace ContosoMobile
{
    /// <summary>
    /// Convert a DateTime value to its short date string representation
    /// </summary>
    public class DateToDateTimeConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((DateTime)value).ToShortDateString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (DateTime)value;
        }
    }
}