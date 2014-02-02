/* 
    Copyright (c) 2012 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using sdkLocationWP8CS.Resources;

using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using System.IO.IsolatedStorage;

namespace sdkLocationWP8CS
{
    public partial class MainPage : PhoneApplicationPage
    {

        
        bool tracking = false;



        // Constructor
        public MainPage()
        {
            InitializeComponent();

            BuildApplicationBar();
        }

        // When the page is loaded, make sure that you have obtained the users consent to use their location
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (IsolatedStorageSettings.ApplicationSettings.Contains("LocationConsent"))
            {
                // User has already opted in or out of Location
                return;
            }
            else
            {
                MessageBoxResult result =
                    MessageBox.Show("This app accesses your phone's location. Is that ok?",
                    "Location",
                    MessageBoxButton.OKCancel);

                if (result == MessageBoxResult.OK)
                {
                    IsolatedStorageSettings.ApplicationSettings["LocationConsent"] = true;
                }
                else
                {
                    IsolatedStorageSettings.ApplicationSettings["LocationConsent"] = false;
                }

                IsolatedStorageSettings.ApplicationSettings.Save();

                UpdateAppBar();
            }
        }


        // Get the current location of the phone. To reduce power consumption, it is recommended that you
        // use one-shot location unless your app requires location tracking.
        private async void OneShotLocation_Click(object sender, RoutedEventArgs e)
        {

            if ((bool)IsolatedStorageSettings.ApplicationSettings["LocationConsent"] != true)
            {
                // The user has opted out of Location. 
                StatusTextBlock.Text = "You have opted out of location. Use the app bar to turn location back on";
                return;
            }

            Geolocator geolocator = new Geolocator();
            geolocator.DesiredAccuracyInMeters = 50;

            try
            {
                // Request the current position
                Geoposition geoposition = await geolocator.GetGeopositionAsync(
                    maximumAge: TimeSpan.FromMinutes(5),
                    timeout: TimeSpan.FromSeconds(10)
                    );

                LatitudeTextBlock.Text = geoposition.Coordinate.Latitude.ToString("0.00");
                LongitudeTextBlock.Text = geoposition.Coordinate.Longitude.ToString("0.00");
                StatusTextBlock.Text = "location obtained";
            }
            catch (Exception ex)
            {
                if ((uint)ex.HResult == 0x80004004)
                {
                    // the application does not have the right capability or the location master switch is off
                    StatusTextBlock.Text = "location  is disabled in phone settings.";
                }
                //else
                {
                    // something else happened acquring the location
                }
            }
        }


        private void TrackLocation_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)IsolatedStorageSettings.ApplicationSettings["LocationConsent"] != true)
            {
                // The user has opted out of Location.
                StatusTextBlock.Text = "You have opted out of location. Use the app bar to turn location back on";
                return;
            } 

            
            if (!tracking)
            {
                // If not currently tacking, create a new Geolocator and set options.
                // Assigning the PositionChanged event handler begins location acquisition.

                if (App.Geolocator == null)
                {
                    // Use the app's global Geolocator variable
                    App.Geolocator = new Geolocator();
                }

                App.Geolocator.DesiredAccuracy = PositionAccuracy.High;
                App.Geolocator.MovementThreshold = 100; // The units are meters.

                App.Geolocator.StatusChanged += geolocator_StatusChanged;
                App.Geolocator.PositionChanged += geolocator_PositionChanged;

                tracking = true;
                TrackLocationButton.Content = "stop tracking";
            }
            else
            {
                // To stop location acquisition, remove the position changed and status changed event handlers.
                App.Geolocator.PositionChanged -= geolocator_PositionChanged;
                App.Geolocator.StatusChanged -= geolocator_StatusChanged;
                App.Geolocator = null;

                tracking = false;
                TrackLocationButton.Content = "track location";
                StatusTextBlock.Text = "stopped";
            }
        }

        // The PositionChanged event is raised when new position data is available
        void geolocator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            if (!App.RunningInBackground)
            {
                Dispatcher.BeginInvoke(() =>
                {
                    LatitudeTextBlock.Text = args.Position.Coordinate.Latitude.ToString("0.00");
                    LongitudeTextBlock.Text = args.Position.Coordinate.Longitude.ToString("0.00");
                });
            }
            else
            {
                Microsoft.Phone.Shell.ShellToast toast = new Microsoft.Phone.Shell.ShellToast();
                toast.Content = args.Position.Coordinate.Latitude.ToString("0.00");
                toast.Title = "Location: ";
                toast.NavigationUri = new Uri("/MainPage.xaml", UriKind.Relative);
                toast.Show();

            }
        }

        // The StatusChanged event is raised when the status of the location service changes.
        void geolocator_StatusChanged(Geolocator sender, StatusChangedEventArgs args)
        {
            
            string status = "";

            switch (args.Status)
            {
                case PositionStatus.Disabled:
                    // the application does not have the right capability or the location master switch is off
                    status = "location is disabled in phone settings";
                    break;
                case PositionStatus.Initializing:
                    // the geolocator started the tracking operation
                    status = "initializing";
                    break;
                case PositionStatus.NoData:
                    // the location service was not able to acquire the location
                    status = "no data";
                    break;
                case PositionStatus.Ready:
                    // the location service is generating geopositions as specified by the tracking parameters
                    status = "ready";
                    break;
                case PositionStatus.NotAvailable:
                    status = "not available";
                    // not used in WindowsPhone, Windows desktop uses this value to signal that there is no hardware capable to acquire location information
                    break;
                case PositionStatus.NotInitialized:
                    // the initial state of the geolocator, once the tracking operation is stopped by the user the geolocator moves back to this state

                    break;
            }


            if (!App.RunningInBackground)
            {
                Dispatcher.BeginInvoke(() =>
                {
                    StatusTextBlock.Text = status;
                });
            }
        }

        // When the page is removed from the backstack, remove the event handlers to stop location acquisition
        protected override void OnRemovedFromJournal(System.Windows.Navigation.JournalEntryRemovedEventArgs e)
        {
            App.Geolocator.PositionChanged -= geolocator_PositionChanged;
            App.Geolocator.StatusChanged -= geolocator_StatusChanged;
            App.Geolocator = null;
        }

        // Allow the user to toggle opting in and out of location with an ApplicationBar menu item.
        private void BuildApplicationBar()
        {
            // Set the page's ApplicationBar to a new instance of ApplicationBar.
            ApplicationBar = new ApplicationBar();

            ApplicationBarMenuItem menuItem = new ApplicationBarMenuItem();
            menuItem.Text = "loading";
    
            menuItem.Click += menuItem_Click;
            ApplicationBar.MenuItems.Add(menuItem);
            ApplicationBar.IsMenuEnabled = true;
        }

        void menuItem_Click(object sender, EventArgs e)
        {
            if ((bool)IsolatedStorageSettings.ApplicationSettings["LocationConsent"] == true)
            {
                IsolatedStorageSettings.ApplicationSettings["LocationConsent"] = false;
            }
            else
            {
                IsolatedStorageSettings.ApplicationSettings["LocationConsent"] = false;
            }
            UpdateAppBar();

        }

       
        void UpdateAppBar()
        {

            ApplicationBarMenuItem menuItem = (ApplicationBarMenuItem)ApplicationBar.MenuItems[0];

            if ((bool)IsolatedStorageSettings.ApplicationSettings["LocationConsent"] == false)
            {
                menuItem.Text = "opt in to location";
            }
            else
            {
                menuItem.Text = "opt out of location";
            }
        }

            
    }
}
