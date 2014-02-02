using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using SimpleAR.Resources;
using GART.BaseControls;
using GART;
using Location = System.Device.Location.GeoCoordinate;
using GART.Data;
using GART.Controls;

namespace SimpleAR
{
    public partial class MainPage : PhoneApplicationPage
    {
        #region Member Variables
        private Random rand = new Random();
        #endregion // Member Variables

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            AddNearbyLabels();

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
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
                    Altitude = Double.NaN // NaN will keep it on the horizon
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

        #region Overrides / Event Handlers
        private void AddLocationsMenu_Click(object sender, EventArgs e)
        {
            AddNearbyLabels();
        }

        private void ClearLocationsMenu_Click(object sender, EventArgs e)
        {
            ARDisplay.ARItems.Clear();
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            // Stop AR services
            ARDisplay.StopServices();

            base.OnNavigatedFrom(e);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            // Start AR services
            ARDisplay.StartServices();

            base.OnNavigatedTo(e);
        }

        /// <summary>
        /// To support any orientation, override this method and call
        /// ARDisplay.HandleOrientationChange() method
        /// </summary>
        /// <param name="e"></param>
        protected override void OnOrientationChanged(OrientationChangedEventArgs e)
        {
            base.OnOrientationChanged(e);

            ControlOrientation orientation = ControlOrientation.Default;

            switch (e.Orientation)
            {
                case PageOrientation.LandscapeLeft:
                    orientation = ControlOrientation.Clockwise270Degrees;
                    break;
                case PageOrientation.LandscapeRight:
                    orientation = ControlOrientation.Clockwise90Degrees;
                    break;
            }

            ARDisplay.Orientation = orientation;
        }

        private void HeadingButton_Click(object sender, System.EventArgs e)
        {
            UIHelper.ToggleVisibility(HeadingIndicator);
        }

        private void MapButton_Click(object sender, System.EventArgs e)
        {
            UIHelper.ToggleVisibility(OverheadMap);
        }

        private void RotateButton_Click(object sender, System.EventArgs e)
        {
            SwitchHeadingMode();
        }

        private void ThreeDButton_Click(object sender, System.EventArgs e)
        {
            UIHelper.ToggleVisibility(WorldView);
        }
        #endregion // Overrides / Event Handlers
    }
}