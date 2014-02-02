using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Phone.Controls;
using Wazup.Services;


namespace Wazup.Views
{
    public partial class TrendsPage : PhoneApplicationPage
    {
        public TrendsPage()
        {
            InitializeComponent();
        }

        #region Trends

        /// <summary>
        /// Trends Dependency Property
        /// </summary>
        public static readonly DependencyProperty TrendsProperty =
            DependencyProperty.Register("Trends", typeof(ObservableCollection<Trend>), typeof(TrendsPage),
                new PropertyMetadata((ObservableCollection<Trend>)null));

        /// <summary>
        /// Gets or sets the Trends property. This dependency property 
        /// indicates the current twitter trends.
        /// </summary>
        public ObservableCollection<Trend> Trends
        {
            get { return (ObservableCollection<Trend>)GetValue(TrendsProperty); }
            set { SetValue(TrendsProperty, value); }
        }

        #endregion

        #region IsTrendsLoading

        /// <summary>
        /// IsTrendsLoading Dependency Property
        /// </summary>
        public static readonly DependencyProperty IsTrendsLoadingProperty =
            DependencyProperty.Register("IsTrendsLoading", typeof(bool), typeof(TrendsPage),
                new PropertyMetadata((bool)false));

        /// <summary>
        /// Gets or sets the IsTrendsLoading property. This dependency property 
        /// indicates whether we are currently loading trends.
        /// </summary>
        public bool IsTrendsLoading
        {
            get { return (bool)GetValue(IsTrendsLoadingProperty); }
            set { SetValue(IsTrendsLoadingProperty, value); }
        }

        #endregion

        /// <summary>
        /// Handles the MouseLeftButtonUp event of the TextBlockTrend control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void TextBlockTrend_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Trend trend = (sender as TextBlock).DataContext as Trend;

            this.GoToPage(ApplicationPages.Twitter);

            TwitterPage.Global_Trends = Trends;
            TwitterPage.Global_CurrentTrend = trend;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (Trends == null)
            {
                IsTrendsLoading = true;
                TwitterService.GetTrends(
                    delegate(IEnumerable<Trend> trends)
                    {
                        IsTrendsLoading = false;

                        Trends = new ObservableCollection<Trend>();

                        foreach (Trend trend in trends)
                        {
                            Trends.Add(trend);
                        }
                    },
                    delegate(Exception exception)
                    {
                        IsTrendsLoading = false;
                    });
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