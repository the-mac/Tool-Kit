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
using System.Device.Location;
#endif

#if WP7
using System.Windows.Media;
using Microsoft.Phone.Controls.Maps;
using Credentials = Microsoft.Phone.Controls.Maps.CredentialsProvider;
using Microsoft.Phone.Controls.Maps.Design;
using Microsoft.Phone.Controls.Maps.Platform;
#endif

#if WP8
using Credentials = GART.Controls.MapCredentials;
using System.Windows.Media;
using Microsoft.Phone.Maps;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Maps.Toolkit;
using Location = System.Device.Location.GeoCoordinate;
using System.Linq;
#endif

#if WIN_RT
using Bing.Maps;
using Credentials = System.String;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
#endif

using GART.Data;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using GART.Converters;





namespace GART.Controls
{
    [TemplatePart(Name = OverheadMap.PartNames.Map, Type = typeof(Map))]
    public class OverheadMap : ARRotateView, IARItemsView
    {
        #region Static Version
        #region Part Names
        static internal class PartNames
        {
            public const string Map = "Map";
        }
        #endregion // Part Names

        #region Dependency Properties
        /// <summary>
        /// Identifies the <see cref="ZoomLevel"/> dependency property.
        /// </summary>
        static public readonly DependencyProperty ZoomLevelProperty = DependencyProperty.Register("ZoomLevel", typeof(double), typeof(OverheadMap), new PropertyMetadata(.85d, OnZoomLevelChanged));

        private static void OnZoomLevelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((OverheadMap)d).OnZoomLevelChanged(e);
        }
        #endregion // Dependency Properties
        #endregion // Static Version

        #region Instance Version
        #region Member Variables
        private ObservableCollection<ARItem> arItems;
        private Map map;

        #if WP7
        private Credentials credentials;
        #endif

        #if WP8
        private MapCredentials credentials;
        #endif

        #if WIN_RT
        private Credentials credentials = "";
        #endif
        #endregion // Member Variables

        #region Constructors
        public OverheadMap()
        {
            DefaultStyleKey = typeof(OverheadMap);

            // Subscribe to LayoutUpdated so we can clip the map correctly.
            // This is necessary because we have to make the map larger than our bounds so rotation works.
            this.LayoutUpdated += OverheadMap_LayoutUpdated;
        }
        #endregion // Constructors

        #region Internal Methods
        private void UpdateMargin()
        {
            if (map != null)
            {
                double largest = Math.Max(this.ActualWidth, this.ActualHeight);
                map.Margin = new Thickness(-largest * 0.3333);
            }
        }
        #endregion // Internal Methods

        #region Overrides / Event Handlers

        #if WINDOWS_PHONE
        public override void OnApplyTemplate()
        #endif
        #if WIN_RT
        protected override void OnApplyTemplate()
        #endif
        {
            base.OnApplyTemplate();

            map = GetTemplateChild(PartNames.Map) as Map;

            // Validate the template
            if (map == null)
            {
                throw new InvalidOperationException(string.Format("{0} template is invalid. A {1} named {2} must be supplied.", GetType().Name, typeof(Map).Name, PartNames.Map));
            }

            // Connect credentials
            #if WP7
            map.CredentialsProvider = credentials;
            #endif

            #if WP8
            if (credentials != null)
            {
                MapsSettings.ApplicationContext.ApplicationId = credentials.ApplicationId;
                MapsSettings.ApplicationContext.AuthenticationToken = credentials.AuthenticationToken;
            }
            #endif
            
            #if WIN_RT
            map.Credentials = credentials;
            #endif

            // Update the margin
            UpdateMargin();

            // Connect data
            map.DataContext = arItems;
            #if WP8
            // We must use the toolkit to get the child map items controls and set their items source properly
            foreach (var itemsControl in MapExtensions.GetChildren(map).OfType<MapItemsControl>())
            {
                itemsControl.ItemsSource = arItems;
            }
            #endif
        
            #if WIN_RT
            // Set initial values for properties that can't be data bound in Windows 8
            map.Center = Location;
            map.ZoomLevel = PercentBingZoomConverter.PercentToMapLevel(ZoomLevel);
            #endif
        
        }

        //void map_ManipulationCompleted(object sender, System.Windows.Input.ManipulationCompletedEventArgs e)
        //{
        //    System.Diagnostics.Debug.WriteLine("Manip Completed");
        //}

        //void map_ManipulationDelta(object sender, System.Windows.Input.ManipulationDeltaEventArgs e)
        //{
        //    System.Diagnostics.Debug.WriteLine("Manip Completed");
        //}

