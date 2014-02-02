using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using Wazup.Services;
using System.IO.IsolatedStorage;
using Wazup.Helpers;

namespace Wazup.Views
{
    public partial class TwitterPage : PhoneApplicationPage
    {
        /// <summary>
        /// Used to transfer the Trends collection between the trends page and the twitter page
        /// </summary>
        public static ObservableCollection<Trend> Global_Trends;

        /// <summary>
        /// Used to transfer the current Trend between the trends page and the twitter page
        /// </summary>
        public static Trend Global_CurrentTrend;

        public TwitterPage()
        {
            InitializeComponent();
        }

        #region Trends

        /// <summary>
        /// Trends Dependency Property
        /// </summary>
        public static readonly DependencyProperty TrendsProperty =
            DependencyProperty.Register("Trends", typeof(ObservableCollection<Trend>), typeof(TwitterPage),
                new PropertyMetadata((ObservableCollection<Trend>)null));

        /// <summary>
        /// Gets or sets the Trends property. This dependency property 
        /// indicates what are the current twitter trends.
        /// </summary>
        public ObservableCollection<Trend> Trends
        {
            get { return (ObservableCollection<Trend>)GetValue(TrendsProperty); }
            set { SetValue(TrendsProperty, value); }
        }

        #endregion

        #region CurrentTrend

        /// <summary>
        /// CurrentTrend Dependency Property
        /// </summary>
        public static readonly DependencyProperty CurrentTrendProperty =
            DependencyProperty.Register("CurrentTrend", typeof(Trend), typeof(TwitterPage),
                new PropertyMetadata((Trend)null,
                    new PropertyChangedCallback(OnCurrentTrendChanged)));

        /// <summary>
        /// Gets or sets the CurrentTrend property. This dependency property 
        /// indicates what is the current trend.
        /// </summary>
        public Trend CurrentTrend
        {
            get { return (Trend)GetValue(CurrentTrendProperty); }
            set { SetValue(CurrentTrendProperty, value); }
        }

        /// <summary>
        /// Handles changes to the CurrentTrend property.
        /// </summary>
        private static void OnCurrentTrendChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TwitterPage target = (TwitterPage)d;
            Trend oldCurrentTrend = (Trend)e.OldValue;
            Trend newCurrentTrend = target.CurrentTrend;
            target.OnCurrentTrendChanged(oldCurrentTrend, newCurrentTrend);
        }
        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the CurrentTrend property.
        /// </summary>
        protected virtual void OnCurrentTrendChanged(Trend oldCurrentTrend, Trend newCurrentTrend)
        {
            if (newCurrentTrend != oldCurrentTrend)
            {
                Dispatcher.BeginInvoke((Action)delegate
                {
                    PivotControl.SelectedItem = newCurrentTrend;
                });
            }
        }

        #endregion

        private Trend restoredTrend;

        #region IsTweetsLoading

        /// <summary>
        /// IsTweetsLoading Dependency Property
        /// </summary>
        public static readonly DependencyProperty IsTweetsLoadingProperty =
            DependencyProperty.Register("IsTweetsLoading", typeof(bool), typeof(TwitterPage),
                new PropertyMetadata((bool)false));

        /// <summary>
        /// Gets or sets the IsTweetsLoading property. This dependency property 
        /// indicates whether we are currently loading Tweets.
        /// </summary>
        public bool IsTweetsLoading
        {
            get { return (bool)GetValue(IsTweetsLoadingProperty); }
            set { SetValue(IsTweetsLoadingProperty, value); }
        }

        #endregion

        private void PivotControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0 || e.AddedItems[0] == null)
            {
                return;
            }

            if (restoredTrend != null)
            {
                // When returning from tombstoning, we get a selection change for the first
                // trend which we need to mitigate
                CurrentTrend = restoredTrend;
                restoredTrend = null;
                return;
            }

            Trend selectedTrend = e.AddedItems[0] as Trend;

            if ((selectedTrend.Tweets == null) || (selectedTrend.Tweets.Count == 0))
            {
                IsTweetsLoading = true;

                selectedTrend.last_tweet_fetch = DateTime.Now;
            }
            TwitterService.Search(selectedTrend.name,
                delegate(IEnumerable<Tweet> tweets)
                {
                    IsTweetsLoading = false;

                    selectedTrend.Tweets = new ObservableCollection<Tweet>();

                    foreach (Tweet tweet in tweets)
                    {
                        selectedTrend.Tweets.Add(tweet);
                    }
                },
                delegate(Exception exception)
                {
                    IsTweetsLoading = false;
                });
        }

        private const string TrendsKey = "TrendsKey";
        private const string CurrentTrendKey = "CurrentTrendKey";

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            this.SaveState(TrendsKey, Trends);
            this.SaveState(CurrentTrendKey, CurrentTrend);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (NavigationContext.QueryString.Count != 0)
            {
                string trendNameToDisplay = UriHelper.GetTrendNameFromUri(e.Uri);

                // We have a query string, so start up in "Single Trend" mode
                ObservableCollection<Trend> singleTrend = new ObservableCollection<Trend>();
                singleTrend.Add((Trend)IsolatedStorageSettings.ApplicationSettings[trendNameToDisplay]);
                // Since we are setting the trends and only have one, it will be the one to be displayed
                Trends = singleTrend;

                return;
            }

            if (Global_Trends != null)
            {
                // get data from trends page, if we got from trends page
                Trends = Global_Trends;
                CurrentTrend = Global_CurrentTrend;
            }
            else 
            {
                // We returned from outside the application (tombstoning/FAS), so load the state
                Trends = this.LoadState<ObservableCollection<Trend>>(TrendsKey);
                restoredTrend = this.LoadState<Trend>(CurrentTrendKey);
            }                        
        }

        #region Appbar handlers

        /// <summary>
        /// Handles the Click event of the AppbarButtonDigg control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void AppbarButtonDigg_Click(object sender, EventArgs e)
        {
            this.GoToPage(ApplicationPages.Digg);
        }

        /// <summary>
        /// Handles the Click event of the AppbarButtonTwitter control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void AppbarButtonTwitter_Click(object sender, EventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
            else
            {
                this.GoToPage(ApplicationPages.Trends);
            }
        }

        /// <summary>
        /// Handles the Click event of the AppbarButtonBlog control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void AppbarButtonBlog_Click(object sender, EventArgs e)
        {
            this.GoToPage(ApplicationPages.Blog);
        }

        #endregion
    }
}