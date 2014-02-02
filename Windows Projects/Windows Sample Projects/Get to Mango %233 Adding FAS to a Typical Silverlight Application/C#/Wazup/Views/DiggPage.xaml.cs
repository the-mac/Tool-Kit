using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Microsoft.Phone.Controls;
using Wazup.Services;


namespace Wazup.Views
{
    public partial class DiggPage : PhoneApplicationPage
    {
        public DiggPage()
        {
            InitializeComponent();
        }

        #region SearchText

        /// <summary>
        /// SearchText Dependency Property
        /// </summary>
        public static readonly DependencyProperty SearchTextProperty =
            DependencyProperty.Register("SearchText", typeof(string), typeof(DiggPage),
                new PropertyMetadata((string)""));

        /// <summary>
        /// Gets or sets the SearchText property. This dependency property 
        /// indicates the text to be searched.
        /// </summary>
        public string SearchText
        {
            get { return (string)GetValue(SearchTextProperty); }
            set { SetValue(SearchTextProperty, value); }
        }

        #endregion

        #region LastSearchText

        /// <summary>
        /// LastSearchText Dependency Property
        /// </summary>
        public static readonly DependencyProperty LastSearchTextProperty =
            DependencyProperty.Register("LastSearchText", typeof(string), typeof(DiggPage),
                new PropertyMetadata((string)""));

        /// <summary>
        /// Gets or sets the LastSearchText property. This dependency property 
        /// indicates the last searched text.
        /// </summary>
        public string LastSearchText
        {
            get { return (string)GetValue(LastSearchTextProperty); }
            set { SetValue(LastSearchTextProperty, value); }
        }

        #endregion

        #region DiggSearchResults

        /// <summary>
        /// DiggSearchResults Dependency Property
        /// </summary>
        public static readonly DependencyProperty DiggSearchResultsProperty =
            DependencyProperty.Register("DiggSearchResults", typeof(ObservableCollection<DiggStory>), typeof(DiggPage),
                new PropertyMetadata((ObservableCollection<DiggStory>)null));

        /// <summary>
        /// Gets or sets the DiggSearchResults property. This dependency property 
        /// indicates digg search results.
        /// </summary>
        public ObservableCollection<DiggStory> DiggSearchResults
        {
            get { return (ObservableCollection<DiggStory>)GetValue(DiggSearchResultsProperty); }
            set { SetValue(DiggSearchResultsProperty, value); }
        }

        #endregion

        #region IsSearching

        /// <summary>
        /// IsSearching Dependency Property
        /// </summary>
        public static readonly DependencyProperty IsSearchingProperty =
            DependencyProperty.Register("IsSearching", typeof(bool), typeof(DiggPage),
                new PropertyMetadata((bool)false));

        /// <summary>
        /// Gets or sets the IsSearching property. This dependency property 
        /// indicates whether we are currently searching.
        /// </summary>
        public bool IsSearching
        {
            get { return (bool)GetValue(IsSearchingProperty); }
            set { SetValue(IsSearchingProperty, value); }
        }

        #endregion

        /// <summary>
        /// Handles the Click event of the ButtonSearch control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(SearchText))
            {
                return;
            }

            IsSearching = true;
            DiggService.Search(SearchText,
                delegate(IEnumerable<DiggStory> diggSearchResults)
                {
                    IsSearching = false;

                    LastSearchText = SearchText;

                    DiggSearchResults = new ObservableCollection<DiggStory>();

                    foreach (DiggStory diggSearchResult in diggSearchResults)
                    {
                        DiggSearchResults.Add(diggSearchResult);
                    }
                },
                delegate(string searchText, Exception exception)
                {
                    IsSearching = false;
                    LastSearchText = string.Format("Error while searching {0}.", searchText);
                    System.Diagnostics.Debug.WriteLine(exception);
                });
        }
                
        #region Appbar handlers

        /// <summary>
        /// Handles the Click event of the AppbarButtonDigg control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void AppbarButtonDigg_Click(object sender, EventArgs e)
        {
            SearchText = string.Empty;
            LastSearchText = string.Empty;
            DiggSearchResults = null;
        }

        /// <summary>
        /// Handles the Click event of the AppbarButtonTwitter control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void AppbarButtonTwitter_Click(object sender, EventArgs e)
        {
            this.GoToPage(ApplicationPages.Trends);
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