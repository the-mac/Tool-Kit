// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

namespace EventBuddy
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using EventBuddy.Common;
    using EventBuddy.DataModel;
    using EventBuddy.Helpers;
    using Microsoft.WindowsAzure.MobileServices;
    using Windows.UI.Popups;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Navigation;

    public sealed partial class EventsPage : Page
    {
        #region

        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private NavigationHelper navigationHelper;
        private ObservableCollection<Event> events = new ObservableCollection<Event>();
        private MobileServiceClient privateClient = null;

        public EventsPage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;

            User.Current.PropertyChanged += this.OnUserPropertyChanged;
            this.SetLoggedUser();

            eventEditor.DataContext = new EventEditorViewModel();
        }

        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        public dynamic Events
        {
            get
            {
                return this.events;
            }

            set
            {
                this.events.Clear();
                foreach (var item in value)
                {
                    this.events.Add(item);
                }
            }
        }

        protected async override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        #endregion

        private async Task SaveEvent(Event item)
        {
            //// TODO: save the new event

            this.Events.Add(item);
        }

        private async Task LoadEvents()
        {
            //// TODO: query for existing events
        }

        #region

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            await this.LoadEvents();
            this.defaultViewModel["Events"] = this.Events;
            this.UpdateVisibility();
        }

        private void OnUserPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "UserId")
            {
                this.SetLoggedUser();
            }
        }

        private void SetLoggedUser()
        {
            this.loggedUser.Text = LoginMessageHelper.GetLoginMessage();
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

        private void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (this.IsUsingMobileAuth() && !(this.GetPrivateClient() != null && this.GetPrivateClient().CurrentUser != null))
            {
                this.ShowMessage();
                return;
            }

            var eventItem = (Event)e.ClickedItem;
            this.Frame.Navigate(typeof(SessionsPage), eventItem);
        }

        private bool IsUsingMobileAuth()
        {
            return this.GetPrivateClient() != null;
        }

        private async void ShowMessage()
        {
            // Create the message dialog and set its content
            var messageDialog = new MessageDialog("You must log in in order to view an Event");

            messageDialog.Commands.Add(new UICommand("Close"));

            // Set the command that will be invoked by default
            messageDialog.DefaultCommandIndex = 0;

            // Set the command to be invoked when escape is pressed
            messageDialog.CancelCommandIndex = 1;

            // Show the message dialog
            await messageDialog.ShowAsync();
        }

        private async void BtnSaveEvent(object sender, EventBuddy.EventEditor.SaveEditorEventArgs args)
        {
            var item = this.eventEditor.DataContext as EventEditorViewModel;
            await this.SaveEvent(item.Event);
            this.UpdateVisibility();
            this.eventEditor.Hide();
        }

        private async void BtnEditEvent(object sender, RoutedEventArgs e)
        {
            Utils.ShowNotImplementedMessage();
        }

        private void UpdateVisibility()
        {
            loadingEventsIndicator.Visibility = Visibility.Collapsed;

            if (this.Events != null && this.Events.Count == 0)
            {
                noRecordsView.Visibility = Visibility.Visible;
                itemGridView.Visibility = Visibility.Collapsed;
            }
            else
            {
                noRecordsView.Visibility = Visibility.Collapsed;
                itemGridView.Visibility = Visibility.Visible;
            }
        }

        private void BtnAddEvent(object sender, RoutedEventArgs e)
        {
            this.eventEditor.DataContext = new EventEditorViewModel();
            this.eventEditor.Show();
        }

        private async void OnLoggedIn(object sender, EventArgs e)
        {
            await this.LoadEvents();
            this.itemGridView.ItemsSource = this.events;
            this.UpdateVisibility();
        }

        private void OnAddEventTapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            this.BtnAddEvent(sender, e);
        }

        #endregion
    }
}
