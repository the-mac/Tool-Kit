using System;

namespace ContosoMobile.Authentication
{   
    /// <summary>
    /// Arguments to be passed in the Authentication message 
    /// </summary>
    public class AuthenticationMessage
    {
        public AuthenticationMessage(string message, bool isSuccess)
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