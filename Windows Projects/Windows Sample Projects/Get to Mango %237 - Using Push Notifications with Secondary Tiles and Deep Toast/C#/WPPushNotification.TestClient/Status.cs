using System;
using System.ComponentModel;

namespace WPPushNotification.TestClient
{
    /// <summary>
    /// Represents a status message.
    /// </summary>
    public class Status : INotifyPropertyChanged
    {
        private string message;

        /// <summary>
        /// A message representing some status.
        /// </summary>
        public string Message
        {
            get
            {
                return message;
            }
            set
            {
                if (value != message)
                {
                    message = value;

                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("Message"));
                    }
                }
            }
        }


        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
