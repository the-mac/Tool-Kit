/* 
    Copyright (c) 2012 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
using Microsoft.Phone.Controls;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Storage;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using System.Xml.Linq;
using Windows.Storage;


namespace sdkRouteMapperWP8CS
{
    // Displays a route from a .GPX file that originates from a file association or SD card.
    public partial class RoutePage : PhoneApplicationPage 
    {
        // The token for the route from a file association.
        private string _fileToken;
        
        // The path to the route on the SD card, passed with a query string parameter from MainPage.xaml.cs.
        private string _sdFilePath;

        // The name of the folder on the SD card where the routes are saved.
        private const string ROUTES_FOLDER_NAME = "Routes";

        // The individual coordinates in the route.
        public GeoCoordinateCollection RouteLocations { get; private set; }

        // The area that is shown in the map control.
        public Microsoft.Phone.Maps.Controls.LocationRectangle MapRectangle { get; private set; }

        public RoutePage()
        {
            InitializeComponent();

            // Initialize route collection.
            RouteLocations = new GeoCoordinateCollection();
        }

        // Assign the path or token value, depending on how the page was launched.
        protected override async void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            // Route is from a file association.
            if (NavigationContext.QueryString.ContainsKey("fileToken"))
            {
                _fileToken = NavigationContext.QueryString["fileToken"];
                await ProcessExternalGPXFile(_fileToken);
            }
            // Route is from the SD card.
            else if (NavigationContext.QueryString.ContainsKey("sdFilePath"))
            {
                _sdFilePath = NavigationContext.QueryString["sdFilePath"];
                await ProcessSDGPXFile(_sdFilePath);
            }
        }

        // Process a route from the SD card.
        private async Task ProcessSDGPXFile(string _sdFilePath)
        {
            // Connect to the current SD card.
            ExternalStorageDevice sdCard = (await ExternalStorage.GetExternalStorageDevicesAsync()).FirstOrDefault();

            // If the SD card is present, get the route from the SD card.
            if (sdCard != null)
            {
                try
                {
                    // Get the route (.GPX file) from the SD card.
                    ExternalStorageFile file = await sdCard.GetFileAsync(_sdFilePath);

                    // Create a stream for the route.
                    Stream s = await file.OpenForReadAsync();

                    // Read the route data.
                    ReadGPXFile(s);
                }
                catch (FileNotFoundException)
                {
                    // The route is not present on the SD card.
                    MessageBox.Show("That route is missing on your SD card.");
                }
            }
            else
            {
                // No SD card is present.
                MessageBox.Show("The SD card is mssing. Insert an SD card that has a Routes folder containing at least one .GPX file and try again.");
            }
        }

        // Process a route from a file association.
        public async Task ProcessExternalGPXFile(string fileToken)
        {
            // Create or open the routes folder.
            IStorageFolder routesFolder = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFolderAsync(ROUTES_FOLDER_NAME, CreationCollisionOption.OpenIfExists);

            // Get the full file name of the route (.GPX file) from the file association.
            string incomingRouteFilename = Windows.Phone.Storage.SharedAccess.SharedStorageAccessManager.GetSharedFileName(fileToken);

            // Copy the route (.GPX file) to the Routes folder.
            IStorageFile routeFile = await Windows.Phone.Storage.SharedAccess.SharedStorageAccessManager.CopySharedFileAsync((StorageFolder)routesFolder, incomingRouteFilename, NameCollisionOption.GenerateUniqueName, fileToken);

            // Create a stream for the route.
            var routeStream = await routeFile.OpenReadAsync();

            // Read the route data.
            ReadGPXFile(routeStream.AsStream());
        }

        // Read the route data from a stream.
        public void ReadGPXFile(System.IO.Stream routeStream)
        {
            // Load all of the route data.
            XElement allData = XElement.Load(routeStream);

            XNamespace xn = allData.GetDefaultNamespace();

            // Look for route <rte> and track <trk> elements in the route data.
            var routesAndTracks = (from e in allData.DescendantsAndSelf()
                                   select new { RouteElements = e.Descendants(xn + "rte"), TrackElements = e.Descendants(xn + "trk") }).FirstOrDefault();

            // Create a list of map points from the route <rte> element, otherwise use the track <trk> element.
            List<XElement> mapPoints;
            if (routesAndTracks.RouteElements.Count() > 0)
            {
                mapPoints = (from p in routesAndTracks.RouteElements.First().Descendants(xn + "rtept")
                             select p).ToList();
            }
            else if (routesAndTracks.TrackElements.Count() > 0)
            {
                mapPoints = (from p in routesAndTracks.TrackElements.First().Descendants(xn + "trkpt")
                             select p).ToList();
            }
            else
            {
                // Route data contains no route <rte> or track <trk> elements.
                MessageBox.Show("The GPX file contains no route data; missing the <rte> or <trk> element.");
                return;
            }

            // Convert the GPX map points to coordinates that can be mapped.
            for (int i = 0; i < mapPoints.Count(); i++)
            {
                XElement xe = mapPoints[i];

                string sLat = xe.Attribute("lat").Value;
                string sLon = xe.Attribute("lon").Value;

                double dLat = double.Parse(sLat);
                double dLon = double.Parse(sLon);

                RouteLocations.Add(new GeoCoordinate(dLat, dLon));
            }

            // Set the map rectangle size based on the most distant mapPoints
            MapRectangle = new Microsoft.Phone.Maps.Controls.LocationRectangle(
                                RouteLocations.Max((p) => p.Latitude),
                                RouteLocations.Min((p) => p.Longitude),
                                RouteLocations.Min((p) => p.Latitude),
                                RouteLocations.Max((p) => p.Longitude));

            // Define the line that will appear on the map.
            MapPolyline line = new MapPolyline();
            line.StrokeThickness = 3;
            line.Path = RouteLocations;

            // Add a layer to display map background.
            MapLayer ml = new MapLayer();
            RouteMap.Layers.Add(ml);

            // Add the line to the map layer.
            RouteMap.MapElements.Add(line);

            // Set the view to the map rectangle.
            RouteMap.SetView(MapRectangle);
        }

    }
}
