using BingMapsAR.Models;
using GART;
using GART.BaseControls;
using GART.Controls;
using GART.Data;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Maps.Toolkit;
using Microsoft.Phone.Tasks;
using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage.Streams;
using Location = System.Device.Location.GeoCoordinate;

namespace BingMapsAR.WP8
{
    public partial class MainPage : PhoneApplicationPage
    {
        #region Private Properties

        /// <summary>
        /// Search Radius in meters
        /// </summary>
        private double SearchRadius = 300;

        /// <summary>
        /// Distance in meeters the user must move before a new set of items are loaded.
        /// </summary>
        private double DistanceThreshold = 100;

        /// <summary>
        /// Maximium number of items returned by the Bing Spatial Data Services
        /// </summary>
        private const int MaxResultsPerQuery = 30;

        private ARItem SelectedItem;

        private MapLayer SelectedItemLayer;
        private MapLayer ItemsLayer;

        private Location LastSearchLocation;

        private MapOverlay userPushpin;

        #endregion

        #region Constructor

        public MainPage()
        {
            this.InitializeComponent();

            OverheadMap.Credentials = new MapCredentials()
            {
                ApplicationId = App.Current.Resources["BingCredentials"].ToString()
            };

            this.Loaded += (s, a) =>
            {
                ItemsLayer = new MapLayer();
                OverheadMap.Map.Layers.Add(ItemsLayer);

                SelectedItemLayer = new MapLayer();
                OverheadMap.Map.Layers.Add(SelectedItemLayer);

                MapLayer userPinLayer = new MapLayer();
                OverheadMap.Map.Layers.Add(userPinLayer);

                userPushpin = new MapOverlay()
                {
                    GeoCoordinate = ARDisplay.Location,
                    Content = new Controls.UserPushpin()
                    {
                        DataContext = ARDisplay
                    }
                };

                userPinLayer.Add(userPushpin);

                GetData();
            };
        }

        #endregion

        #region Overrides / Event Handlers

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            base.OnBackKeyPress(e);

            if (ItemPanel.Visibility == System.Windows.Visibility.Visible)
            {
                ItemPanel.Visibility = System.Windows.Visibility.Collapsed;
                e.Cancel = true;
            }
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            // Stop AR services
            ARDisplay.StopServices();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Start AR services
            ARDisplay.StartServices();
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

