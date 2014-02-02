// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

namespace EventBuddy.DataModel
{
    using System.ComponentModel;

    public class User : INotifyPropertyChanged
    {
        private static User user;
        private string userId;

        private User()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public static User Current
        {
            get
            {
                if (user == null)
                {
                    user = new User();
                }

                return user;
            }
        }

        public string UserId
        {
            get
            {
                return this.userId;
            }

            set 
            { 
                this.userId = value;
                this.OnPropertyChanged("UserId");
            }
        }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
