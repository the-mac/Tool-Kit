using System;

namespace ContosoMobile
{
    /// <summary>
    /// Args class to send messages to the UI after submission
    /// </summary>
    public class NotificationMessageArgs : EventArgs
    {
        public NotificationMessageArgs(NotificationMessage notificationMessage)
        {
            this.NotificationMessage = notificationMessage;
        }

        public NotificationMessage NotificationMessage
        {
            get;
            private set;
        }
    }
}