        private void MapButton_Click(object sender, System.EventArgs e)
        {
            OverheadMap.Visibility = System.Windows.Visibility.Visible;
            WorldView.Visibility = System.Windows.Visibility.Collapsed;
            ItemPanel.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void ThreeDButton_Click(object sender, System.EventArgs e)
        {
            OverheadMap.Visibility = System.Windows.Visibility.Collapsed;
            WorldView.Visibility = System.Windows.Visibility.Visible;
            ItemPanel.Visibility = System.Windows.Visibility.Collapsed;
        }

        #endregion // Overrides / Event Handlers

        #region Button Handlers

        private void PoiItem_Tapped(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ARItem selectedItem = null;

            if (sender is Grid)
            {
                selectedItem = (sender as Grid).DataContext as ARItem;

                //Check to see if the item is already selected. If so then toggle it.
                if (selectedItem == SelectedItem)
                {
                    selectedItem = null;
                }
            }

            if (sender is Ellipse)
            {
                selectedItem = (sender as Ellipse).Tag as ARItem;
            }

            SetSelectedItem(selectedItem);
        }

        private void Directions_Tapped(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (SelectedItem != null)
            {
                var poiItem = SelectedItem as PoiItem;

                MapsDirectionsTask directionTask = new MapsDirectionsTask()
                {
                    Start = new LabeledMapLocation("My Location", ARDisplay.Location),
                    End = new LabeledMapLocation(poiItem.Name, poiItem.GeoLocation)
                };

                directionTask.Show();
            }
        }

        private void Share_Tapped(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (SelectedItem != null)
            {
                var poiItem = SelectedItem as PoiItem;

                //Create a converter for converting the Entity Type ID of the selected item to a user friendly text.
                BingMapsAR.Converters.EntityTypeNameConverter converter = new BingMapsAR.Converters.EntityTypeNameConverter();

                EmailComposeTask emailComposeTask = new EmailComposeTask()
                {
                    Subject = poiItem.Name,
                    Body = string.Format("{0} - {1}\r\n{2}, {3} {4}\r\nPhone: {5}",
                    poiItem.Name, converter.Convert(poiItem.EntityTypeID, null, null, null),
                    poiItem.AddressLine, poiItem.Locality, poiItem.PostalCode, poiItem.Phone)
                };

                emailComposeTask.Show();
            }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// An event handler for when the location property changes in the ARDisplay
        /// </summary>
        private void ARDisplay_LocationChanged(object sender, EventArgs e)
        {
            if (userPushpin != null)
            {
                userPushpin.GeoCoordinate = ARDisplay.Location;
            }
            
            GetData();
        }

        /// <summary>
        /// An event handler for any errors thrown by the ARDisplay services. 
        /// The errors are caught and displayed to the user. 
        /// </summary>
        private void ARDisplay_ServiceErrors(object sender, ServiceErrorsEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var error in e.Errors)
            {
                sb.AppendLine(string.Format("There was a problem with {0}: {1}", error.Service, error.Exception.Message));
            }

            MessageBox.Show(sb.ToString(), "Error", MessageBoxButton.OK);
        }

        #endregion

        #region Private Methods

        private void SetSelectedItem(ARItem item)
        {
            //Clear the Selected Item Layer on the map
            SelectedItemLayer.Clear();

            //Update the selected item
            SelectedItem = item;

            //Update the selected item panel data context
            ItemPanel.DataContext = SelectedItem;

            //Update the selected item in the WorldView
            WorldView.SelectedItem = SelectedItem;

            if (SelectedItem != null)
            {
                MapOverlay pin = new MapOverlay
                {
                    GeoCoordinate = SelectedItem.GeoLocation,
                    Content = new Ellipse()
                                {
                                    Width = 40,
                                    Height = 40,
                                    Fill = new SolidColorBrush(Colors.Red),
                                    Stroke = new SolidColorBrush(Colors.White),
                                    StrokeThickness = 4,
                                    Tag = item
                                }
                };

                (pin.Content as Ellipse).Tap += PoiItem_Tapped;

                SelectedItemLayer.Add(pin);

                ItemPanel.Visibility = System.Windows.Visibility.Visible;
            }
        }

        /// <summary>
        /// Begins an asynchronous search against the Bing Spatial Data Services.
        /// </summary>
        private void GetData()
        {
            //If the last search location is null or if the user has moved more than the distance threshold then do a new search
            if (LastSearchLocation == null || ARHelper.DistanceTo(ARDisplay.Location, LastSearchLocation) > DistanceThreshold)
            {
                // Last search location is now this location
                LastSearchLocation = ARDisplay.Location;

                //Remove items from ARDisplay
                ARDisplay.ARItems.Clear();

                ItemsLayer.Clear();

                //Create Search URL for Bing Spatial Data Service

                string baseURL;
                bool containsSelectedItem = false;

                //Get the hash code of the selected item for easier comparisons
                int selectedItemHash = (SelectedItem != null) ? SelectedItem.GetHashCode() : -1;

                //Switch between the NAVTEQ POI data sets for NA and EU based on where the user is.
                if (LastSearchLocation.Longitude < -30)
                {
                    //Use the NAVTEQ NA data source: http://msdn.microsoft.com/en-us/library/hh478192.aspx
                    baseURL = "http://spatial.virtualearth.net/REST/v1/data/f22876ec257b474b82fe2ffcb8393150/NavteqNA/NavteqPOIs";
                }
                else
                {
                    //Use the NAVTEQ EU data source: http://msdn.microsoft.com/en-us/library/hh478193.aspx
                    baseURL = "http://spatial.virtualearth.net/REST/v1/data/c2ae584bbccc4916a0acf75d1e6947b4/NavteqEU/NavteqPOIs";
                }

                string poiRequest = string.Format("{0}?spatialFilter=nearby({1:N5},{2:N5},{3:N2})&$format=json&$top={4}&key={5}",
                    baseURL, LastSearchLocation.Latitude, LastSearchLocation.Longitude, SearchRadius / 1000, MaxResultsPerQuery, OverheadMap.Credentials.ApplicationId);

                GetResponse(new Uri(poiRequest), (response) =>
                {
                    if (response != null &&
                        response.ResultSet != null &&
                        response.ResultSet.Results != null &&
                        response.ResultSet.Results.Length > 0)
                    {
                        GeoCoordinateCollection locs = new GeoCoordinateCollection();

                        foreach (var r in response.ResultSet.Results)
                        {
                            Location loc = new Location(r.Latitude, r.Longitude);
                            locs.Add(loc);

                            PoiItem item = new PoiItem()
                            {
                                Name = r.DisplayName,
                                AddressLine = r.AddressLine,
                                //Some locations have postal code information in the locality property.
                                Locality = (r.PostalCode.StartsWith(r.Locality)) ? r.AdminDistrict : r.Locality,
                                PostalCode = r.PostalCode,
                                EntityTypeID = r.EntityTypeID,
                                Phone = Helpers.FormatPhoneNumber(r.Phone),
                                GeoLocation = loc,
                                Distance = Math.Round(ARHelper.DistanceTo(loc, LastSearchLocation))
                            };
                            ARDisplay.ARItems.Add(item);

                            //Check to see if the currently selected item is in the new results
                            containsSelectedItem |= (selectedItemHash == item.GetHashCode());
                        }

                        //If the selected item isn't in the current results unselect it.
                        if (!containsSelectedItem)
                        {
                            SetSelectedItem(null);
                        }

                        //Set the map view to show all the locations.
                        OverheadMap.Map.SetView(ARDisplay.Location, 15);
                    }
                });
            }
        }

        private void GetResponse(Uri uri, Action<Response> callback)
        {
            System.Net.WebClient client = new System.Net.WebClient();
            client.OpenReadCompleted += (s, a) =>
            {
                try
                {
                    using (var stream = a.Result)
                    {
                        DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Response));

                        if (callback != null)
                        {
                            callback(ser.ReadObject(stream) as Response);
                        }
                    }
                }
                catch (Exception e)
                {
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
            };
            client.OpenReadAsync(uri);
        }

        #endregion
    }
}
