using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using Wazup.Services;

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

        #region IsTwitsLoading

        /// <summary>
        /// IsTwitsLoading Dependency Property
        /// </summary>
        public static readonly DependencyProperty IsTwitsLoadingProperty =
            DependencyProperty.Register("IsTwitsLoading", typeof(bool), typeof(TwitterPage),
                new PropertyMetadata((bool)false));

        /// <summary>
        /// Gets or sets the IsTwitsLoading property. This dependency property 
        /// indicates whether we are currently loading twits.
        /// </summary>
        public bool IsTwitsLoading
        {
            get { return (bool)GetValue(IsTwitsLoadingProperty); }
            set { SetValue(IsTwitsLoadingProperty, value); }
        }

        #endregion

        private void PivotControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
            {
                return;
            }

            Trend selectedTrend = e.AddedItems[0] as Trend;

            if ((selectedTrend.Twits == null) || (selectedTrend.Twits.Count == 0))
            {
                IsTwitsLoading = true;
            }
            TwitterService.Search(selectedTrend.name,
                delegate(IEnumerable<Twit> twits)
                {
                    IsTwitsLoading = false;

                    selectedTrend.Twits = new ObservableCollection<Twit>();

                    foreach (Twit twit in twits)
                    {
                        selectedTrend.Twits.Add(twit);
                    }
                },
                delegate(Exception exception)
                {
                    IsTwitsLoading = false;
                });
        }        

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (Global_Trends != null)
            {
                // get data from trends page, if we got from trends page
                Trends = Global_Trends;
                CurrentTrend = Global_CurrentTrend;
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
            NavigationService.GoBack();
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