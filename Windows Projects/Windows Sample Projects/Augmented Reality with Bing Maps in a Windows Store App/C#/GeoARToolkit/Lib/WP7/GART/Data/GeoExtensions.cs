#if WP7
using Microsoft.Phone.Controls.Maps.Platform;
#endif
#if WP8
using Microsoft.Phone.Maps.Controls;
using Location = System.Device.Location.GeoCoordinate;
#endif
#if WIN_RT
using Bing.Maps;
#endif

namespace GART
{
    static public class GeoExtensions
    {
        static public bool IsUnknown(this Location location)
        {
            #if WINDOWS_PHONE
            return (location.Latitude == 0 && location.Longitude == 0 && location.Altitude == 0);
            #else
            return (location.Latitude == 0 && location.Longitude == 0);
            #endif
        }
    }
}
