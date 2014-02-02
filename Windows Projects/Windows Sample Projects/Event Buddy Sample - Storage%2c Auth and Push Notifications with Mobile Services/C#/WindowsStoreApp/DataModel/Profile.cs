// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

namespace EventBuddy.DataModel
{
    using EventBuddy.Common;

    public class Profile : BindableBase
    {
        private string firstName;
        private string lastName;
        private string profileUri;

        public string FirstName
        {
            get { return this.firstName; }
            set { this.SetProperty(ref this.firstName, value); }
        }

        public string LastName
        {
            get { return this.lastName; }
            set { this.SetProperty(ref this.lastName, value); }
        }

        public string ProfileUri
        {
            get { return this.profileUri; }
            set { this.SetProperty(ref this.profileUri, value); }
        }
    }
}
