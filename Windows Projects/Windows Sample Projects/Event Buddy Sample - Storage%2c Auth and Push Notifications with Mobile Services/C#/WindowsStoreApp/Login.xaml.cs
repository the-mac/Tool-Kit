// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

namespace EventBuddy
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using EventBuddy.DataModel;
    using EventBuddy.Helpers;
    using Microsoft.WindowsAzure.MobileServices;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Input;

    public sealed partial class Login : UserControl
    {
        #region

        private MobileServiceClient privateClient = null;

        public Login()
        {
            this.InitializeComponent();

            this.UpdateVisibility();
        }

        public event EventHandler<EventArgs> LoggedIn;

        public static Profile Profile { get; set; }

        #endregion

        public async Task LoginTwitter()
        {
            // TODO: Login using Twitter
        }

        public async Task LoginFacebook()
        {
            // TODO: Login using Facebook
        }

        #region

        private void UserControl_Tapped(object sender, TappedRoutedEventArgs e)
        {
            loginPopup.IsOpen = !loginPopup.IsOpen;
        }

        private async void OnLoginFacebook(object sender, PointerRoutedEventArgs e)
        {
            try
            {
                await this.LoginFacebook();
            }
            catch (Exception)
            {
                return;
            }

            var success = false;
            var shouldSleep = false;
            for (var iterator = 0; iterator < 3 && !success; iterator++)
            {
                try
                {
                    var facebookId = this.GetPrivateClient().CurrentUser.UserId.Split(':')[1];
                    var userInformation = await FacebookHelper.RetrieveUserInformation(facebookId);
                    User.Current.UserId = userInformation.FullName;
                    success = true;
                }
                catch (Exception)
                {
                    if (iterator == 2)
                    {
                        User.Current.UserId = this.GetPrivateClient().CurrentUser.UserId;
                    }
                    else
                    {
                        shouldSleep = true;
                    }
                }

                if (shouldSleep)
                {
                    shouldSleep = false;
                    await Task.Delay(TimeSpan.FromMilliseconds(400 * Math.Pow(2, iterator)));
                }
            }

            var login = this.LoggedIn;

            if (login != null)
            {
                login.Invoke(this, null);
            }

            this.UpdateVisibility();

            this.loginPopup.IsOpen = false;
        }

        private async void OnLoginMicrosoft(object sender, PointerRoutedEventArgs e)
        {
            Utils.ShowNotImplementedMessage();
        }

        private async void OnLoginGoogle(object sender, PointerRoutedEventArgs e)
        {
            Utils.ShowNotImplementedMessage();
        }

        private async void OnLoginTwitter(object sender, PointerRoutedEventArgs e)
        {
            try
            {
                await this.LoginTwitter();
            }
            catch (Exception)
            {
                return;
            }

            var success = false;
            var shouldSleep = false;
            for (var iterator = 0; iterator < 3 && !success; iterator++)
            {
                try
                {
                    var twitterId = this.GetPrivateClient().CurrentUser.UserId.Split(':')[1];
                    var userInformation = await TwitterHelper.RetrieveUserInformation(twitterId);
                    User.Current.UserId = userInformation.Handle;
                    success = true;
                }
                catch (Exception)
                {
                    if (iterator == 2)
                    {
                        User.Current.UserId = this.GetPrivateClient().CurrentUser.UserId;
                    }
                    else
                    {
                        shouldSleep = true;
                    }
                }

                if (shouldSleep)
                {
                    shouldSleep = false;
                    await Task.Delay(TimeSpan.FromMilliseconds(400 * Math.Pow(2, iterator)));
                }
            }

            var login = this.LoggedIn;

            if (login != null)
            {
                login.Invoke(this, null);
            }

            this.UpdateVisibility();

            this.loginPopup.IsOpen = false;
        }

        private void UpdateVisibility()
        {
            if (this.GetPrivateClient() != null && this.GetPrivateClient().CurrentUser != null)
            {
                this.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.Visibility = Visibility.Visible;
            }
        }

        private MobileServiceClient GetPrivateClient()
        {
            if (this.privateClient == null)
            {
                var field = typeof(App).GetRuntimeFields().SingleOrDefault(pi => pi.FieldType == typeof(MobileServiceClient));
                if (field == null)
                {
                    return null;
                }

                this.privateClient = field.GetValue(null) as MobileServiceClient;
            }

            return this.privateClient;
        }

        #endregion
    }
}
