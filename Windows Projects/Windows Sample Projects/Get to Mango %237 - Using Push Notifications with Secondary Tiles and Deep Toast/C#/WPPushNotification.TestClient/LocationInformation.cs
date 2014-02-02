using System;
using System.ComponentModel;

namespace WPPushNotification.TestClient
{
    /// <summary>
    /// Information about a location monitored by the client.
    /// </summary>
    public class LocationInformation : INotifyPropertyChanged
    {
        private bool tilePinned;
        private string name;
        private string temperature;
        private string imageName;

        /// <summary>
        /// Whether or not the location's secondary tile has been 
        /// pinned by the user.
        /// </summary>
        public bool TilePinned
        {
            get
            {
                return tilePinned;
            }
            set
            {
                if (value != tilePinned)
                {
                    tilePinned = value;

                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("TilePinned"));
                    }
                }
            }
        }

        /// <summary>
        /// The location's name.
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (value != name)
                {
                    name = value;

                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("Name"));
                    }
                }
            }
        }

        /// <summary>
        /// The temperature at the location.
        /// </summary>
        public string Temperature
        {
            get
            {
                return temperature;
            }
            set
            {
                if (value != temperature)
                {
                    temperature = value;

                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("Temperature"));
                    }
                }
            }
        }

        /// <summary>
        /// The name of the image to use for representing the weather
        /// at the location.
        /// </summary>
        public string ImageName
        {
            get
            {
                return imageName;
            }
            set
            {
                if (value != imageName)
                {
                    imageName = value;

                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("ImageName"));
                    }
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
