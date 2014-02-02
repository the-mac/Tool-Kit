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

using System.Windows;

#if WINDOWS_PHONE
using Microsoft.Phone.Controls;
using System.Device.Location;
using System.Windows.Controls;
using System.Windows.Media;
using VideoSource = System.Windows.Media.Brush;
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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using VideoSource = Windows.Media.Capture.MediaCapture;
#endif

using GART.BaseControls;
using GART.Data;
#if X3D
using GART.X3D;
using Matrix = GART.X3D.Matrix;
#else
using Microsoft.Xna.Framework;
using Matrix = Microsoft.Xna.Framework.Matrix;
#endif
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace GART.Controls
{
    /// <summary>
    /// A base control that serves as the starting point for an augmented reality view that renders ARItems.
    /// </summary>
    #if WINDOWS_PHONE
    public abstract class ARItemsView : ListBox, IARItemsView
    #endif
    #if WIN_RT
    public abstract class ARItemsView : ListView, IARItemsView
    #endif
    {
        #region Static Version
        #region Dependency Properties
        /// <summary>
        /// Identifies the <see cref="ARItems"/> dependency property.
        /// </summary>
        static public readonly DependencyProperty ARItemsProperty = DependencyProperty.Register("ARItems", typeof(ObservableCollection<ARItem>), typeof(ARItemsView), new PropertyMetadata(new ObservableCollection<ARItem>(), OnARItemsChanged));

        private static void OnARItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ARItemsView)d).OnARItemsChanged(e);
        }

        /// <summary>
        /// Identifies the <see cref="Attitude"/> dependency property.
        /// </summary>
        static public readonly DependencyProperty AttitudeProperty = DependencyProperty.Register("Attitude", typeof(Matrix), typeof(ARItemsView), new PropertyMetadata(ARDefaults.EmptyMatrix, OnAttitudeChanged));

        private static void OnAttitudeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ARItemsView)d).OnAttitudeChanged(e);
        }

        /// <summary>
        /// Identifies the <see cref="AttitudeHeading"/> dependency property.
        /// </summary>
        static public readonly DependencyProperty AttitudeHeadingProperty = DependencyProperty.Register("AttitudeHeading", typeof(double), typeof(ARItemsView), new PropertyMetadata(0d, OnAttitudeHeadingChanged));

        private static void OnAttitudeHeadingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ARItemsView)d).OnAttitudeHeadingChanged(e);
        }

        /// <summary>
        /// Identifies the <see cref="Location"/> dependency property.
        /// </summary>
        static public readonly DependencyProperty LocationProperty = DependencyProperty.Register("Location", typeof(Location), typeof(ARItemsView), new PropertyMetadata(ARDefaults.DefaultStartLocation, OnLocationChanged));

        private static void OnLocationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ARItemsView)d).OnLocationChanged(e);
        }

        /// <summary>
        /// Identifies the <see cref="Orientation"/> dependency property
        /// </summary>
        static public DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(ControlOrientation), typeof(ARItemsView), new PropertyMetadata(ControlOrientation.Default, OnOrientationChanged));

        private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ARItemsView)d).OnOrientationChanged(e);
        }

        /// <summary>
        /// Identifies the <see cref="TravelHeading"/> dependency property.
        /// </summary>
        static public readonly DependencyProperty TravelHeadingProperty = DependencyProperty.Register("TravelHeading", typeof(double), typeof(ARItemsView), new PropertyMetadata(0d, OnTravelHeadingChanged));

        private static void OnTravelHeadingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ARItemsView)d).OnTravelHeadingChanged(e);
        }

        /// <summary>
        /// Identifies the <see cref="VideoSource"/> dependency property.
        /// </summary>
        static public readonly DependencyProperty VideoSourceProperty = DependencyProperty.Register("VideoSource", typeof(VideoSource), typeof(ARItemsView), new PropertyMetadata(ARDefaults.DefaultVideoSource, OnVideoSourceChanged));

        private static void OnVideoSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ARItemsView)d).OnVideoSourceChanged(e);
        }

        #endregion // Dependency Properties
        #endregion // Static Version

        #region Instance Version
        #region Overridables / Event Triggers

        protected virtual void OnOrientationChanged(DependencyPropertyChangedEventArgs e)
        {
        }
        
        /// <summary>
        /// Occurs when the value of the <see cref="ARItems"/> property has changed.
        /// </summary>
        /// <param name="e">
        /// A <see cref="DependencyPropertyChangedEventArgs"/> containing event information.
        /// </param>
        protected virtual void OnARItemsChanged(DependencyPropertyChangedEventArgs e)
        {
            // Change our ItemsSource to be the new items collection
            this.ItemsSource = this.ARItems;
        }

        /// <summary>
        /// Occurs when the value of the <see cref="Attitude"/> property has changed.
        /// </summary>
        /// <param name="e">
        /// A <see cref="DependencyPropertyChangedEventArgs"/> containing event information.
        /// </param>
        protected virtual void OnAttitudeChanged(DependencyPropertyChangedEventArgs e)
        {

        }

        /// <summary>
        /// Occurs when the value of the <see cref="AttitudeHeading"/> property has changed.
        /// </summary>
        /// <param name="e">
        /// A <see cref="DependencyPropertyChangedEventArgs"/> containing event information.
        /// </param>
        protected virtual void OnAttitudeHeadingChanged(DependencyPropertyChangedEventArgs e)
        {

        }

        /// <summary>
        /// Occurs when the value of the <see cref="Location"/> property has changed.
        /// </summary>
        /// <param name="e">
        /// A <see cref="DependencyPropertyChangedEventArgs"/> containing event information.
        /// </param>
        protected virtual void OnLocationChanged(DependencyPropertyChangedEventArgs e)
        {

        }

        /// <summary>
        /// Occurs when the value of the <see cref="TravelHeading"/> property has changed.
        /// </summary>
        /// <param name="e">
        /// A <see cref="DependencyPropertyChangedEventArgs"/> containing event information.
        /// </param>
        protected virtual void OnTravelHeadingChanged(DependencyPropertyChangedEventArgs e)
        {

        }

        /// <summary>
        /// Occurs when the value of the <see cref="VideoBrush"/> property has changed.
        /// </summary>
        /// <param name="e">
        /// A <see cref="DependencyPropertyChangedEventArgs"/> containing event information.
        /// </param>
        protected virtual void OnVideoSourceChanged(DependencyPropertyChangedEventArgs e)
        {

        }
        #endregion // Overridables / Event Triggers

        #region Public Properties
        /// <summary>
        /// Gets or sets the collection of ARItems rendered by the view. This is a dependency property.
        /// </summary>
        /// <value>
        /// The collection of ARItems rendered by the view.
        /// </value>
        #if WP7
        [Category("AR")]
        #endif
        public ObservableCollection<ARItem> ARItems
        {
            get
            {
                return (ObservableCollection<ARItem>)GetValue(ARItemsProperty);
            }
            set
            {
                SetValue(ARItemsProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a matrix that represents where the user is looking. This is a dependency property.
        /// </summary>
        /// <value>
        /// A matrix that represents where the user is looking.
        /// </value>
        #if WP7
        [Category("AR")]
        #endif
        public Matrix Attitude
        {
            get
            {
                return (Matrix)GetValue(AttitudeProperty);
            }
            set
            {
                SetValue(AttitudeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the direction the user is looking in degrees. This is a dependency property.
        /// </summary>
        /// <value>
        /// The direction the user is looking in degrees.
        /// </value>
        #if WP7
        [Category("AR")]
        #endif
        public double AttitudeHeading
        {
            get
            {
                return (double)GetValue(AttitudeHeadingProperty);
            }
            set
            {
                SetValue(AttitudeHeadingProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the location of the user in Geo space. This is a dependency property.
        /// </summary>
        /// <value>
        /// The location of the user in Geo space.
        /// </value>
        #if WP7
        [Category("AR")]
        #endif
        public Location Location
        {
            get
            {
                return (Location)GetValue(LocationProperty);
            }
            set
            {
                SetValue(LocationProperty, value);
            }
        }


        /// <summary>
        /// Gets or sets current device orientation
        /// </summary>
        /// <value>
        /// Current <see cref="PageOrientation"/> of a device
        /// </value>
        #if WP7
        [Category("AR")]
        #endif
        public ControlOrientation Orientation
        {
            get { return (ControlOrientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        /// <summary>
        /// Gets or sets the direction the user is traveling in degrees. This is a dependency property.
        /// </summary>
        /// <value>
        /// The direction the user is traveling in degrees.
        /// </value>
        #if WP7
        [Category("AR")]
        #endif
        public double TravelHeading
        {
            get
            {
                return (double)GetValue(TravelHeadingProperty);
            }
            set
            {
                SetValue(TravelHeadingProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the video source for the camera. This is a dependency property.
        /// </summary>
        /// <value>
        /// The video source for the camera.
        /// </value>
        #if WP7
        [Category("AR")]
        #endif
        public VideoSource VideoSource
        {
            get
            {
                return (VideoSource)GetValue(VideoSourceProperty);
            }
            set
            {
                SetValue(VideoSourceProperty, value);
            }
        }
        #endregion // Public Properties

        #endregion // Instance Version
    }
}
