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

namespace ODataWinPhoneQuickstart
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Set the view to the view model.
            this.DataContext = App.ViewModel;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }
            else
            {
                if (this.NavigationContext.QueryString.Count == 1)
                {
                    // Get the value of the requested page.
                    int page = int.Parse(this.NavigationContext.QueryString["page"]);

                    // Check to see if the page is currently loaded.
                    if (page != App.ViewModel.CurrentPage)
                    {
                        // Load data for the specific page.
                        App.ViewModel.LoadData(page);
                    }
                }
                else
                {
                    // If there is no query parameter we are at the first page.
                    App.ViewModel.LoadData(0);
                }
            }
        }
        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selector = (Selector)sender;
            if (selector.SelectedIndex == -1)
            {
                return;
            }

            this.NavigationService.Navigate(
                new Uri("/TitleDetailsPage.xaml?selectedIndex=" + selector.SelectedIndex, UriKind.Relative));

            selector.SelectedIndex = -1;
        }

        private void MoreButton_Click(object sender, RoutedEventArgs e)
        {
            if (App.ViewModel.IsDataLoaded)
            {
                // Navigate to the next page of data.
                this.NavigationService.Navigate(
                   new Uri("/MainPage.xaml?page=" + (App.ViewModel.CurrentPage + 1), UriKind.Relative));
            }
        }
    }
}