// ---------------------------------------------------------- 
// Copyright (c) Microsoft Corporation.  All rights reserved. 
// ---------------------------------------------------------- 
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using System.Data.Services.Client;
using ODataStreamingPhoneClient.Model;

namespace ODataStreamingPhoneClient
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // If we haven't already, load data from the service.
            if (!App.ViewModel.IsDataLoaded)
            {
                this.LoadData();

                // Bind the view (UI) to the ViewModel.
                this.DataContext = App.ViewModel;
            }
        }

        private void LoadData()
        {
            // Display the progress bar.
            this.requestProgress.Visibility = Visibility.Visible;
            this.requestProgress.IsIndeterminate = true;

            // Load data from the data service.
            App.ViewModel.LoadData();

            // Register for the LoadCompleted event to turn off the progress bar.
            App.ViewModel.Photos.LoadCompleted +=
                new EventHandler<LoadCompletedEventArgs>(Photos_LoadCompleted);
        }

        void Photos_LoadCompleted(object sender, System.Data.Services.Client.LoadCompletedEventArgs e)
        {
            // Hide the progress bar and unregister for the event.
            this.requestProgress.Visibility = Visibility.Collapsed;
            this.requestProgress.IsIndeterminate = false;
            App.ViewModel.Photos.LoadCompleted -= this.Photos_LoadCompleted;
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (App.ViewModel.IsDataLoaded)
            {
                var selector = (Selector)sender;
                if (selector.SelectedIndex == -1)
                {
                    return;
                }

                // Navigate to the details page for the selected item.
                this.NavigationService.Navigate(
                    new Uri("/PhotoDetailsPage.xaml?selectedIndex="
                        + selector.SelectedIndex, UriKind.Relative));

                selector.SelectedIndex = -1;
            }
        }

        private void OnCreatePhoto(object sender, EventArgs e)
        {
            if (App.ViewModel.IsDataLoaded)
            {
                // Create a new PhotoInfo instance.
                PhotoInfo newPhoto = PhotoInfo.CreatePhotoInfo(0, string.Empty,
                    DateTime.Now, new Exposure(), new Dimensions(), DateTime.Now);

                // Add the new photo to the tracking collection.
                App.ViewModel.Photos.Add(newPhoto);

                // Select the newly added photo.
                this.PhotosListBox.SelectedItem = newPhoto;
            }
        }

        private void OnRefresh(object sender, EventArgs e)
        {
            // Reload the data from the data service.
            this.LoadData();
        }
    }
}