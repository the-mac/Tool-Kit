using System;
using Microsoft.Phone.Notification;
using System.Collections.Generic;
using System.Windows.Threading;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml.Linq;
using System.Net;

namespace WPPushNotification.TestClient
{
    /// <summary>
    /// Handles the various aspects of managing push notifications for the application.
    /// </summary>
    public class PushHandler
    {
        private object dataRequestSync = new object();

        private HttpNotificationChannel httpChannel;
        const string channelName = "WeatherUpdatesChannel";

        private bool connectedToMSPN;
        private bool connectedToServer;
        private bool notificationsBound;

        // Stores locations for which data is to be requested once connection to the MSPN is established.
        private Stack<string> dataRequestLocations = new Stack<string>();

        /// <summary>
        /// Contains information about the locations displayed by the application.
        /// </summary>
        public Dictionary<string, LocationInformation> Locations { get; private set; }

        /// <summary>
        /// A dispatcher used to interact with the UI.
        /// </summary>
        public Dispatcher Dispatcher { get; private set; }

        /// <summary>
        /// Push service related status information.
        /// </summary>
        public Status PushStatus { get; private set; }

        /// <summary>
        /// Whether or not the handler has fully established a connection to both the MSPN and the application server.
        /// </summary>
        public bool ConnectionEstablished
        {
            get
            {
                return connectedToMSPN && connectedToServer && notificationsBound;
            }
        }

        /// <summary>
        /// Creates a new instance of the class.
        /// </summary>
        /// <param name="pushStatus">Status object to update with push related status messages.</param>
        /// <param name="locationsInformation">Dictionary containing information about the locations tracked by the 
        /// application</param>
        /// <param name="uiDispatcher">A dispatcher to use when updating the user interface.</param>
        public PushHandler (Status pushStatus, Dictionary<string, LocationInformation> locationsInformation,
            Dispatcher uiDispatcher)
        {
            PushStatus = pushStatus;
            Locations = locationsInformation;
            Dispatcher = uiDispatcher;
        }

        #region Push service methods
        /// <summary>
        /// Connects to the Microsoft Push Service and registers the received channel with the application server.
        /// </summary>
        public void EstablishConnections()
        {
            connectedToMSPN = false;
            connectedToServer = false;
            notificationsBound = false;

            try
            {
                //First, try to pick up existing channel
                httpChannel = HttpNotificationChannel.Find(channelName);

                if (null != httpChannel)
                {
                    connectedToMSPN = true;

                    ClearDataRequests();

                    App.Trace("Channel Exists - no need to create a new one");
                    SubscribeToChannelEvents();

                    App.Trace("Register the URI with 3rd party web service");
                    SubscribeToService();

                    App.Trace("Subscribe to the channel to Tile and Toast notifications");
                    SubscribeToNotifications();

                    UpdateStatus("Channel recovered");
                }
                else
                {
                    App.Trace("Trying to create a new channel...");
                    //Create the channel
                    httpChannel = new HttpNotificationChannel(channelName, "HOLWeatherService");
                    App.Trace("New Push Notification channel created successfully");

                    SubscribeToChannelEvents();

                    App.Trace("Trying to open the channel");
                    httpChannel.Open();
                    UpdateStatus("Channel open requested");
                }
            }
            catch (Exception ex)
            {
                UpdateStatus("Channel error: " + ex.Message);
            }
        }

        /// <summary>
        /// Asks the application server to send a raw push notification for the latest data available 
        /// for a specific location.
        /// </summary>
        /// <param name="locationName">The name of the location for which the data is requested.</param>
        /// <remarks>If a connection to the MSPN has not been established, requests will be queued
        /// until connection is established.</remarks>
        public void RequestLatestData(string locationName)
        {
            lock (dataRequestSync)
            {
                if (!connectedToMSPN)
                {
                    dataRequestLocations.Push(locationName);
                }
                else
                {
                    // In case some requests were not cleared out due to connection timing issues,
                    // we clear the pending request stack.                    
                    ClearDataRequests();
                }
            }

            //Hardcode for solution - needs to be updated in case the REST WCF service address change
            string baseUri = "http://localhost:8000/RegirstatorService/RequestData?locationName={0}&uri={1}";
            string theUri = String.Format(baseUri, locationName, httpChannel.ChannelUri.ToString());
            WebClient client = new WebClient();
            client.DownloadStringCompleted += (s, e) =>
            {
                if (null == e.Error)
                {
                    UpdateStatus("Data requested");
                }
                else
                {
                    UpdateStatus("Error requesting data: " + e.Error.Message);
                }
            };

            client.DownloadStringAsync(new Uri(theUri));
        }        
        #endregion

