using GART.Controls;
using System;
using System.Windows;
using System.Windows.Data;

namespace BingMapsAR.Converters
{
    public class RotationConverter : DependencyObject, IValueConverter
    {
        // The property used as a parameter
        public ARDisplay ARDisplay
        {
            get { return (ARDisplay)GetValue(ARDisplayProperty); }
            set { SetValue(ARDisplayProperty, value); }
        }

        // The dependency property to allow the property to be used from XAML.
        public static readonly DependencyProperty ARDisplayProperty =
            DependencyProperty.Register(
            "ARDisplay",
            typeof(ARDisplay),
            typeof(RotationConverter),
            new PropertyMetadata(null));

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is double)
            {
                double angle = (double)value;

                switch (ARDisplay.Orientation)
                {
                    case GART.BaseControls.ControlOrientation.Clockwise90Degrees:
                        angle -= 90;
                        break;
                    case GART.BaseControls.ControlOrientation.Clockwise270Degrees:
                        angle += 90;
                        break;
                }

                return angle;
            }

            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
