using System;

#if WIN_RT
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml;
#elif WP8
using System.Windows.Data;
using System.Windows;
#endif

namespace BingMapsAR.Converters
{
    public class VisibilityConverter : IValueConverter
    {
        #if WIN_RT
        public object Convert(object value, Type targetType, object parameter, string language)
        #elif WP8
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        #endif
        {
            if (value != null)
            {
                if (value is bool)
                {
                    return (bool)value ? Visibility.Visible : Visibility.Collapsed;
                }

                return Visibility.Visible;
            }

            return Visibility.Collapsed;
        }

        #if WIN_RT
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        #elif WP8
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        #endif
        {
            throw new NotImplementedException();
        }
    }
}
