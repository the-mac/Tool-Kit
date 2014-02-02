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
#if WINDOWS_PHONE
using System.Device.Location;
#endif

#if WP7
using Microsoft.Phone.Controls.Maps.Platform;
#endif

#if WP8
using Microsoft.Phone.Maps.Controls;
using Location = System.Device.Location.GeoCoordinate;
#endif

#if WIN_RT
using Bing.Maps;
using Windows.Devices.Geolocation;
#endif

#if X3D
using GART.X3D;
using Vector3 = GART.X3D.Vector3;
using Matrix = GART.X3D.Matrix;
#else
using Microsoft.Xna.Framework;
using Vector3 = Microsoft.Xna.Framework.Vector3;
using Matrix = Microsoft.Xna.Framework.Matrix;
#endif

namespace GART.Data
{
    /// <summary>
    /// An item that is rendered in one or more ARViews.
    /// </summary>
    public class ARItem : ObservableObject
    {
        #region Instance Version
        #region Member Variables
        private object content;
        private Location geoLocation = new Location();
        private Vector3 relativeLocation = Vector3.Zero;
        private Action<ItemCalculationSettings, ARItem> worldCalculation = ARHelper.WorldFromGeoLocation; // Default to calculate based on geo location since this will be the most common.
        private Vector3 worldLocation = Vector3.Zero;
        #endregion // Member Variables

        #region Public Properties
        /// <summary>
        /// Gets or sets the content used to represnt the item.
        /// </summary>
        /// <value>
        /// The content used to represnt the item.
        /// </value>
        public object Content
        {
            get
            {
                return content;
            }
            set
            {
                if (content != value)
                {
                    content = value;
                    NotifyPropertyChanged(() => Content);
                }
            }
        }

        /// <summary>
        /// Gets or sets the location of the item in geo space.
        /// </summary>
        /// <value>
        /// The location of the item in virtual geo space.
        /// </value>
        public Location GeoLocation
        {
            get
            {
                return geoLocation;
            }
            set
            {
                if (geoLocation != value)
                {
                    geoLocation = value;
                    NotifyPropertyChanged(() => GeoLocation);
                }
            }
        }

        /// <summary>
        /// Gets or sets the location of the item in virtual relative space.
        /// </summary>
        /// <value>
        /// The location of the item in virtual relative space.
        /// </value>
        public Vector3 RelativeLocation
        {
            get
            {
                return relativeLocation;
            }
            set
            {
                if (relativeLocation != value)
                {
                    relativeLocation = value;
                    NotifyPropertyChanged(() => RelativeLocation);
                }
            }
        }

        /// <summary>
        /// Gets or sets the mode used to calculate the items position in virtual world space.
        /// </summary>
        /// <value>
        /// The mode used to calculate the items position in virtual world space.
        /// </value>
        public Action<ItemCalculationSettings, ARItem> WorldCalculation
        {
            get
            {
                return worldCalculation;
            }
            set
            {
                if (worldCalculation != value)
                {
                    worldCalculation = value;
                    NotifyPropertyChanged(() => WorldCalculation);
                }
            }
        }

        /// <summary>
        /// Gets or sets the location of the item in virtual world space.
        /// </summary>
        /// <value>
        /// The location of the item in virtual world space.
        /// </value>
        public Vector3 WorldLocation
        {
            get
            {
                return worldLocation;
            }
            set
            {
                if (worldLocation != value)
                {
                    worldLocation = value;
                    NotifyPropertyChanged(() => WorldLocation);
                }
            }
        }
        #endregion // Public Properties
        #endregion // Instance Version
    }
}
