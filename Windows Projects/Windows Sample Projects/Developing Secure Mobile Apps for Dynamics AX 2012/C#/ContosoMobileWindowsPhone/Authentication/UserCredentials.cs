using System;

namespace ContosoMobile.Authentication
{
    /// <summary>
    /// The user's alias and password that will be used to authenticate the user
    /// </summary>
    public class UserCredentials
    {
        /// <summary>
        /// User's alias: domain\username
        /// </summary>
        public string Alias
        {
            get;
            private set;
        }

        /// <summary>
        /// User's password
        /// </summary>
        public string Password
        {
            get;
            private set;
        }

        public UserCredentials(string userAlias, string password)
        {
            Alias = userAlias;
            Password = password;
        }
    }
}