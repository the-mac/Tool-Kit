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

using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Linq;
using System.Xml.Linq;
using BingAR.Bing.Search;
using System.Device.Location;
using GART.Data;
using Microsoft.Phone.Controls.Maps.Platform;

using Location = Microsoft.Phone.Controls.Maps.Platform.Location;


namespace BingAR.Data
{
    static public class BingDataHelper
    {
        #region Constants
        private const string CuisinePropertyKey = "Cuisine";        
        private const int RestaurantPropertiesKey = 11168;
        private const string RooftopMethod = "Rooftop";
        private const string UnknownString = "Unknown";
        #endregion // Constants

        #region Public Methods
        /// <summary>
        /// Gets a <see cref="GeoCoordinate"/> for the Bing search result
        /// </summary>
        /// <param name="result">
        /// The search result to get the coordinates for.
        /// </param>
        /// <returns>
        /// The geo point for the search result.
        /// </returns>
        static public Location GetLocation(SearchResultBase result)
        {
            // Try to find the location that was recorded in "rooftop" mode
            var bingLocation = (from l in result.LocationData.Locations
                               where l.CalculationMethod == RooftopMethod
                                    select l).FirstOrDefault();

            // If rooftop wasn't found, take first available              
            if (bingLocation == null)
            {
                bingLocation = result.LocationData.Locations.FirstOrDefault();
            }

            // Do we have a location to work with?
            if (bingLocation != null)
            {
                return new Location()
                {
                    Latitude = bingLocation.Latitude,
                    Longitude = bingLocation.Longitude,
                    Altitude = bingLocation.Altitude
                };
            }
            else
            {
                return new Location();
            }
        }

        /// <summary>
        /// Gets the name of the primary cuisine for the Bing search result.
        /// </summary>
        /// <param name="result">
        /// The search result to get the cuisine for.
        /// </param>
        /// <returns>
        /// The name of the cuisine for the search result.
        /// </returns>
        static public string GetCuisineName(BusinessSearchResult result)
        {
            // Try to get the cuisine property
            var properties = result.CategorySpecificProperties;

            // Do we have restaurant properties?
            if (properties.ContainsKey(RestaurantPropertiesKey))
            {
                // Get the restaurant properties
                var restaurantProps = properties[RestaurantPropertiesKey].Properties;

                // Do we have the cuisine property?
                if (restaurantProps.ContainsKey(CuisinePropertyKey))
                {
                    // Read the cuisine property and return it as a string
                    return restaurantProps[CuisinePropertyKey].ToString();
                }
            }

            // Could not get the cusine from the result
            return UnknownString;
        }
        #endregion // Public Methods
    }
}
