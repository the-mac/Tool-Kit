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
using Microsoft.Devices;
using Microsoft.Devices.Sensors;
using Microsoft.Phone.Controls;
using System.Device.Location;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
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
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Geolocation;
using Windows.Devices.Sensors;
using Motion = Windows.Devices.Sensors.Inclinometer;
using Windows.Media.Capture;
using VideoSource = Windows.Media.Capture.MediaCapture;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

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

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace GART.Controls
{
    #if WINDOWS_PHONE
    [ContentProperty("Views")]
    #endif
    #if WIN_RT
    [ContentProperty(Name="Views")]
    #endif
    public class ARDisplay : Grid, IARItemsView
    {
        #region Static Version

        #region Dependency Properties
        /// <summary>
        /// Identifies the <see cref="ARItems"/> dependency property.
        /// </summary>
        static public readonly DependencyProperty ARItemsProperty = DependencyProperty.Register("ARItems", typeof(ObservableCollection<ARItem>), typeof(ARDisplay), new PropertyMetadata(new ObservableCollection<ARItem>(), OnARItemsChanged));

        private static void OnARItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ARDisplay)d).OnARItemsChanged(e);
        }

        /// <summary>
        /// Identifies the <see cref="Attitude"/> dependency property.
        /// </summary>
        static public readonly DependencyProperty AttitudeProperty = DependencyProperty.Register("Attitude", typeof(Matrix), typeof(ARDisplay), new PropertyMetadata(ARDefaults.EmptyMatrix, OnAttitudeChanged));

        private static void OnAttitudeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ARDisplay)d).OnAttitudeChanged(e);
        }

        /// <summary>
        /// Identifies the <see cref="CameraEnabled"/> dependency property.
        /// </summary>
        static public readonly DependencyProperty CameraEnabledProperty = DependencyProperty.Register("CameraEnabled", typeof(bool), typeof(ARDisplay), new PropertyMetadata(true, OnCameraEnabledChanged));

        private static void OnCameraEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ARDisplay)d).OnCameraEnabledChanged(e);
        }

        /// <summary>
        /// Identifies the <see cref="AttitudeHeading"/> dependency property.
        /// </summary>
        static public readonly DependencyProperty AttitudeHeadingProperty = DependencyProperty.Register("AttitudeHeading", typeof(double), typeof(ARDisplay), new PropertyMetadata(0d, OnAttitudeHeadingChanged));

        private static void OnAttitudeHeadingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ARDisplay)d).OnAttitudeHeadingChanged(e);
        }

        /// <summary>
        /// Identifies the <see cref="Location"/> dependency property.
        /// </summary>
        static public readonly DependencyProperty LocationProperty = DependencyProperty.Register("Location", typeof(Location), typeof(ARDisplay), new PropertyMetadata(ARDefaults.DefaultStartLocation, OnLocationChanged));

        private static void OnLocationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ARDisplay)d).OnLocationChanged(e);
        }

        /// <summary>
        /// Identifies the <see cref="LocationEnabled"/> dependency property.
        /// </summary>
        static public readonly DependencyProperty LocationEnabledProperty = DependencyProperty.Register("LocationEnabled", typeof(bool), typeof(ARDisplay), new PropertyMetadata(true, OnLocationEnabledChanged));

        private static void OnLocationEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ARDisplay)d).OnLocationEnabledChanged(e);
        }

        /// <summary>
        /// Identifies the <see cref="MotionEnabled"/> dependency property.
        /// </summary>
        static public readonly DependencyProperty MotionEnabledProperty = DependencyProperty.Register("MotionEnabled", typeof(bool), typeof(ARDisplay), new PropertyMetadata(true, OnMotionEnabledChanged));

        private static void OnMotionEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ARDisplay)d).OnMotionEnabledChanged(e);
        }

        static public readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(ControlOrientation), typeof(ARDisplay), new PropertyMetadata(ControlOrientation.Default, OnOrientationChanged));

        private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ARDisplay)d).OnOrientationChanged(e);
        }

        /// <summary>
        /// Identifies the <see cref="TravelHeading"/> dependency property.
        /// </summary>
        static public readonly DependencyProperty TravelHeadingProperty = DependencyProperty.Register("TravelHeading", typeof(double), typeof(ARDisplay), new PropertyMetadata(0d, OnTravelHeadingChanged));

        private static void OnTravelHeadingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ARDisplay)d).OnTravelHeadingChanged(e);
        }

        /// <summary>
        /// Identifies the <see cref="VideoSource"/> dependency property.
        /// </summary>
        static public readonly DependencyProperty VideoSourceProperty = DependencyProperty.Register("VideoSource", typeof(VideoSource), typeof(ARDisplay), new PropertyMetadata(ARDefaults.DefaultVideoSource, OnVideoSourceChanged));

        private static void OnVideoSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ARDisplay)d).OnVideoSourceChanged(e);
        }

        #endregion // Dependency Properties
        #endregion // Static Version

        #region Instance Version
        #region Member Variables

        #if WINDOWS_PHONE
        private GeoCoordinateWatcher locationService;
        private Motion motion;
        private Compass compass;
        private PhotoCamera photoCamera;
        #endif

        #if WIN_RT
        private Geolocator locationService;
        // Ricky: Use Orientation & SimpleOrientation Sensor
        private OrientationSensor motion;
        private SimpleOrientationSensor simpleMotion;
        #endif

        private Collection<ServiceErrorData> serviceErrors = new Collection<ServiceErrorData>();
        private bool servicesRunning;
        private ItemCalculationSettings settings;
        private ObservableCollection<IARView> views;
        #endregion // Member Variables

        #region Constructors
        public ARDisplay()
        {
            // Create the views collection
            views = new ObservableCollection<IARView>();

            // Create the calculation settings instance
            settings = new ItemCalculationSettings { View = this };

            // Subscribe to events
            ARItems.CollectionChanged += ItemCollectionChanged;
            views.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(views_CollectionChanged);
        }
        #endregion // Constructors

        #region Internal Methods
        private void CalculateItemLocation(ARItem item)
        {
            // If a calculation exists, call it
            if (item.WorldCalculation != null)
            {
                item.WorldCalculation(settings, item);
            }
        }

        private void CalculateItemLocations()
        {
            // Loop through all the items
            for (int i = 0; i < ARItems.Count; i++)
            {
                // Get the item
                ARItem item = ARItems[i];

                // Calculate the single item
                CalculateItemLocation(item);
            }
        }

        #if WINDOWS_PHONE
        private void CreateVideoBrush()
        {
            // Create our brush
            VideoBrush vb = new VideoBrush();

            // The video from the camera comes across at 640 x 480 but in portraid mode we need to 
            // rotate the video 90 degrees.
            vb.RelativeTransform = new CompositeTransform { CenterX = 0.5, CenterY = 0.5, Rotation = photoCamera.Orientation };

            // Ideally we would make sure video is never stretched, but 
            // this method doesn't seem to work correctly when the video 
            // brush is used in multiple places.
            // vb.Stretch = Stretch.Uniform;

            // Update our dependency property
            VideoSource = vb;
        }
        #endif

        private void AddViews(IList views)
        {
            // Can only add and remove if the view container has been loaded from the template
            foreach (object view in views)
            {
                // Make sure it's not us
                if (view == this) { throw new InvalidOperationException("Cannot add the ARDisplay as a view, this would cause a circular reference."); }

                // Look for known interfaces
                IARView arView = view as IARView;
                IARItemsView itemView = view as IARItemsView;
                UIElement uie = view as UIElement;

                // If it's an IARView (which it always should be) setup current values
                if (arView != null)
                {
                    // arView.Attitude = this.Attitude; // HACK: Uncommenting this line breaks the Visual Studio designer. No reason is known.
                    arView.AttitudeHeading = this.AttitudeHeading;
                    arView.Location = this.Location;
                    arView.VideoSource = this.VideoSource;
                }

                // If it's an IARItemsView give it the list of current items
                if (itemView != null)
                {
                    itemView.ARItems = this.ARItems;
                }

                // If it's a UIElement, add it to the screen.
                if (uie != null)
                {
                    Children.Add(uie);
                }
            }
        }

        #if WIN_RT
        private async Task FindBestCamera()
        {
            // Get all video capture devices
            var devices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
            
            // First look using panel, this is the best approach
            var device = (from d in devices
                            where d.EnclosureLocation != null &&
                            d.EnclosureLocation.Panel == Windows.Devices.Enumeration.Panel.Back
                            select d).FirstOrDefault();

            // If camera wasn't found using panel location, look for a device that has "back" or "rear" in the name or ID
            if (device == null)
            {
                device = (from d in devices
                              where d.Name.ToLower().Contains("back") || 
                                    d.Id.ToLower().Contains("back") ||
                                    d.Name.ToLower().Contains("rear") || 
                                    d.Id.ToLower().Contains("rear")
                              select d).FirstOrDefault();
            }
            
            // If a device was found, store it's ID
            if (device != null)
            {
                CameraId = device.Id;
            }
        }
        #endif

        private void RemoveViews(IList views)
        {
            foreach (object view in views)
            {
                // Look for known interfaces
                IARView arView = view as IARView;
                IARItemsView itemView = view as IARItemsView;
                UIElement uie = view as UIElement;

                // If it's an IARView (which it always should be) disconnect current values
                if (arView != null)
                {
                    // Set current values
                    arView.VideoSource = null;
                }

                // If it's an IARItemsView disconnect the list of current items
                if (itemView != null)
                {
                    itemView.ARItems = null;
                }

                // If it's a UIElement, remove it from the screen.
                if (uie != null)
                {
                    Children.Remove(uie);
                }
            }
        }

        #if WINDOWS_PHONE
        private void StartCamera()
        {
            // If the camera hasn't been created yet, create it.
            if (photoCamera == null)
            {
                photoCamera = new PhotoCamera();
            }

            // If our video brush hasn't been created yet, create it
            VideoBrush vb = VideoSource as VideoBrush;
            if (vb == null)
            {
                CreateVideoBrush();
                vb = VideoSource as VideoBrush;
            }

            // Connect the video brush to the camera
            vb.SetSource(photoCamera);
        }
        #endif

        #if WIN_RT
        private async Task StartCamera()
        {
            if (VideoSource == null)
            {
                var source = new Windows.Media.Capture.MediaCapture();
                try
                {
                    // Do we need to look for the best camera?
                    if (string.IsNullOrEmpty(CameraId))
                    {
                        await FindBestCamera();
                    }

                    // Use a specific camera?
                    if (!string.IsNullOrEmpty(CameraId))
                    {
                        await source.InitializeAsync(
                            new MediaCaptureInitializationSettings
                            {
                                VideoDeviceId = CameraId,
                                StreamingCaptureMode = StreamingCaptureMode.Video
                            });
                    }
                    else
                    {
                        // Just use the default camera
                        await source.InitializeAsync();
                    }
                    VideoSource = source;

                    // Start video preview
                    await source.StartPreviewAsync();
                }
                catch (Exception ex)
                {
                    serviceErrors.Add(new ServiceErrorData(ARService.Camera, ex));
                }
            }
        }
        #endif

        #if WINDOWS_PHONE
        private void StartLocation()
        #else
        private async Task StartLocation()
        #endif
        {
            // If the Location object is null, initialize it and add a CurrentValueChanged
            // event handler.
            if (locationService == null)
            {
                #if WINDOWS_PHONE

                locationService = new GeoCoordinateWatcher(GeoPositionAccuracy.High);
                locationService.MovementThreshold = MovementThreshold;
                locationService.PositionChanged += location_PositionChanged;

                // Try to start the Motion API.
                try
                {
                    // Start the service
                    locationService.Start();

                    // Force a grab of location once
                    Location = locationService.Position.Location;
                }
                catch (Exception ex)
                {
                    serviceErrors.Add(new ServiceErrorData(ARService.Location, ex));
                }
                #endif

                #if WIN_RT
                locationService = new Geolocator();
                locationService.MovementThreshold = MovementThreshold;
                locationService.PositionChanged += location_PositionChanged;

                // Grab location once?
                try
                {
                    var loc = (await locationService.GetGeopositionAsync()).Coordinate;
                    this.Location = new Location(loc.Latitude, loc.Longitude);
                    this.TravelHeading = loc.Heading ?? 0; // Force to 0 degrees if unknown.
                }
                catch (Exception ex)
                {
                    serviceErrors.Add(new ServiceErrorData(ARService.Location, ex));
                }
#endif
            }
        }


        private void StartMotion()
        {
            #if WINDOWS_PHONE
            if (Motion.IsSupported)
            {
                // If the Motion object is null, initialize it and add a CurrentValueChanged
                // event handler.
                if (motion == null)
                {
                    motion = new Motion();
                    motion.TimeBetweenUpdates = TimeSpan.FromMilliseconds(20);
                    motion.CurrentValueChanged += motion_CurrentValueChanged;
                }

                // Try to start the Motion API.
                try
                {
                    motion.Start();
                }
                catch (Exception ex)
                {
                    serviceErrors.Add(new ServiceErrorData(ARService.Motion, ex));
                }

                // If the Motion object is null, initialize it and add a CurrentValueChanged
                // event handler.
                if (compass == null)
                {
                    compass = new Compass();
                    compass.TimeBetweenUpdates = TimeSpan.FromMilliseconds(AttitudeRefreshRate);
                    compass.CurrentValueChanged += compass_CurrentValueChanged;
                }

                // Try to start the Motion API.
                try
                {
                    compass.Start();
                }
                catch (Exception ex)
                {
                    serviceErrors.Add(new ServiceErrorData(ARService.Motion, ex));
                }


            }
            else
            {
                serviceErrors.Add(new ServiceErrorData(ARService.Motion, new InvalidOperationException("The Motion API is not supported on this device.")));
            }
            #endif

            #if WIN_RT
            // Ricky: Switch to orienation sensor
            motion = OrientationSensor.GetDefault();
            if (motion != null)
            {
                // Ricky: motion.ReportInterval = 20;
                motion.ReportInterval = (motion.MinimumReportInterval < 20) ? 20 : motion.MinimumReportInterval;
                motion.ReadingChanged += motion_CurrentValueChanged;
            }
            else
            {
                // Ricky: Update message to state Orientation Sensor
                serviceErrors.Add(new ServiceErrorData(ARService.Motion, new InvalidOperationException("Orientation Sensor is not supported on this device.")));
            }

            // Ricky: Add Simple Orientation Sensor
            simpleMotion = SimpleOrientationSensor.GetDefault();
            if (simpleMotion != null)
            {
                simpleMotion.OrientationChanged += simpleMotion_OrientationChanged;
            }
            #endif
        }

    	#if WINDOWS_PHONE
        private void StopCamera()
        {
		    if (photoCamera != null)
            {
                photoCamera.Dispose();
                photoCamera = null;
            }
        }
		#endif

        #if WIN_RT
        private async void StopCamera()
        {

            // Ricky: Stop the cemera if the video source is not null
            if (VideoSource != null)
            {
                await VideoSource.StopPreviewAsync();
                VideoSource = null;
            }
            
        }
		#endif

        private void StopLocation()
        {
            locationService.PositionChanged -= location_PositionChanged;

            #if WINDOWS_PHONE
            locationService.Stop();
            locationService.Dispose();
            #endif
            
            locationService = null;
        }

        private void StopMotion()
        {
            #if WINDOWS_PHONE
            motion.CurrentValueChanged -= motion_CurrentValueChanged;
            motion.Stop();
            motion.Dispose();
            #else // WIN_RT
            // Ricky: Changed event name for switch to Orientation Sensor
            if (motion != null)
            {
                motion.ReadingChanged -= motion_CurrentValueChanged;
            }

            if (simpleMotion != null)
            {
                simpleMotion.OrientationChanged -= simpleMotion_OrientationChanged;
            }
            #endif

            motion = null;
        }
        #endregion // Internal Methods

        #region Overrides / Event Handlers
        #if WINDOWS_PHONE
        private void location_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            // This event arrives on a background thread. Use BeginInvoke to call
            // CurrentValueChanged on the UI thread.
            Dispatcher.BeginInvoke(() =>
            {
                // Update ourslves which will in turn update all the views
                var loc = e.Position.Location;
                this.Location = loc; // new Location(loc.Latitude, loc.Longitude, loc.Altitude, loc.HorizontalAccuracy, loc.VerticalAccuracy);
                this.TravelHeading = loc.Course;
            });
        }
        #endif

        #if WIN_RT
        private void location_PositionChanged(object sender, PositionChangedEventArgs e)
        {
            // This event arrives on a background thread. Use BeginInvoke to call
            // CurrentValueChanged on the UI thread.
            var t = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                // Update ourslves which will in turn update all the views
                var loc = e.Position.Coordinate;
                this.Location = new Location(loc.Latitude, loc.Longitude);
                this.TravelHeading = loc.Heading ?? 0; // Force to 0 degrees if unknown.
            });
        }
        #endif

        #if WINDOWS_PHONE

        void compass_CurrentValueChanged(object sender, SensorReadingEventArgs<CompassReading> e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.AttitudeHeading = e.SensorReading.TrueHeading;
            });
        }
        
        private void motion_CurrentValueChanged(object sender, SensorReadingEventArgs<MotionReading> e)
        {
            // This event arrives on a background thread. Use BeginInvoke to call
            // CurrentValueChanged on the UI thread.
            Dispatcher.BeginInvoke(() =>
            {
                MotionReading mr = e.SensorReading;

                // Update ourslves which will in turn update all the views
                
                // Converting XNA to nonXna Matrix
                this.Attitude = new Matrix(
                    mr.Attitude.RotationMatrix.M11,
                    mr.Attitude.RotationMatrix.M12,
                    mr.Attitude.RotationMatrix.M13,
                    mr.Attitude.RotationMatrix.M14,

                    mr.Attitude.RotationMatrix.M21,
                    mr.Attitude.RotationMatrix.M22,
                    mr.Attitude.RotationMatrix.M23,
                    mr.Attitude.RotationMatrix.M24,

                    mr.Attitude.RotationMatrix.M31,
                    mr.Attitude.RotationMatrix.M32,
                    mr.Attitude.RotationMatrix.M33,
                    mr.Attitude.RotationMatrix.M34,

                    mr.Attitude.RotationMatrix.M41,
                    mr.Attitude.RotationMatrix.M42,
                    mr.Attitude.RotationMatrix.M43,
                    mr.Attitude.RotationMatrix.M44);

            });
        }
        #endif

        #if WIN_RT
        //private void motion_ReadingChanged(Inclinometer sender, InclinometerReadingChangedEventArgs args)
        private void motion_CurrentValueChanged(OrientationSensor sender, OrientationSensorReadingChangedEventArgs args)
        {
            // This event arrives on a background thread. Use Dispatcher to call
            // CurrentValueChanged on the UI thread.
            var t = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                var r = args.Reading;

                // Ricky:  Use the rotation matrix from the orienation sensor
                this.Attitude = new Matrix(r.RotationMatrix);

                //http://planning.cs.uiuc.edu/node103.html
                double yawRadians = Math.Atan2(Attitude.M21, Attitude.M11);
                this.AttitudeHeading = (360 - MathHelper.ToDegrees((float)yawRadians))%360;
            });
        }

        // Ricky: Added event handler for simple Orientation sensor.
        private void simpleMotion_OrientationChanged(SimpleOrientationSensor sender, SimpleOrientationSensorOrientationChangedEventArgs args)
        {
            var t = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                switch (args.Orientation)
                {
                    case SimpleOrientation.Rotated90DegreesCounterclockwise:
                        Orientation = ControlOrientation.Clockwise270Degrees;
                        break;
                    case SimpleOrientation.Rotated180DegreesCounterclockwise:
                        Orientation = ControlOrientation.Clockwise180Degrees;
                        break;
                    case SimpleOrientation.Rotated270DegreesCounterclockwise:
                        Orientation = ControlOrientation.Clockwise90Degrees;
                        break;
                    case SimpleOrientation.NotRotated:
                    default:
                        Orientation = ControlOrientation.Default;
                        break;
                }
            });
        }
        #endif

        private void views_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Add or remove from view container?
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                AddViews(e.NewItems);
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                RemoveViews(e.NewItems);
            }
            // TODO: Link or unlink the items collection.
        }
        #endregion // Overrides / Event Handlers

        #region Overridables / Event Triggers
        /// <summary>
        /// Occurs when the value of the <see cref="ARItems"/> property has changed.
        /// </summary>
        /// <param name="e">
        /// A <see cref="DependencyPropertyChangedEventArgs"/> containing event information.
        /// </param>
        protected virtual void OnARItemsChanged(DependencyPropertyChangedEventArgs e)
        {
            // Unsubscribe events from existing collection (if one exists)
            if (e.OldValue != null)
            {
                ((ObservableCollection<ARItem>)e.OldValue).CollectionChanged -= ItemCollectionChanged;
            }

            // Subscribe to new collection events
            if (e.NewValue != null)
            {
                ((ObservableCollection<ARItem>)e.NewValue).CollectionChanged += ItemCollectionChanged;
            }

            // Calculate the items locations before updating views
            CalculateItemLocations();

            // Update views
            foreach (IARView view in views)
            {
                IARItemsView itemsView = view as IARItemsView;
                if (itemsView != null)
                {
                    itemsView.ARItems = this.ARItems;
                }
            }
        }

        private void ItemCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // If a new item was added, replaced or the collection was reset, calculate locations
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Replace:
                    // Recalculate added or changed items
                    for (int i = 0; i < e.NewItems.Count; i++)
                    {
                        CalculateItemLocation((ARItem)e.NewItems[i]);
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    // Recalculate all
                    CalculateItemLocations();
                    break;
            }
        }

        /// <summary>
        /// Occurs when the value of the <see cref="Attitude"/> property has changed.
        /// </summary>
        /// <param name="e">
        /// A <see cref="DependencyPropertyChangedEventArgs"/> containing event information.
        /// </param>
        protected virtual void OnAttitudeChanged(DependencyPropertyChangedEventArgs e)
        {
            // Update views
            foreach (IARView view in views)
            {
                view.Attitude = this.Attitude;
            }
        }

        /// <summary>
        /// Occurs when the value of the <see cref="CameraEnabled"/> property has changed.
        /// </summary>
        /// <param name="e">
        /// A <see cref="DependencyPropertyChangedEventArgs"/> containing event information.
        /// </param>
        protected virtual void OnCameraEnabledChanged(DependencyPropertyChangedEventArgs e)
        {
            // If services ar running, attemt to start or stop the camera
            if (servicesRunning)
            {
                if (CameraEnabled)
                {
                    #if WINDOWS_PHONE
                    StartCamera();
                    #endif

                    #if WIN_RT
                    var t = StartCamera();
                    #endif
                }
                else
                {
                    StopCamera();
                }
            }
        }

        /// <summary>
        /// Occurs when the value of the <see cref="AttitudeHeading"/> property has changed.
        /// </summary>
        /// <param name="e">
        /// A <see cref="DependencyPropertyChangedEventArgs"/> containing event information.
        /// </param>
        protected virtual void OnAttitudeHeadingChanged(DependencyPropertyChangedEventArgs e)
        {
            // Update views
            foreach (IARView view in views)
            {
                view.AttitudeHeading = this.AttitudeHeading;
            }
        }

        /// <summary>
        /// Occurs when the value of the <see cref="Location"/> property has changed.
        /// </summary>
        /// <param name="e">
        /// A <see cref="DependencyPropertyChangedEventArgs"/> containing event information.
        /// </param>
        protected virtual void OnLocationChanged(DependencyPropertyChangedEventArgs e)
        {
            // Update views
            foreach (IARView view in views)
            {
                view.Location = this.Location;
            }

            // Run any necessary calculations on the items 
            CalculateItemLocations();

            // Notify subscribers
            if (LocationChanged != null)
            {
                LocationChanged(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Occurs when the value of the <see cref="LocationEnabled"/> property has changed.
        /// </summary>
        /// <param name="e">
        /// A <see cref="DependencyPropertyChangedEventArgs"/> containing event information.
        /// </param>
        protected virtual void OnLocationEnabledChanged(DependencyPropertyChangedEventArgs e)
        {
            // If services ar running, attemt to start or stop Location tracking
            if (servicesRunning)
            {
                if (LocationEnabled)
                {
                    #if WINDOWS_PHONE
                    StartLocation();
                    #endif

                    #if WIN_RT
                    var t = StartLocation();
                    #endif
                }
                else
                {
                    StopLocation();
                }
            }
        }

        /// <summary>
        /// Occurs when the value of the <see cref="MotionEnabled"/> property has changed.
        /// </summary>
        /// <param name="e">
        /// A <see cref="DependencyPropertyChangedEventArgs"/> containing event information.
        /// </param>
        protected virtual void OnMotionEnabledChanged(DependencyPropertyChangedEventArgs e)
        {
            // If services ar running, attemt to start or stop motion tracking
            if (servicesRunning)
            {
                if (MotionEnabled)
                {
                    StartMotion();
                }
                else
                {
                    StopMotion();
                }
            }
        }

        /// <summary>
        /// Occurs when an error was encountered starting or stopping a service.
        /// </summary>
        /// <param name="e">
        /// A <see cref="DependencyPropertyChangedEventArgs"/> containing event information.
        /// </param>
        protected virtual void OnServiceErrors(ServiceErrorsEventArgs e)
        {
            if (ServiceErrors != null)
            {
                ServiceErrors(this, e);
            }
        }

        /// <summary>
        /// Occurs when the value of the <see cref="TravelHeading"/> property has changed.
        /// </summary>
        /// <param name="e">
        /// A <see cref="DependencyPropertyChangedEventArgs"/> containing event information.
        /// </param>
        protected virtual void OnTravelHeadingChanged(DependencyPropertyChangedEventArgs e)
        {
            // Update views
            foreach (IARView view in views)
            {
                view.TravelHeading = this.TravelHeading;
            }
        }

        /// <summary>
        /// Occurs when the value of the <see cref="VideoSource"/> property has changed.
        /// </summary>
        /// <param name="e">
        /// A <see cref="DependencyPropertyChangedEventArgs"/> containing event information.
        /// </param>
        protected virtual void OnVideoSourceChanged(DependencyPropertyChangedEventArgs e)
        {
            // Update views
            foreach (IARView view in views)
            {
                view.VideoSource = this.VideoSource;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnOrientationChanged(DependencyPropertyChangedEventArgs e)
        {
            foreach (IARView view in views)
            {
                view.Orientation = this.Orientation;
            }

            // Need to handle videobrush rotation as well:
            ControlOrientation newOrientation = (ControlOrientation)(e.NewValue);

            #if WINDOWS_PHONE
            CompositeTransform orientationRotation; 
            switch (newOrientation)
            {
                case ControlOrientation.Clockwise270Degrees:
                    orientationRotation = new CompositeTransform() { CenterX = 0.5, CenterY = 0.5, Rotation = 0 };
                    break;

                case ControlOrientation.Clockwise90Degrees:
                    orientationRotation = new CompositeTransform() { CenterX = 0.5, CenterY = 0.5, Rotation = 180 };
                    break;

                default:
                    orientationRotation = new CompositeTransform() { CenterX = 0.5, CenterY = 0.5, Rotation = 90 };
                    break;
            } // end switch 

            // Update our dependency property
            VideoSource.RelativeTransform = orientationRotation;
            #endif

            #if WIN_RT
            VideoRotation orientationRotation = VideoRotation.None;
            switch (newOrientation)
            {
                case ControlOrientation.Default:
                    orientationRotation = VideoRotation.None;
                    break;
                case ControlOrientation.Clockwise90Degrees:
                    orientationRotation = VideoRotation.Clockwise90Degrees;
                    break;
                case ControlOrientation.Clockwise180Degrees:
                    orientationRotation = VideoRotation.Clockwise180Degrees;
                    break;
                case ControlOrientation.Clockwise270Degrees:
                    orientationRotation = VideoRotation.Clockwise270Degrees;
                    break;
            } // end switch 

            if (VideoSource != null)
            {
                VideoSource.SetPreviewRotation(orientationRotation);
            }
#endif
        }
        #endregion // Overridables / Event Triggers

        #region Public Methods
        /// <summary>
        /// Starts any enabled AR services (Motion, Camera, etc)
        /// </summary>
        #if WINDOWS_PHONE
        public void StartServices()
        #endif
        #if WIN_RT
        public async Task StartServices()
        #endif
        {
            // If services are already started, ignore
            if (servicesRunning) { return; }
            
            // Started
            servicesRunning = true;

            // Clear any errors
            serviceErrors.Clear();

            if (CameraEnabled)
            {
                #if WINDOWS_PHONE
                StartCamera();
                #endif
                
                #if WIN_RT
                await StartCamera();
                #endif
            }
            if (LocationEnabled)
            {
                #if WINDOWS_PHONE
                StartLocation();
                #endif

                #if WIN_RT
                await StartLocation();
                #endif
            }
            if (MotionEnabled)
            {
                StartMotion();
            }

            // Notify of errors?
            if (serviceErrors.Count > 0)
            {
                OnServiceErrors(new ServiceErrorsEventArgs(serviceErrors));
            }
        }

        /// <summary>
        /// Stops any enabled AR services (Motion, Camera, etc)
        /// </summary>
        public void StopServices()
        {
            // If services are not started, ignore
            if (!servicesRunning) { return; }

            // Not started
            servicesRunning = false;

            if (CameraEnabled)
            {
                StopCamera();
            }
            if (LocationEnabled)
            {
                StopLocation();
            }
            if (MotionEnabled)
            {
                StopMotion();
            }

            // Not started
            servicesRunning = false;
        }
        #endregion // Public Methods

        #region Public Properties
        /// <summary>
        /// Gets or sets the collection of ARItems rendered by the <see cref="ARDisplay"/>. This is a dependency property.
        /// </summary>
        /// <value>
        /// The collection of ARItems rendered by the <see cref="ARDisplay"/>.
        /// </value>
        #if WINDOWS_PHONE
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


#if WP7

        public void HandleOrientationChange(OrientationChangedEventArgs args)
        {
            ControlOrientation orientation = ControlOrientation.Default;

            switch (args.Orientation)
            {
                case PageOrientation.LandscapeLeft:
                    orientation = ControlOrientation.Clockwise270Degrees;
                    break;
                case PageOrientation.LandscapeRight:
                    orientation = ControlOrientation.Clockwise90Degrees;
                    break;
            }

            Orientation = orientation;
        }

#endif // WP7

        /// <summary>
        /// Gets or sets a matrix that represents where the user is looking. This is a dependency property.
        /// </summary>
        /// <value>
        /// A matrix that represents where the user is looking.
        /// </value>
        #if WINDOWS_PHONE
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
        /// Gets or sets a value that indicates if the camera preview is enabled. This is a dependency property.
        /// </summary>
        /// <value>
        /// <c>true</c> if the camera preview is enabled; otherwise <c>false</c>.
        /// </value>
        #if WINDOWS_PHONE
        [Category("AR")]
        #endif
        public bool CameraEnabled
        {
            get
            {
                return (bool)GetValue(CameraEnabledProperty);
            }
            set
            {
                SetValue(CameraEnabledProperty, value);
            }
        }

        #if WIN_RT
        /// <summary>
        /// Gets or sets the ID of the camera device to use.
        /// </summary>
        /// <value>
        /// The ID of the camera device to use of the <c>ARDisplay</c>.
        /// </value>
        public string CameraId { get; set; }
        #endif

        /// <summary>
        /// Gets or sets the direction the user is looking in degrees. This is a dependency property.
        /// </summary>
        /// <value>
        /// The direction the user is looking in degrees.
        /// </value>
        #if WINDOWS_PHONE
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
        #if WINDOWS_PHONE
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
        /// Gets or sets a value that indicates if Location tracking is enabled. This is a dependency property.
        /// </summary>
        /// <value>
        /// <c>true</c> if Location tracking is enabled; otherwise <c>false</c>.
        /// </value>
        #if WINDOWS_PHONE
        [Category("AR")]
        #endif
        public bool LocationEnabled
        {
            get
            {
                return (bool)GetValue(LocationEnabledProperty);
            }
            set
            {
                SetValue(LocationEnabledProperty, value);
            }
        }

        /// <summary>
        /// Gets the <see cref="Motion"/> instance used by the ARDisplay.
        /// </summary>
        #if WINDOWS_PHONE
        [Category("AR")]
        public Motion Motion
        #elif WIN_RT
        public OrientationSensor Motion
        #endif

        {
            get
            {
                return motion;
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates if motion tracking is enabled. This is a dependency property.
        /// </summary>
        /// <value>
        /// <c>true</c> if motion tracking is enabled; otherwise <c>false</c>.
        /// </value>
        #if WINDOWS_PHONE
        [Category("AR")]
        #endif
        public bool MotionEnabled
        {
            get
            {
                return (bool)GetValue(MotionEnabledProperty);
            }
            set
            {
                SetValue(MotionEnabledProperty, value);
            }
        }


        #if WINDOWS_PHONE
        [Category("AR")]
        #endif
        public ControlOrientation Orientation
        {
            get { return (ControlOrientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        /// <summary>
        /// Gets the PhotoCamera used by the ARDisplay.
        /// </summary>
        #if WINDOWS_PHONE
        [Category("AR")]
        public PhotoCamera PhotoCamera
        {
            get
            {
                return photoCamera;
            }
        }
        #endif

        /// <summary>
        /// Gets a value that indicates if AR services (Motion, Camera, etc.) have been started.
        /// </summary>
        /// <value><c>true</c> if AR services have been started; otherwise <c>false</c>.</value>
        #if WINDOWS_PHONE
        [Category("AR")]
        #endif
        public bool ServicesRunning
        {
            get
            {
                return servicesRunning;
            }
        }

        /// <summary>
        /// Gets or sets the direction the user is traveling in degrees. This is a dependency property.
        /// </summary>
        /// <value>
        /// The direction the user is traveling in degrees.
        /// </value>
        #if WINDOWS_PHONE
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
        #if WINDOWS_PHONE
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

        /// <summary>
        /// Gets the collection of child views associated with this display.
        /// </summary>
        #if WINDOWS_PHONE
        [Category("AR")]
        #endif
        public ObservableCollection<IARView> Views
        {
            get
            {
                return views;
            }
        }
        #endregion // Public Properties

        #if WINDOWS_PHONE
        private int attitudeRefreshRate = 500;

        /// <summary>
        /// Defines time between updates used for reading attitude heading from compass.
        /// WindowsPhone only.
        /// </summary>
        public int AttitudeRefreshRate
        {
            get { return attitudeRefreshRate; }
            set
            {
                if (value != attitudeRefreshRate && value > 0)
                {
                    attitudeRefreshRate = value;
                    if (compass != null)
                    {
                        compass.TimeBetweenUpdates = TimeSpan.FromMilliseconds(attitudeRefreshRate);
                    }
                }
            }
        }

        #endif  //WINDOWS_PHONE

        private int movementThreshold = 10;

        /// <summary>
        /// Set the movement threshold on location service. Default value = 10
        /// </summary>
        public int MovementThreshold
        {
            get { return movementThreshold; }
            set
            {
                if (value != movementThreshold && value > 0)
                {
                    movementThreshold = value;
                    if (locationService != null)
                    {
                        locationService.MovementThreshold = movementThreshold;
                    }
                }
            }
        }

        #region Public Events
        /// <summary>
        /// Occurs when the value of the <see cref="Location"/> property has changed.
        /// </summary>
        public event EventHandler LocationChanged;

        /// <summary>
        /// Occurs when an error was encountered starting or stopping a service.
        /// </summary>
        public event EventHandler<ServiceErrorsEventArgs> ServiceErrors;
        #endregion // Public Events

        #endregion // Instance Version
    }

}
