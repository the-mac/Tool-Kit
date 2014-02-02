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

#if WP7
using System.Device.Location;
using System.Windows.Data;
using Microsoft.Phone.Controls.Maps.Platform;
#endif

#if WP8
using System.Device.Location;
using Location = System.Device.Location.GeoCoordinate;
using System.Windows.Data;
using Microsoft.Phone.Maps.Controls;
#endif

#if WIN_RT
using Bing.Maps;
using Windows.UI.Xaml.Data;
#endif

using System;
using GART.Data;




namespace GART.Converters
{
    /// <summary>
    /// Allows the Bing map control to deal with GeoCoordinates that are unknown.
    /// </summary>
    /// <remarks>
    /// This converter works by replacing any Unknown GeoCoordinate with the GeoCoordinate for the North Pole during binding. If an item should not 
    /// be displayed when its GeoCoordinate is unknown, the <see cref="UnknownGeoVisibilityConverter"/> in combination with this converter.
    /// </remarks>
    public class UnknownGeoCoordinateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // Validate
            if (!(value is Location)) { throw new InvalidOperationException("Only Location is supported as a source."); }
            if (targetType != typeof(GeoCoordinate)) { throw new InvalidOperationException("Only GeoCoordinate is supported as the target type"); }

            // Cast to proper types
            var starting = (Location)value;

            // If it's unknown we need to return a new one
            if (starting.IsUnknown())
            {
                return ARDefaults.NorthPole;
            }
            else
            {
                return (GeoCoordinate)starting;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
