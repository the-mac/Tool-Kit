using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WPPushNotification.ServerSideWeatherSimulator.Service
{
    public class RegistrationService : IRegistrationService
    {
        public static event EventHandler<SubscriptionEventArgs> Subscribed;
        public static event EventHandler<DataRequestEventArgs> DataRequested;

        private static List<Uri> subscribers = new List<Uri>();
        private static object obj = new object();

        public void Register(string uri)
        {
            Uri channelUri = new Uri(uri, UriKind.Absolute);
            Subscribe(channelUri);
        }

        public void Unregister(string uri)
        {
            Uri channelUri = new Uri(uri, UriKind.Absolute);
            Unsubscribe(channelUri);
        }

        public void RequestData(string locationName, string uri)
        {
            Uri channelUri = new Uri(uri, UriKind.Absolute);
            SignalDataRequested(locationName, channelUri);
        }

        #region Subscription/Unsubscribing logic
        private void Subscribe(Uri channelUri)
        {
            lock (obj)
            {
                if (!subscribers.Exists((u) => u == channelUri))
                {
                    subscribers.Add(channelUri);
                }
            }
            OnSubscribed(channelUri, true);
        }

        public static void Unsubscribe(Uri channelUri)
        {
            lock (obj)
            {
                subscribers.Remove(channelUri);
            }
            OnSubscribed(channelUri, false);
        }
        #endregion

        #region Data request logic
        public static void SignalDataRequested(string locationName, Uri channelUri)
        {
            lock (obj)
            {
                if (subscribers.Exists((u) => u == channelUri))
                {
                    if (DataRequested != null)
                    {
                        DataRequested(null, new DataRequestEventArgs(locationName, channelUri));
                    }
                }
            }            
        }
        #endregion

        #region Helper private functionality
        private static void OnSubscribed(Uri channelUri, bool isActive)
        {
            EventHandler<SubscriptionEventArgs> handler = Subscribed;
            if (handler != null)
            {
                handler(null,
                  new SubscriptionEventArgs(channelUri, isActive));
            }
        }
        #endregion

        #region Internal SubscriptionEventArgs class definition
        public class SubscriptionEventArgs : EventArgs
        {
            public SubscriptionEventArgs(Uri channelUri, bool isActive)
            {
                this.ChannelUri = channelUri;
                this.IsActive = isActive;
            }

            public Uri ChannelUri { get; private set; }
            public bool IsActive { get; private set; }
        }
        #endregion

        #region Internal DataRequestEventArgs class definition
        public class DataRequestEventArgs : EventArgs
        {
            public DataRequestEventArgs(string locationName, Uri channelUri)
            {
                this.ChannelUri = channelUri;
                this.LocationName = locationName;
            }

            public Uri ChannelUri { get; private set; }
            public string LocationName { get; private set; }
        }
        #endregion

        #region Helper public functionality
        public static List<Uri> GetSubscribers()
        {
            return subscribers;
        }
        #endregion
    }
}
