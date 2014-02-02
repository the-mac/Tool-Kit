using System;

namespace ContosoMobile
{
    /// <summary>
    /// Arguments to be passed in the Notification message shown to the user
    /// </summary>
    public class NotificationMessage
    {
        public NotificationMessage(string message, bool isSuccess)
        {
            this.Message = message;
            this.IsSuccess = isSuccess;
        }

        public string Message
        {
            get;
            set;
        }

        public bool IsSuccess
        {
            get;
            set;
        }
    }
}