        //void map_ManipulationStarted(object sender, System.Windows.Input.ManipulationStartedEventArgs e)
        //{
        //    System.Diagnostics.Debug.WriteLine("Manip Completed");
        //}
        #if WP7
        private void OverheadMap_LayoutUpdated(object sender, EventArgs e)
        #else
        private void OverheadMap_LayoutUpdated(object sender, object e)
        #endif
        {
            // This method gets called a lot. Check to see that we actually need to update 
            // before we new up objects.
            RectangleGeometry clipGeometry = this.Clip as RectangleGeometry;

            if ((clipGeometry == null) || (clipGeometry.Bounds.Width != this.ActualWidth) || (clipGeometry.Bounds.Height != this.ActualHeight))
            {
                // Clip all children so the map doesn't get drawn outside our bounds
                clipGeometry = new RectangleGeometry();
                clipGeometry.Rect = new Rect(0, 0, this.ActualWidth, this.ActualHeight);
                this.Clip = clipGeometry;
            }

            // Update the margin
            UpdateMargin();
        }

        #if WP8
        // The WP8 version of the map control does not allow heading to be data bound
        protected override void OnRotationChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnRotationChanged(e);
            if (map != null)
            {
                map.Heading = Rotation;
            }
        }
        #endif
        #endregion // Overrides / Event Handlers

        #region Overridables / Event Triggers
        #if WIN_RT
        protected override void OnLocationChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnLocationChanged(e);
            
            // The Win8 version of the map control does not allow Center to be data bound
            if (map != null)
            {
                map.Center = Location;
            }
        }
        #endif

        /// <summary>
        /// Occurs when the value of the <see cref="ZoomLevel"/> property has changed.
        /// </summary>
        /// <param name="e">
        /// A <see cref="DependencyPropertyChangedEventArgs"/> containing event information.
        /// </param>
        protected virtual void OnZoomLevelChanged(DependencyPropertyChangedEventArgs e)
        {
            #if WIN_RT
            // The Win8 version of the map control does not allow Zoom Level to be data bound
            if (map != null)
            {
                map.ZoomLevel = PercentBingZoomConverter.PercentToMapLevel(ZoomLevel);
            }
            #endif
        }
        #endregion // Overridables / Event Triggers

        #region Public Properties

        protected override void OnOrientationChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnOrientationChanged(e);
            if (map != null)
            {
                #if WP8
                // WP8 seems to have problem with map after orientation change: map is decentered
                // Reseting view seems to fix the problem
                map.SetView(map.Center, map.ZoomLevel);    
                #endif // WP8
            }

        }

        /// <summary>
        /// Gets or sets the collection of ARItem objects that should be rendered in the view.
        /// </summary>
        #if WP7
        [Category("AR")]
        #endif
        public ObservableCollection<ARItem> ARItems
        {
            get
            {
                return arItems;
            }
            set
            {
                arItems = value;
                if (map != null)
                {
                    map.DataContext = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the credentials provider used by the underlying map.
        /// </summary>
        #if WP7
        [Category("Map")]
        [TypeConverter(typeof(ApplicationIdCredentialsProviderConverter))]
        #endif
        #if WP8
        [Category("Map")]
        #endif
        public Credentials Credentials
        {
            get
            {
                return credentials;
            }
            set
            {
                credentials = value;
                if (map != null)
                {
                    #if WP7
                    map.CredentialsProvider = value;
                    #endif
                    #if WP8
                    if (value != null)
                    {
                        MapsSettings.ApplicationContext.ApplicationId = value.ApplicationId;
                        MapsSettings.ApplicationContext.AuthenticationToken = value.AuthenticationToken;
                    }
                    #endif
                    #if WIN_RT
                    map.Credentials = value;
                    #endif
                }
            }
        }


        /// <summary>
        /// Gets the <see cref="Map"/> instance used by the OverheadMap.
        /// </summary>
        #if WP7
        [Category("Map")]
        #endif
        public Map Map
        {
            get
            {
                return map;
            }
        }

        /// <summary>
        /// Gets or sets the ZoomLevel of the <see cref="OverheadMap"/>. This is a dependency property.
        /// </summary>
        /// <value>
        /// The ZoomLevel of the <see cref="OverheadMap"/>. 0 is zoomed out completely and 1 is zoomed in completely.
        /// </value>
        #if WP7
        [Category("Map")]
        #endif
        public double ZoomLevel
        {
            get
            {
                return (double)GetValue(ZoomLevelProperty);
            }
            set
            {
                SetValue(ZoomLevelProperty, value);
            }
        }
        #endregion // Public Properties
        #endregion // Instance Version


    }
}
