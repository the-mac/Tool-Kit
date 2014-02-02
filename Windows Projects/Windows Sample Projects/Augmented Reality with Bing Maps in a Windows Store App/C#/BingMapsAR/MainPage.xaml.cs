using Bing.Maps;
using BingMapsAR.Controls;
using BingMapsAR.Models;
using GART;
using GART.Controls;
using GART.Data;
using System;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace BingMapsAR
{
    public sealed partial class MainPage : Page
    {
        #region Private Properties

        /// <summary>
        /// Search Radius in meters
        /// </summary>
        private double SearchRadius = 300;

        /// <summary>
        /// Maximium number of items returned by the Bing Spatial Data Services
        /// </summary>
        private const int MaxResultsPerQuery = 30;

        /// <summary>
        /// The item that the user has selected.
        /// </summary>
        private ARItem SelectedItem;

        /// <summary>
        /// A map layer for displaying the selected item in the overhead map. 
        /// This ensures the selected item is always above all the other items on the map.
        /// </summary>
        private MapLayer SelectedItemLayer;

        /// <summary>
        /// The last location a search was performed. 
        /// </summary>
        private Location LastSearchLocation;

        /// <summary>
        /// A reference to the DataTransferManager for the Share charm.
        /// </summary>
        private DataTransferManager dataTransferManager;

        /// <summary>
        /// A special Bing Maps key used for making requests to Bing Maps 
        /// non-billable by tying them into the current map session.
        /// </summary>
        private string sessionKey;

        #endregion

        #region Constructor
        
        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;

            //Get the DataTransferManager object associated with current window 
            dataTransferManager = DataTransferManager.GetForCurrentView();
        }

        #endregion

        #region Button Handlers

        /// <summary>
        /// Event handler for when a Poi Item is tapped.
        /// </summary>
        private void PoiItem_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
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

            if (sender is Pushpin)
            {
                selectedItem = (sender as Pushpin).Tag as ARItem;
            }

            SetSelectedItem(selectedItem);
        }

        /// <summary>
        /// Event handler for when the directions button is pressed.
        /// </summary>
        private async void Directions_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            //Launch the Maps App to get walking directions from the users location to the selected location
            //http://msdn.microsoft.com/en-us/library/windows/apps/jj635237.aspx
            Uri mapsAppUri = new Uri(string.Format("bingmaps:?rtp=pos.{0:N5}_{1:N5}~pos.{2:N5}_{3:N5}&mode=W",
                ARDisplay.Location.Latitude, ARDisplay.Location.Longitude,
                SelectedItem.GeoLocation.Latitude, SelectedItem.GeoLocation.Longitude));

            await Windows.System.Launcher.LaunchUriAsync(mapsAppUri);
        }

        /// <summary>
        /// Event handler for when the share button is pressed.
        /// </summary>
        private void Share_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            //Show the charm bar with Share option opened
            DataTransferManager.ShowShareUI();
        }

        #endregion
        
        #region Event Handlers

        /// <summary>
        /// Event handler for when the main page has loaded. 
        /// </summary>
        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            Window.Current.CoreWindow.VisibilityChanged += async (s, a) =>
            {
                if (a.Visible)
                {
                    //Register the share handler event
                    dataTransferManager.DataRequested += ShareHandler;

                    //Start or restart the services used by the ARDisplay
                    await ARDisplay.StartServices();
                }
                else
                { 
                    //Unregister the share handler event
                    dataTransferManager.DataRequested -= ShareHandler;

                    //Stop the services used by the ARDisplay to free up those resources while the app is not in use.
                    ARDisplay.StopServices();
                }
            };

            //Get a session key from the map.
            sessionKey = await OverheadMap.Map.GetSessionIdAsync();

            SelectedItemLayer = new MapLayer();
            OverheadMap.Map.Children.Add(SelectedItemLayer);

            UserPushpin userPin = new UserPushpin();
            userPin.DataContext = OverheadMap;
            OverheadMap.Map.Children.Add(userPin);

            //Clear the selected item panel data context
            ItemPanel.DataContext = null;
        }

        /// <summary>
        /// An event handler for when the user uses the Sharm Charm or presses the share button in the app. 
        /// If the user has selected an item this handler will generate HTMl that contains details about 
        /// the selected location and also a map of the location.
        /// </summary>
        /// <param name="sender">Data Transfer manager for Share Charm</param>
        /// <param name="e">Share Charm request</param>
        private void ShareHandler(DataTransferManager sender, DataRequestedEventArgs e)
        {
            if (SelectedItem != null)
            {
                var poiItem = SelectedItem as PoiItem;

                //A temporary image URL that is used for referencing the created map image.
                string localImage = "ms-appx:///images/map.png"; 

                //Use the Bing Maps REST Services to retrieve a static map image of the selected location.
                //Documentation: http://msdn.microsoft.com/en-us/library/ff701724.aspx
                string mapImageUrl = string.Format("http://dev.virtualearth.net/REST/v1/Imagery/Map/Road/{0:N5},{1:N5}/{2}?pp={0:N5},{1:N5};56;&mapSize=400,300&key={3}",
                    poiItem.GeoLocation.Latitude, poiItem.GeoLocation.Longitude, 17, OverheadMap.Credentials);

                //Create a converter for converting the Entity Type ID of the selected item to a user friendly text.
                Converters.EntityTypeNameConverter converter = new Converters.EntityTypeNameConverter();

                //Handle the Share Charm request and insert HTML content.
                DataRequest request = e.Request;
                request.Data.Properties.Title = "Share details";
                request.Data.Properties.Description = poiItem.Name;

                request.Data.SetHtmlFormat(HtmlFormatHelper.CreateHtmlFormat(
                    string.Format("<b>{0}</b> - {1}<br/>{2}, {3} {4}<br/>Phone: {5}<br/><br/><img src='{6}'>",
                    poiItem.Name, converter.Convert(poiItem.EntityTypeID, null, null, null),
                    poiItem.AddressLine, poiItem.Locality, poiItem.PostalCode, poiItem.Phone, localImage)));

                //Load the map image into the email as a stream.
                RandomAccessStreamReference streamRef = RandomAccessStreamReference.CreateFromUri(new Uri(mapImageUrl));
                request.Data.ResourceMap[localImage] = streamRef;
            }
        }

        /// <summary>
        /// An event handler for when the location property changes in the ARDisplay. 
        /// Begins an asynchronous search against the Bing Spatial Data Services.
        /// </summary>
        private async void ARDisplay_LocationChanged(object sender, EventArgs e)
        {
            // Last search location is now this location
            LastSearchLocation = ARDisplay.Location;

            if (LastSearchLocation != null)
            {
                //Remove items from ARDisplay
                ARDisplay.ARItems.Clear();

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
                    baseURL, LastSearchLocation.Latitude, LastSearchLocation.Longitude, SearchRadius / 1000, MaxResultsPerQuery, sessionKey);

                Response response = await GetResponse(new Uri(poiRequest));

                if (response != null &&
                    response.ResultSet != null &&
                    response.ResultSet.Results != null &&
                    response.ResultSet.Results.Length > 0)
                {
                    //Loop through the results and create PoiItems that can be added to the ARDisplay
                    foreach (var r in response.ResultSet.Results)
                    {
                        Location loc = new Location(r.Latitude, r.Longitude);

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
            }
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
                //Ignore errors from the camera. The camera is optional.
                if (error.Service != ARService.Camera)
                {
                    sb.AppendLine(string.Format("There was a problem with {0}: {1}", error.Service, error.Exception.Message));
                }
            }

            if (sb.Length > 0)
            {
                new MessageDialog(sb.ToString(), "Error").ShowAsync();
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// HAndles the logic for setting the selected item.
        /// </summary>
        /// <param name="item">Item that has beeen selected.</param>
        private void SetSelectedItem(ARItem item)
        {
            //Clear the Selected Item Layer on the map
            SelectedItemLayer.Children.Clear();

            //Update the selected item
            SelectedItem = item;

            //Update the selected item panel data context
            ItemPanel.DataContext = SelectedItem;

            //Update the selected item in the WorldView
            WorldView.SelectedItem = SelectedItem;

            if (SelectedItem != null)
            {
                //Highlight the selected item on the map with a red pin.
                Pushpin pin = new Pushpin();
                pin.Background = new SolidColorBrush(Colors.Red);
                MapLayer.SetPosition(pin, SelectedItem.GeoLocation);
                SelectedItemLayer.Children.Add(pin);

                //Center the map over the selected item.
                OverheadMap.Map.SetView(SelectedItem.GeoLocation);
            }
        }

        /// <summary>
        /// A method for making REST requests to the Bing Maps REST services and processing the response.
        /// </summary>
        /// <param name="uri">Uri with the REST request.</param>
        /// <returns>A response object from the Bing Maps REST services.</returns>
        private async Task<Response> GetResponse(Uri uri)
        {
            try
            {
                System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
                var response = await client.GetAsync(uri);

                using (var stream = await response.Content.ReadAsStreamAsync())
                {
                    DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Response));

                    return ser.ReadObject(stream) as Response;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
        
        #endregion
    }
}
