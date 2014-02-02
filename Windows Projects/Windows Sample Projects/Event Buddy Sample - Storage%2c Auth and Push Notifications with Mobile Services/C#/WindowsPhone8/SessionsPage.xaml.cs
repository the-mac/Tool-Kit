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
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Controls;
    using EventBuddy.WindowsPhone.Model;
    using Microsoft.Phone.Controls;
    using Microsoft.Phone.Shell;

    public partial class SessionsPage : PhoneApplicationPage, INotifyPropertyChanged
    {
        private readonly ObservableCollection<Session> sessions = new ObservableCollection<Session>();

        private string eventName;

        public SessionsPage()
        {
            this.InitializeComponent();

            this.DataContext = this;
            this.Loaded += this.Sessions_Loaded;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Session> EventSessions
        {
            get { return this.sessions; }
        }

        public string EventName
        {
            get { return this.eventName; }
            set { this.SetProperty(ref this.eventName, value); }
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var eventHandler = this.PropertyChanged;
            if (eventHandler != null)
            {
                eventHandler(this, new PropertyChangedEventArgs(propertyName));
            }
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

        private void Sessions_Loaded(object sender, RoutedEventArgs e)
        {
            this.LoadData();
        }

        private async void LoadData()
        {
            this.LoadingProgress.IsIndeterminate = true;
            var eventId = this.NavigationContext.QueryString["id"];
            this.EventName = this.NavigationContext.QueryString["name"];
            var sessions = await App.MobileService.GetTable<Session>().Where(s => s.EventId == eventId).ToEnumerableAsync();
            this.EventSessions.Clear();
            foreach (var item in sessions)
            {
                this.EventSessions.Add(item);
            }

            this.LoadingProgress.IsIndeterminate = false;
        }
   
        private void OnGridTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
        }

        private void OnListboxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Session session = null;

            if (e.AddedItems.Count > 0)
            { 
                session = e.AddedItems[0] as Session;
            }

            if (session != null)
            {
                PhoneApplicationService.Current.State["Session"] = session;
                string uri = string.Format("/SessionDetail.xaml?id={0}", session.Id);
                this.NavigationService.Navigate(new Uri(uri, UriKind.Relative));
            }
        }

        private void RefreshAppBarButton_Click(object sender, EventArgs e)
        {
            this.LoadData();
        }

        private void AddAppBarButton_Click(object sender, EventArgs e)
        {
            this.ShowNotImplementedMessage();
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