        #region Subscriptions
        private void SubscribeToChannelEvents()
        {
            //Register to UriUpdated event - occurs when channel successfully opens
            httpChannel.ChannelUriUpdated += new EventHandler<NotificationChannelUriEventArgs>(httpChannel_ChannelUriUpdated);

            //Subscribed to Raw Notification
            httpChannel.HttpNotificationReceived += new EventHandler<HttpNotificationEventArgs>(httpChannel_HttpNotificationReceived);

            //general error handling for push channel
            httpChannel.ErrorOccurred += new EventHandler<NotificationChannelErrorEventArgs>(httpChannel_ExceptionOccurred);

            //subscribe to toast notification when running app    
            httpChannel.ShellToastNotificationReceived += new EventHandler<NotificationEventArgs>(httpChannel_ShellToastNotificationReceived);
        }

        private void SubscribeToService()
        {
            //Hardcode for solution - needs to be updated in case the REST WCF service address change
            string baseUri = "http://localhost:8000/RegirstatorService/Register?uri={0}";
            string theUri = String.Format(baseUri, httpChannel.ChannelUri.ToString());
            WebClient client = new WebClient();
            client.DownloadStringCompleted += (s, e) =>
            {
                if (null == e.Error)
                {
                    connectedToServer = true;
                    UpdateStatus("Registration succeeded");
                }
                else
                {
                    UpdateStatus("Registration failed: " + e.Error.Message);
                }
            };

            client.DownloadStringAsync(new Uri(theUri));
        }

        private void SubscribeToNotifications()
        {
            //////////////////////////////////////////
            // Bind to Toast Notification 
            //////////////////////////////////////////
            try
            {
                if (httpChannel.IsShellToastBound == true)
                {
                    App.Trace("Already bound to Toast notification");
                }
                else
                {
                    App.Trace("Registering to Toast Notifications");
                    httpChannel.BindToShellToast();
                }
            }
            catch (Exception ex)
            {
                // handle error here
            }

            //////////////////////////////////////////
            // Bind to Tile Notification 
            //////////////////////////////////////////
            try
            {
                if (httpChannel.IsShellTileBound == true)
                {
                    App.Trace("Already bound to Tile Notifications");
                }
                else
                {
                    App.Trace("Registering to Tile Notifications");

                    // you can register the phone application to receive tile images from remote servers [this is optional]
                    Collection<Uri> uris = new Collection<Uri>();
                    uris.Add(new Uri("http://www.larvalabs.com"));

                    httpChannel.BindToShellTile(uris);
                }
            }
            catch (Exception ex)
            {
                //handle error here
            }

            notificationsBound = true;
        }

        #endregion

        #region Channel event handlers
        void httpChannel_ChannelUriUpdated(object sender, NotificationChannelUriEventArgs e)
        {
            connectedToMSPN = true;

            ClearDataRequests();

            App.Trace("Channel opened. Got Uri:\n" + httpChannel.ChannelUri.ToString());
            App.Trace("Subscribing to channel events");
            SubscribeToService();
            SubscribeToNotifications();

            UpdateStatus("Channel created successfully");
        }

        void httpChannel_ExceptionOccurred(object sender, NotificationChannelErrorEventArgs e)
        {
            UpdateStatus(e.ErrorType + " occurred: " + e.Message);
        }

        void httpChannel_HttpNotificationReceived(object sender, HttpNotificationEventArgs e)
        {
            App.Trace("===============================================");
            App.Trace("RAW notification arrived");

            Dispatcher.BeginInvoke(() => ParseRAWPayload(e.Notification.Body));

            App.Trace("===============================================");
        }

        void httpChannel_ShellToastNotificationReceived(object sender, NotificationEventArgs e)
        {
            App.Trace("===============================================");
            App.Trace("Toast/Tile notification arrived:");

            string msg = e.Collection["wp:Text2"];

            App.Trace(msg);
            UpdateStatus("Toast/Tile message: " + msg);

            App.Trace("===============================================");
        }
        #endregion

        #region Helper methods
        private void UpdateStatus(string message)
        {
            Dispatcher.BeginInvoke(() => PushStatus.Message = message);
        }

        private void ClearDataRequests()
        {            
            while (dataRequestLocations.Count > 0)
            {
                RequestLatestData(dataRequestLocations.Pop());
            }
        }

        private void ParseRAWPayload(Stream e)
        {
            XDocument document;

            using (var reader = new StreamReader(e))
            {
                string payload = reader.ReadToEnd().Replace('\0', ' ');
                document = XDocument.Parse(payload);
            }

            XElement updateElement = document.Root;

            string locationName = updateElement.Element("Location").Value;
            LocationInformation locationInfo = Locations[locationName];
            App.Trace("Got location: " + locationName);

            string temperature = updateElement.Element("Temperature").Value;
            locationInfo.Temperature = temperature;
            App.Trace("Got temperature: " + temperature);

            string weather = updateElement.Element("WeatherType").Value;
            locationInfo.ImageName = weather;
            App.Trace("Got weather type: " + weather);
        }
        #endregion
    }
}
