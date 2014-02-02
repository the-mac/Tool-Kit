// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

namespace EventBuddy
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using EventBuddy.Common;
    using EventBuddy.DataModel;
    using EventBuddy.Helpers;
    using Microsoft.WindowsAzure.MobileServices;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Navigation;

    public sealed partial class SessionsPage : Page
    {
        #region

        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private NavigationHelper navigationHelper;
        private ObservableCollection<Session> sessions = new ObservableCollection<Session>();
        private MobileServiceClient privateClient = null;

        public SessionsPage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;

            User.Current.PropertyChanged += this.OnUserPropertyChanged;
            this.SetLoggedUser();
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

        public dynamic Sessions
        {
            get
            {
                return this.sessions;
            }

            set
            {
                this.sessions.Clear();
                foreach (var item in value)
                {
                    this.sessions.Add(item);
                }
            }
        }

        public Event Event { get; set; }

        protected async override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        #endregion

        private async Task SaveSession(Session item)
        {
            // TODO: Save Session
            this.Sessions.Add(item);
        }

        private async Task LoadSessions(Event eventItem)
        {
            // TODO: Query Sessions for selected eventItem.Id
        }

        private async Task UpdateSession(Session item)
        {
            // TODO: Update Session
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
            this.Event = e.NavigationParameter as Event;

            await this.LoadSessions(this.Event);

            this.DefaultViewModel["Sessions"] = this.Sessions;
            this.DefaultViewModel["Event"] = this.Event;
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

        private void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.editButtonCommands.Visibility = Visibility.Visible;
            this.BottomAppBar.IsOpen = true;

            sessionEditor.DataContext = (Session)e.ClickedItem;
        }

        private void BtnAddSession(object sender, RoutedEventArgs e)
        {
            sessionEditor.DataContext = new Session(Event);
            sessionEditor.Show();
        }

        private void BtnEditSession(object sender, RoutedEventArgs e)
        {
            this.sessionEditor.Show();
        }

        private async void BtnSaveSession(object sender, EventBuddy.SessionEditor.SaveEditorEventArgs args)
        {
            var item = this.sessionEditor.DataContext as Session;

            if (item.Id == null)
            {
                await this.SaveSession(item);
            }
            else
            {
                await this.UpdateSession(item);

                await this.LoadSessions(this.Event);
            }

            this.sessionEditor.Hide();
            this.editButtonCommands.Visibility = Visibility.Collapsed;
        }

        private MobileServiceClient GetPrivateClient()
        {
            if (this.privateClient == null)
            {
                var field = typeof(App).GetRuntimeFields().SingleOrDefault(pi => pi.Name == "MobileService");
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
