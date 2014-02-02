#region License
/******************************************************************************
 * COPYRIGHT © MICROSOFT CORP. 
 * MICROSOFT LIMITED PERMISSIVE LICENSE (MS-LPL)
 * This license governs use of the accompanying software. If you use the software, you accept this license. If you do not accept the license, do not use the software.
 * 1. Definitions
 * The terms “reproduce,” “reproduction,” “derivative works,” and “distribution” have the same meaning here as under U.S. copyright law.
 * A “contribution” is the original software, or any additions or changes to the software.
 * A “contributor” is any person that distributes its contribution under this license.
 * “Licensed patents” are a contributor’s patent claims that read directly on its contribution.
 * 2. Grant of Rights
 * (A) Copyright Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, each contributor grants you a non-exclusive, worldwide, royalty-free copyright license to reproduce its contribution, prepare derivative works of its contribution, and distribute its contribution or any derivative works that you create.
 * (B) Patent Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, each contributor grants you a non-exclusive, worldwide, royalty-free license under its licensed patents to make, have made, use, sell, offer for sale, import, and/or otherwise dispose of its contribution in the software or derivative works of the contribution in the software.
 * 3. Conditions and Limitations
 * (A) No Trademark License- This license does not grant you rights to use any contributors’ name, logo, or trademarks.
 * (B) If you bring a patent claim against any contributor over patents that you claim are infringed by the software, your patent license from such contributor to the software ends automatically.
 * (C) If you distribute any portion of the software, you must retain all copyright, patent, trademark, and attribution notices that are present in the software.
 * (D) If you distribute any portion of the software in source code form, you may do so only under this license by including a complete copy of this license with your distribution. If you distribute any portion of the software in compiled or object code form, you may only do so under a license that complies with this license.
 * (E) The software is licensed “as-is.” You bear the risk of using it. The contributors give no express warranties, guarantees or conditions. You may have additional consumer rights under your local laws which this license cannot change. To the extent permitted under your local laws, the contributors exclude the implied warranties of merchantability, fitness for a particular purpose and non-infringement.
 * (F) Platform Limitation- The licenses granted in sections 2(A) & 2(B) extend only to the software or derivative works that you create that run on a Microsoft Windows operating system product.
 ******************************************************************************/
#endregion // License

#if WINDOWS_PHONE
using Culture = System.Globalization.CultureInfo;
using System.Windows.Data;
#else
using Culture = System.String;
using Windows.UI.Xaml.Data;
#endif

using System;

namespace GART.Converters
{
    /// <summary>
    /// Converts a percentage to a Bing zoom level.
    /// </summary>
    public class PercentBingZoomConverter : IValueConverter
    {
        #region Constants
        private const double MinZoomLevel = 1;
        private const double MaxZoomLevel = 21;
        #endregion // Constants

        /// <summary>
        /// Converts a percentage to a Bing Maps zoom level.
        /// </summary>
        /// <param name="percentZoom">
        /// The percentage (between 0.0 and 1.0).
        /// </param>
        /// <returns>
        /// The Bing zoom level.
        /// </returns>
        static public double PercentToMapLevel(double percentZoom)
        {
            // Test range
            if ((percentZoom < 0d) || (percentZoom > 1d)) throw new ArgumentOutOfRangeException("Value should represnt a percent (between 0 and 1).");

            // Multiply by current level to get actual
            double bingZoom = Math.Round(percentZoom * MaxZoomLevel);

            // Make sure not less than minimum
            if (bingZoom < MinZoomLevel) { bingZoom = MinZoomLevel; }

            // Return Bing zoom
            return bingZoom;
        }

        /// <summary>
        /// Converts a bing zoom level to a percentage.
        /// </summary>
        /// <param name="bingZoom">
        /// The bing zoom level.
        /// </param>
        /// <returns>
        /// The converted percentage.
        /// </returns>
        static public double MapLevelToPercent(double bingZoom)
        {
            // Test range
            if ((bingZoom < MinZoomLevel) || (bingZoom > MaxZoomLevel)) throw new ArgumentOutOfRangeException(string.Format("Value should be between {0} and {1}.", MinZoomLevel, MaxZoomLevel));

            // Divide current by max to get percentage
            double percentZoom = bingZoom / MaxZoomLevel;

            // Return percentage zoom
            return percentZoom;
        }

        public object Convert(object value, Type targetType, object parameter, Culture culture)
        {
            // Validate
            if (!(value is double)) { throw new InvalidOperationException("Only double is supported as a source."); }
            if (targetType != typeof(double)) { throw new InvalidOperationException("Only double is supported as the target type"); }

            // Cast to proper types
            double percentZoom = (double)value;

            // Do conversion
            return PercentToMapLevel(percentZoom);
        }

        public object ConvertBack(object value, Type targetType, object parameter, Culture culture)
        {
            // Validate
            if (!(value is double)) { throw new InvalidOperationException("Only double is supported as a source."); }
            if (targetType != typeof(double)) { throw new InvalidOperationException("Only double is supported as the target type"); }

            // Cast to proper types
            double bingZoom = (double)value;

            // Do conversion
            return MapLevelToPercent(bingZoom);
        }
    }
}
