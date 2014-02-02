using Bing.Maps;
using GART;
using GART.Controls;
using GART.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SimpleAR
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        #region Member Variables
        private Random rand = new Random();
        #endregion // Member Variables

        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;
        }


        #region Internal Methods
        private void AddLabel(Location location, string label)
        {
            // We'll use the specified text for the content and we'll let 
            // the system automatically project the item into world space
            // for us based on the Geo location.
            ARItem item = new ARItem()
            {
                Content = label,
                GeoLocation = location,
            };

            ARDisplay.ARItems.Add(item);
        }

        private void AddNearbyLabels()
        {
            // Start with the current location
            var current = ARDisplay.Location;

            // We'll add three Labels
            for (int i = 0; i < 3; i++)
            {
                // Create a new location based on the users location plus
                // a random offset.
                Location offset = new Location()
                {
                    Latitude = current.Latitude + ((double)rand.Next(-60, 60)) / 100000,
                    Longitude = current.Longitude + ((double)rand.Next(-60, 60)) / 100000,
                    // Altitude = Double.NaN // NaN will keep it on the horizon
                };

                AddLabel(offset, "Location " + i);
            }
        }

        /// <summary>
        /// Switches between rottaing the Heading Indicator or rotating the Map to the current heading.
        /// </summary>
        private void SwitchHeadingMode()
        {
            if (HeadingIndicator.RotationSource == RotationSource.AttitudeHeading)
            {
                HeadingIndicator.RotationSource = RotationSource.North;
                OverheadMap.RotationSource = RotationSource.AttitudeHeading;
            }
            else
            {
                OverheadMap.RotationSource = RotationSource.North;
                HeadingIndicator.RotationSource = RotationSource.AttitudeHeading;
            }
        }
        #endregion // Internal Methods


        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            var t = ARDisplay.StartServices();
        }

        

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            // ARDisplay.StartServices();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            ARDisplay.StopServices();
        }

        private void MapButton_Click(object sender, RoutedEventArgs e)
        {
            UIHelper.ToggleVisibility(OverheadMap);
        }

        private void HeadingButton_Click(object sender, RoutedEventArgs e)
        {
            UIHelper.ToggleVisibility(HeadingIndicator);
        }

        private void WorldButton_Click(object sender, RoutedEventArgs e)
        {
            // UIHelper.ToggleVisibility(WorldView);
        }

        private void RotationButton_Click(object sender, RoutedEventArgs e)
        {
            SwitchHeadingMode();
        }

        private void ARDisplay_ServiceErrors(object sender, ServiceErrorsEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var error in e.Errors)
            {
                sb.AppendLine(string.Format("There was a problem with {0}: {1}", error.Service, error.Exception.Message));
            }

            MessageDialog md = new MessageDialog(sb.ToString());
            var t = md.ShowAsync();
        }

        private void AddItemsButton_Click(object sender, RoutedEventArgs e)
        {
            AddNearbyLabels();
        }

        private void ClearItemsButton_Click(object sender, RoutedEventArgs e)
        {
            ARDisplay.ARItems.Clear();
        }
    }
}
