using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using Microsoft.Phone.Notification;
using System.Diagnostics;
using System.Windows.Threading;
using System.Collections.ObjectModel;

namespace WPPushNotification.TestClient
{
    public partial class App : Application
    {
        /// <summary>
        /// Provides easy access to the root frame of the Phone Application.
        /// </summary>
        /// <returns>The root frame of the Phone Application.</returns>
        public PhoneApplicationFrame RootFrame { get; private set; }

        /// <summary>
        /// Whether or not it is necessary to refresh the pin status of the secondary tiles.
        /// </summary>
        public bool TileRefreshNeeded { get; set; }

        /// <summary>
        /// Contains information about the locations displayed by the application.
        /// </summary>
        public Dictionary<string, LocationInformation> Locations { get; set; }

        /// <summary>
        /// Returns the application's dispatcher.
        /// </summary>
        public Dispatcher Dispatcher
        {
            get
            {
                return Deployment.Current.Dispatcher;
            }
        }

        private PushHandler PushHandler { get; set; }

        /// <summary>
        /// Constructor for the Application object.
        /// </summary>
        public App()
        {
            // Global handler for uncaught exceptions. 
            UnhandledException += Application_UnhandledException;

            // Show graphics profiling information while debugging.
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Display the current frame rate counters.
                Application.Current.Host.Settings.EnableFrameRateCounter = false;

                // Show the areas of the app that are being redrawn in each frame.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode, 
                // which shows areas of a page that are being GPU accelerated with a colored overlay.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;
            }

            // Standard Silverlight initialization
            InitializeComponent();

            // Phone-specific initialization
            InitializePhoneApplication();

            // Create the shell tile schedule instance
            CreateShellTileSchedule();
        }

        // To store the instance for the application lifetime
        private ShellTileSchedule shellTileSchedule;

        /// <summary>
        /// Requests data for a specific location from the application server.
        /// </summary>
        /// <param name="locationName">The name of the location for which data is requested.</param>
        public void RequestLatestData(string locationName)
        {
            PushHandler.RequestLatestData(locationName);
        }

        /// <summary>
        /// Create the application shell tile schedule instance
        /// </summary>
        private void CreateShellTileSchedule()
        {
            shellTileSchedule = new ShellTileSchedule();
            shellTileSchedule.Recurrence = UpdateRecurrence.Interval;
            shellTileSchedule.Interval = UpdateInterval.EveryHour;
            shellTileSchedule.StartTime = DateTime.Now;
            shellTileSchedule.RemoteImageUri = new Uri(@"http://cdn3.afterdawn.fi/news/small/windows-phone-7-series.png");
            shellTileSchedule.Start();
        }

        // Code to execute when the application is launching (e.g., from Start)
        // This code will not execute when the application is reactivated
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
            TileRefreshNeeded = true;

            InitializeLocations();
            RefreshTilesPinState();

            PushHandler = new PushHandler(Resources["PushStatus"] as Status, Locations, Dispatcher);
            PushHandler.EstablishConnections();
        }

        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
            if (!e.IsApplicationInstancePreserved)
            {
                // The application was tombstoned, so restore its state
                foreach (var keyValue in PhoneApplicationService.Current.State)
                {
                    Locations[keyValue.Key] = keyValue.Value as LocationInformation;
                }

                // Reconnect to the MSPN
                PushHandler = new PushHandler(Resources["PushStatus"] as Status, Locations, Dispatcher);
                PushHandler.EstablishConnections();
            }
            else if (!PushHandler.ConnectionEstablished)
            {
                // Connection was not fully established before fast app switching occurred
                PushHandler.EstablishConnections();
            }

            RefreshTilesPinState();
        }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
            foreach (var keyValue in Locations)
            {
                PhoneApplicationService.Current.State[keyValue.Key] = keyValue.Value;
            }
        }

        // Code to execute when the application is closing (e.g., user hit Back)
        // This code will not execute when the application is deactivated
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
        }

        // Code to execute if a navigation fails
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        // Code to execute on Unhandled Exceptions
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        #region Phone application initialization

        // Avoid double-initialization
        private bool phoneApplicationInitialized = false;

        // Do not add any additional code to this method
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            RootFrame = new PhoneApplicationFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Ensure we don't initialize again
            phoneApplicationInitialized = true;
        }

        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        #endregion

        #region Helper methods

        /// <summary>
        /// Writes debug output in debug mode.
        /// </summary>
        /// <param name="message">The message to write to debug output.</param>
        internal static void Trace(string message)
        {
#if DEBUG
            Debug.WriteLine(message);
#endif
        }

        /// <summary>
        /// Initializes the contents of the location dictionary.
        /// </summary>
        private void InitializeLocations()
        {
            List<LocationInformation> locationList = new List<LocationInformation>(new[] { 
                new LocationInformation { Name = "Redmond", TilePinned = false },
                new LocationInformation { Name = "Moscow", TilePinned = false },
                new LocationInformation { Name = "Paris", TilePinned = false },
                new LocationInformation { Name = "London", TilePinned = false },
                new LocationInformation { Name = "New York", TilePinned = false }
            });

            Locations = locationList.ToDictionary(l => l.Name);
        }

        /// <summary>
        /// Sees which of the application's sub-tiles are pinned and updates the location information accordingly.
        /// </summary>
        private void RefreshTilesPinState()
        {
        }

        #endregion
    }
}