#if WP7
using Culture = System.Globalization.CultureInfo;
using System.Windows.Data;
#else
using Culture = System.String;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml;
#endif

using System;

namespace GART.Converters
{
    public class MarginInflationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, Culture culture)
        {
            // Validate
            if (!(value is double)) { throw new InvalidOperationException("Only double is supported as a source."); }
            if (targetType != typeof(Thickness)) { throw new InvalidOperationException("Only Thickness is supported as the target type"); }

            // Cast to proper types
            double start = (double)value;
            double infalte = (double)parameter;

            // If it's unknown the item shouldn't be shown
            if (double.IsNaN(start))
            {
                return new Thickness(0);
            }
            else
            {
                return new Thickness(start * infalte);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, Culture culture)
        {
            throw new NotImplementedException();
        }
    }
}
