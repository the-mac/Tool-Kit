// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

namespace EventBuddy.WindowsPhone
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using EventBuddy.WindowsPhone.Helpers;
    using EventBuddy.WindowsPhone.Model;
    using Microsoft.Phone.Controls;
    using Microsoft.Phone.Shell;

    public partial class SessionDetail : PhoneApplicationPage, INotifyPropertyChanged
    {
        private Session session;

        public SessionDetail()
        {
            this.InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Session Session
        {
            get
            {
                return this.session;
            }

            set
            {
                this.SetProperty(ref this.session, value);
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Session = PhoneApplicationService.Current.State["Session"] as Session;
            this.DocumentsPanel.Visibility = string.IsNullOrEmpty(Session.DeckSource) ? Visibility.Collapsed : Visibility.Visible;
            this.DataContext = Session;
        }

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (object.Equals(storage, value))
            {
                return false;
            }

            storage = value;
            this.OnPropertyChanged(propertyName);
            return true;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var eventHandler = this.PropertyChanged;
            if (eventHandler != null)
            {
                eventHandler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void OnStarTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            int score = 0;
            Image starSender = (Image)sender;
            var selectedImage = new BitmapImage(new Uri("Assets/SelectedStar.png", UriKind.RelativeOrAbsolute));
            var unselectedImage = new BitmapImage(new Uri("Assets/UnselectedStar.png", UriKind.RelativeOrAbsolute));

            switch (starSender.Name)
            {
                case "Star1":
                    Star1.Source = selectedImage;
                    Star2.Source = Star3.Source = Star4.Source = Star5.Source = unselectedImage;
                    score = 1;
                    break;
                case "Star2":
                    Star1.Source = Star2.Source = selectedImage;
                    Star3.Source = Star4.Source = Star5.Source = unselectedImage;
                    score = 2;
                    break;
                case "Star3":
                    Star1.Source = Star2.Source = Star3.Source = selectedImage;
                    Star4.Source = Star5.Source = unselectedImage;
                    score = 3;
                    break;
                case "Star4":
                    Star1.Source = Star2.Source = Star3.Source = Star4.Source = selectedImage;
                    Star5.Source = unselectedImage;
                    score = 4;
                    break;
                case "Star5":
                    Star1.Source = Star2.Source = Star3.Source = Star4.Source = Star5.Source = selectedImage;
                    score = 5;
                    break;
            }

            this.SendRating(score);
        }

        private void SendRating(int score)
        {
            var provider = App.MobileService.CurrentUser.UserId.Split(':')[0];
            if (provider.Equals("twitter", StringComparison.OrdinalIgnoreCase))
            {
                this.SendRatingTwitter(score);
            }
            else if (provider.Equals("facebook", StringComparison.OrdinalIgnoreCase))
            {
                this.SendRatingFacebook(score);
            }
        }

        private void SendRatingTwitter(int score)
        {
            var twitterId = App.MobileService.CurrentUser.UserId.Split(':')[1];
            TwitterHelper.RetrieveUserInformation(
                twitterId,
                async tu =>
                {
                    Rating rating = new Rating()
                    {
                        SessionId = Session.Id,
                        Score = (float)score,
                        RaterName = tu.Handle,
                        ImageUrl = tu.PictureUrl
                    };

                    await App.MobileService.GetTable<Rating>().InsertAsync(rating);
                },
                async () =>
                {
                    Rating rating = new Rating()
                    {
                        SessionId = Session.Id,
                        Score = (float)score,
                        RaterName = "Someone",
                        ImageUrl = "Assets/NoProfile.png"
                    };

                    await App.MobileService.GetTable<Rating>().InsertAsync(rating);
                });
        }

        private void SendRatingFacebook(int score)
        {
            var facebookId = App.MobileService.CurrentUser.UserId.Split(':')[1];
            FacebookHelper.RetrieveUserInformation(
                facebookId,
                async fu =>
                {
                    Rating rating = new Rating()
                    {
                        SessionId = Session.Id,
                        Score = (float)score,
                        RaterName = fu.FullName,
                        ImageUrl = fu.PictureUrl
                    };

                    await App.MobileService.GetTable<Rating>().InsertAsync(rating);
                },
                async () =>
                {
                    Rating rating = new Rating()
                    {
                        SessionId = Session.Id,
                        Score = (float)score,
                        RaterName = "Someone",
                        ImageUrl = "Assets/NoProfile.png"
                    };

                    await App.MobileService.GetTable<Rating>().InsertAsync(rating);
                });
        }

        private void OnTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Microsoft.Phone.Tasks.WebBrowserTask task = new Microsoft.Phone.Tasks.WebBrowserTask();
            task.Uri = new Uri(Session.DeckSource);
            task.Show();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Microsoft.Phone.Tasks.WebBrowserTask task = new Microsoft.Phone.Tasks.WebBrowserTask();
            task.Uri = new Uri(Session.DeckSource);
            task.Show();
        }

        private void EditAppBarButton_Click(object sender, EventArgs e)
        {
            this.ShowNotImplementedMessage();
        }

        private void ShowNotImplementedMessage()
        {
            MessageBox.Show("This feature is not implemented.");
        }
    }
}