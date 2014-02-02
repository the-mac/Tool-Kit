// ----------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// ----------------------------------------------------------

using System;
using System.ComponentModel;
using System.Data.Services.Client;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using System.IO.IsolatedStorage;
using ODataWinPhoneQuickstart.DataServiceContext.Netflix;

namespace ODataWinPhoneQuickstart
{
    public partial class MainPage : PhoneApplicationPage
    {
        private IsolatedStorageSettings appSettings;
        bool clearCache;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Set the view to the view model.
            this.DataContext = App.ViewModel;

            appSettings = IsolatedStorageSettings.ApplicationSettings;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Register for the load completed callback.
            App.ViewModel.LoadCompleted +=
                new EventHandler<SourcesLoadCompletedEventArgs>(ViewModel_LoadCompleted);

            // Show the progress bar.
            this.LoadProgress.IsIndeterminate = true;
            this.LoadProgress.Visibility = Visibility.Visible;
            this.ContentPanel.Opacity = .5;

            // Check to see if this is the not first page.
            if (this.NavigationContext.QueryString.Count == 1)
            {
                // Set the CurrentPage value to the requested page.
                App.ViewModel.CurrentPage = int.Parse(this.NavigationContext.QueryString["page"]);
            }
            else
            {
                // We are at the first page.
                App.ViewModel.CurrentPage = 0;

                // If there is no query parameter we are at the first page.
                // Check to see if we need to clear the cache.
                if (appSettings.TryGetValue("ClearCache", out clearCache) && clearCache)
                {
                    App.ViewModel.ClearCache();
                    MessageBox.Show("The stored title data has been removed.");

                    // Reset the app setting.
                    appSettings["ClearCache"] = false;
                    appSettings.Save();
                }
            }

            // Load data.
            App.ViewModel.LoadData();
        }

        void ViewModel_LoadCompleted(object sender, SourcesLoadCompletedEventArgs e)
        {
            // Hide the progress bar.
            this.LoadProgress.IsIndeterminate = false;
            this.LoadProgress.Visibility = Visibility.Collapsed;
            this.ContentPanel.Opacity = 1;

            App.ViewModel.LoadCompleted -= ViewModel_LoadCompleted;
        }
        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selector = (Selector)sender;
            if (selector.SelectedIndex == -1)
            {
                return;
            }

            // Set the selected title.
            App.ViewModel.SelectedTitle = 
                ((Selector)sender).SelectedItem as Title;

            // Navigate to the details page.
            this.NavigationService.Navigate(
                new Uri("/TitleDetailsPage.xaml", UriKind.Relative));

            selector.SelectedIndex = -1;
        }

        private void NextPageButton_Click(object sender, EventArgs e)
        {
            if (App.ViewModel.IsDataLoaded)
            {
                // Navigate to the next page of data.
                this.NavigationService.Navigate(
                   new Uri("/MainPage.xaml?page=" + (App.ViewModel.CurrentPage + 1), UriKind.Relative));
            }
        }
       
        private void ClearCacheButton_Click(object sender, EventArgs e)
        {
            // We need to clear the cache at the begining of execution.
            // If we clear the cache during a session, the paging in the DB will be
            // out of sync with the paging in the data service.
            if (MessageBox.Show("Do you want to delete all stored data the next time the application starts? "
                + " This will require the data to be reloaded from the data service.",
                "confirm", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                App.ViewModel.ClearLocalStorageOnNextStart();
            }           
        }        
    }
}