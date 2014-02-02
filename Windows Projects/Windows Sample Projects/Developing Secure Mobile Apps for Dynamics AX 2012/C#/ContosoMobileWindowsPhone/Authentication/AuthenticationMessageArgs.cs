using System;

namespace ContosoMobile.Authentication
{
    /// <summary>
    /// Event arguments required for the AuthenticationCompleted event.
    /// </summary>
    public class AuthenticationMessageArgs : EventArgs
    {
        public AuthenticationMessageArgs(AuthenticationMessage authenticationMessage)
        {
            this.AuthenticationMessage = authenticationMessage;
        }

        public AuthenticationMessage AuthenticationMessage
        {
            get;
            private set;
        }
    }
}
