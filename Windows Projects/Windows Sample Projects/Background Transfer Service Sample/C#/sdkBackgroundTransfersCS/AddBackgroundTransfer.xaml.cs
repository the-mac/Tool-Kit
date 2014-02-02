/* 
    Copyright (c) 2011 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using Microsoft.Phone.BackgroundTransfer;
using System.IO.IsolatedStorage;

namespace sdkBackgroundTransfersCS
{
    public partial class AddBackgroundTransfer : PhoneApplicationPage
    {
        private readonly List<string> urls =
            new List<string>
                {
                    "http://create.msdn.com/assets/cms/images/samples/windowsphonetestfile1.png",  
                    "http://create.msdn.com/assets/cms/images/samples/windowsphonetestfile2.png",  
                    "http://create.msdn.com/assets/cms/images/samples/windowsphonetestfile3A.png",  
                    "http://create.msdn.com/assets/cms/images/samples/windowsphonetestfile4.png",  
                    "http://create.msdn.com/assets/cms/images/samples/windowsphonetestfile5.png",  
                };

        public AddBackgroundTransfer()
        {
            InitializeComponent();

            // Bind the list of urls to the ListBox
            URLListBox.ItemsSource = urls;

            // Make sure that the required "transfers" directory exists
            // in isolated storage.
            using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (!isoStore.DirectoryExists("/shared/transfers"))
                {
                    isoStore.CreateDirectory("/shared/transfers");
                }
            }
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            // Check to see if the maximum number of requests per app has been exceeded.
            if (BackgroundTransferService.Requests.Count() >= 5)
            {
                // Note: Instead of showing a message to the user, you could store the
                // requested file URI in isolated storage and add it to the queue later.
                MessageBox.Show("The maximum number of background file transfer requests for this application has been exceeded. ");
                return;
            }

            // Get the URI of the file to be transferred from the Tag property
            // of the button that was clicked.
            string transferFileName = ((Button)sender).Tag as string;
            Uri transferUri = new Uri(Uri.EscapeUriString(transferFileName), UriKind.RelativeOrAbsolute);

            // Create the new transfer request, passing in the URI of the file to 
            // be transferred.
            BackgroundTransferRequest transferRequest = new BackgroundTransferRequest(transferUri);

            // Set the transfer method. GET and POST are supported.
            transferRequest.Method = "GET";

            // Get the file name from the end of the transfer Uri and create a local Uri 
            // in the "transfers" directory in isolated storage.
            string downloadFile = transferFileName.Substring(transferFileName.LastIndexOf("/") + 1);
            Uri downloadUri = new Uri("shared/transfers/" + downloadFile, UriKind.RelativeOrAbsolute);
            transferRequest.DownloadLocation = downloadUri;

            // Pass custom data with the Tag property. This value cannot be more than 4000 characters.
            // In this example, the friendly name for the file is passed. 
            transferRequest.Tag = downloadFile;



            // If the WiFi-only checkbox is not checked, then set the TransferPreferences
            // to allow transfers over a cellular connection.
            if (wifiOnlyCheckbox.IsChecked == false)
            {
                transferRequest.TransferPreferences = TransferPreferences.AllowCellular;
            }
            if (externalPowerOnlyCheckbox.IsChecked == false)
            {
                transferRequest.TransferPreferences = TransferPreferences.AllowBattery;
            }
            if (wifiOnlyCheckbox.IsChecked == false && externalPowerOnlyCheckbox.IsChecked == false)
            {
                transferRequest.TransferPreferences = TransferPreferences.AllowCellularAndBattery;
            }


            // Add the transfer request using the BackgroundTransferService. Do this in 
            // a try block in case an exception is thrown.
            try
            {
                BackgroundTransferService.Add(transferRequest);
            }
            catch (InvalidOperationException ex)
            {
                // TBD - update when exceptions are finalized
                MessageBox.Show("Unable to add background transfer request. " + ex.Message);
            }
            catch (Exception)
            {
                MessageBox.Show("Unable to add background transfer request.");
            }

        }
    }
}
