// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

namespace EventBuddy.WindowsPhone
{
    using System;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using EventBuddy.WindowsPhone.Dialogs;
    using EventBuddy.WindowsPhone.Model;
    using Microsoft.Phone.Controls;
    using Microsoft.WindowsAzure.MobileServices;

    public partial class EventsPage : PhoneApplicationPage
    {
        private static Popup popup = new Popup();
        private readonly ObservableCollection<Event> events = new ObservableCollection<Event>();
        private bool showError = false;

        public EventsPage()
        {
            this.InitializeComponent();

            this.DataContext = this;
        }

        public ObservableCollection<Event> Events
        {
            get { return this.events; }
        }

        private void EventsPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (App.MobileService.CurrentUser == null && !this.showError)
            {
                this.ShowIdentityProviders();
            }
            else if (!this.showError)
            {
                this.LoadData();
            }
        }

        private async Task TwitterLogin()
        {
            try
            {
                await App.MobileService.LoginAsync(MobileServiceAuthenticationProvider.Twitter);
                this.LoadData();
            }
            catch (Exception)
            {
                this.ShowConnectionError();
            }
        }

        private void ShowConnectionError()
        {
            this.showError = true;
            popup = new Popup();
            popup.Height = Application.Current.Host.Content.ActualHeight;
            popup.Width = Application.Current.Host.Content.ActualWidth;
            popup.VerticalOffset = 30;
            ErrorDialog dialog = new ErrorDialog();
            dialog.Height = Application.Current.Host.Content.ActualHeight;
            dialog.Width = Application.Current.Host.Content.ActualWidth;
            popup.Child = dialog;
            popup.IsOpen = true;
            dialog.ErrorMessage.Text =
                "Oops! An error ocurred when connecting. " +
                "Make sure the configuration is correct and " +
                "retry again in a couple of seconds.";

            dialog.RetryButton.Click += (s, args) =>
            {
                popup.IsOpen = false;
                showError = false;
                ShowIdentityProviders();
            };

            dialog.CloseButton.Click += (s, args) =>
            {
                popup.IsOpen = false;
                Application.Current.Terminate();
            };
        }

        private async Task FacebookLogin()
        {
            try
            {
                await App.MobileService.LoginAsync(MobileServiceAuthenticationProvider.Facebook);
                this.LoadData();
            }
            catch (Exception)
            {
                this.ShowConnectionError();
            }
        }

        private async void LoadData()
        {
            this.LoadingProgress.IsIndeterminate = true;
            var events = await App.MobileService.GetTable<Event>().ToEnumerableAsync();
            this.Events.Clear();
            foreach (var item in events)
            {
                this.Events.Add(item);
            }

            this.LoadingProgress.IsIndeterminate = false;
        }

        private void ListBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            ListBox lb = sender as ListBox;
            if (lb != null)
            {
                Event selected = lb.SelectedItem as Event;
                if (selected != null)
                {
                    string uri = string.Format("/SessionsPage.xaml?id={0}&name={1}", selected.Id, selected.Name);

                    this.NavigationService.Navigate(new Uri(uri, UriKind.Relative));
                }
            }
        }

        private void ShowIdentityProviders()
        {
            popup.Height = Application.Current.Host.Content.ActualHeight;
            popup.Width = Application.Current.Host.Content.ActualWidth;
            popup.VerticalOffset = 30;
            IdentityDialog dialog = new IdentityDialog();
            dialog.Height = Application.Current.Host.Content.ActualHeight;
            dialog.Width = Application.Current.Host.Content.ActualWidth;
            popup.Child = dialog;
            popup.IsOpen = true;

            dialog.FacebookButton.Click += this.FacebookLogin_Click;
            dialog.TwitterButton.Click += this.TwitterLogin_Click;
        }

        private async void TwitterLogin_Click(object sender, RoutedEventArgs e)
        {
            popup.IsOpen = false;
            await this.TwitterLogin();
        }

        private async void FacebookLogin_Click(object sender, RoutedEventArgs e)
        {
            popup.IsOpen = false;
            await this.FacebookLogin();
        }

        private void RefreshAppBarButton_Click(object sender, EventArgs e)
        {
            this.LoadData();
        }

        private void LoginAppBarButton_Click(object sender, EventArgs e)
        {
            this.ShowIdentityProviders();
        }

        private void AddAppBarButton_Click(object sender, EventArgs e)
        {
            this.ShowNotImplementedMessage();
        }

        private void ShowNotImplementedMessage()
        {
            MessageBox.Show("This feature is not implemented.");
        }
    }
}