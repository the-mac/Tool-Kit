/* 
    Copyright (c) 2012 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
using Microsoft.Phone.Controls;
using Microsoft.Phone.Storage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace sdkRouteMapperWP8CS
{
    public partial class MainPage : PhoneApplicationPage
    {
        // A collection of routes (.GPX files) for binding to the ListBox.
        public ObservableCollection<ExternalStorageFile> Routes {get;set;}
        
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Initialize the collection for routes.
            Routes = new ObservableCollection<ExternalStorageFile>();

            // Enable data binding to the page itself.
            this.DataContext = this;
        }
        
        // Scan the SD card to see if it contains any GPX files.
        private async void scanExternalStorageButton_Click_1(object sender, RoutedEventArgs e)
        {
            // Clear the collection bound to the page.
            Routes.Clear();

            // Connect to the current SD card.
            ExternalStorageDevice _sdCard = (await ExternalStorage.GetExternalStorageDevicesAsync()).FirstOrDefault();

            // If the SD card is present, add GPX files to the Routes collection.
            if (_sdCard != null)
            {
                try
                {
                    // Look for a folder on the SD card named Routes.
                    ExternalStorageFolder routesFolder = await _sdCard.GetFolderAsync("Routes");

                    // Get all files from the Routes folder.
                    IEnumerable<ExternalStorageFile> routeFiles = await routesFolder.GetFilesAsync();

                    // Add each GPX file to the Routes collection.
                    foreach (ExternalStorageFile esf in routeFiles)
                    {
                        if (esf.Path.EndsWith(".gpx"))
                        {
                            Routes.Add(esf);
                        }
                    }
                }
                catch (FileNotFoundException)
                {
                    // No Routes folder is present.
                    MessageBox.Show("The Routes folder is missing on your SD card. Add a Routes folder containing at least one .GPX file and try again.");
                }
            }
            else 
            {
                // No SD card is present.
                MessageBox.Show("The SD card is mssing. Insert an SD card that has a Routes folder containing at least one .GPX file and try again.");
            }
        }


        // When a different route is selected, launch the RoutePage and send the file path with the URI.
        private void gpxFilesListBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            ListBox lb = (ListBox)sender;

            if (lb.SelectedItem != null)
            {
                ExternalStorageFile esf = (ExternalStorageFile)lb.SelectedItem;
                NavigationService.Navigate(new Uri("/RoutePage.xaml?sdFilePath=" + esf.Path, UriKind.Relative));
            }
        }
    }
}
