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
using System.IO.IsolatedStorage;
using Microsoft.Phone.BackgroundTransfer;


namespace sdkBackgroundTransfersCS
{
    public partial class MainPage : PhoneApplicationPage
    {
        // List of BackgroundTransferRequest objects that will be bound
        // to the ListBox in MainPage.xaml
        IEnumerable<BackgroundTransferRequest> transferRequests;

        // Booleans for tracking if any transfers are waiting for user
        // action. 
        bool WaitingForExternalPower;
        bool WaitingForExternalPowerDueToBatterySaverMode;
        bool WaitingForNonVoiceBlockingNetwork;
        bool WaitingForWiFi;


        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            // Reset all of the user action booleans on page load.
            WaitingForExternalPower = false;
            WaitingForExternalPowerDueToBatterySaverMode = false;
            WaitingForNonVoiceBlockingNetwork = false;
            WaitingForWiFi = false;

            // When the page loads, refresh the list of file transfers.
            InitialTansferStatusCheck();
            UpdateUI();
        }

        private void InitialTansferStatusCheck()
        {
            UpdateRequestsList();

            foreach (var transfer in transferRequests)
            {
                transfer.TransferStatusChanged += new EventHandler<BackgroundTransferEventArgs>(transfer_TransferStatusChanged);
                transfer.TransferProgressChanged += new EventHandler<BackgroundTransferEventArgs>(transfer_TransferProgressChanged);
                ProcessTransfer(transfer);
            }

            if (WaitingForExternalPower)
            {
                MessageBox.Show("You have one or more file transfers waiting for external power. Connect your device to external power to continue transferring.");
            }
            if (WaitingForExternalPowerDueToBatterySaverMode)
            {
                MessageBox.Show("You have one or more file transfers waiting for external power. Connect your device to external power or disable Battery Saver Mode to continue transferring.");
            }
            if (WaitingForNonVoiceBlockingNetwork)
            {
                MessageBox.Show("You have one or more file transfers waiting for a network that supports simultaneous voice and data.");
            }
            if (WaitingForWiFi)
            {
                MessageBox.Show("You have one or more file transfers waiting for a WiFi connection. Connect your device to a WiFi network to continue transferring.");
            }

        }

        void transfer_TransferStatusChanged(object sender, BackgroundTransferEventArgs e)
        {
            ProcessTransfer(e.Request);
            UpdateUI();
        }

        private void ProcessTransfer(BackgroundTransferRequest transfer)
        {
            switch (transfer.TransferStatus)
            {
                case TransferStatus.Completed:

                    // If the status code of a completed transfer is 200 or 206, the
                    // transfer was successful
                    if (transfer.StatusCode == 200 || transfer.StatusCode == 206)
                    {
                        // Remove the transfer request in order to make room in the 
                        // queue for more transfers. Transfers are not automatically
                        // removed by the system.
                        RemoveTransferRequest(transfer.RequestId);

                        // In this example, the downloaded file is moved into the root
                        // Isolated Storage directory
                        using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
                        {
                            string filename = transfer.Tag;
                            if (isoStore.FileExists(filename))
                            {
                                isoStore.DeleteFile(filename);
                            }
                            isoStore.MoveFile(transfer.DownloadLocation.OriginalString, filename);
                        }

                    }
                    else
                    {
                        // This is where you can handle whatever error is indicated by the
                        // StatusCode and then remove the transfer from the queue. 
                        RemoveTransferRequest(transfer.RequestId);

                        if (transfer.TransferError != null)
                        {
                            // Handle TransferError, if there is one.
                        }
                    }
                    break;

                case TransferStatus.WaitingForExternalPower:
                    WaitingForExternalPower = true;
                    break;

                case TransferStatus.WaitingForExternalPowerDueToBatterySaverMode:
                    WaitingForExternalPowerDueToBatterySaverMode = true;
                    break;

                case TransferStatus.WaitingForNonVoiceBlockingNetwork:
                    WaitingForNonVoiceBlockingNetwork = true;
                    break;

                case TransferStatus.WaitingForWiFi:
                    WaitingForWiFi = true;
                    break;
            }
        }

        void transfer_TransferProgressChanged(object sender, BackgroundTransferEventArgs e)
        {
            UpdateUI();
        }

        private void UpdateRequestsList()
        {
            // The Requests property returns new references, so make sure that
            // you dispose of the old references to avoid memory leaks.
            if (transferRequests != null)
            {
                foreach (var request in transferRequests)
                {
                    request.Dispose();
                }
            }
            transferRequests = BackgroundTransferService.Requests;
        }

        private void UpdateUI()
        {
            // Update the list of transfer requests
            UpdateRequestsList();
   

            // Update the TransferListBox with the list of transfer requests.
            TransferListBox.ItemsSource = transferRequests;

            // If there are 1 or more transfers, hide the "no transfers"
            // TextBlock. IF there are zero transfers, show the TextBlock.
            if (TransferListBox.Items.Count > 0)
            {
                EmptyTextBlock.Visibility = Visibility.Collapsed;
            }
            else
            {
                EmptyTextBlock.Visibility = Visibility.Visible;
            }
        }



        private void RemoveTransferRequest(string transferID)
        {
            // Use Find to retrieve the transfer request with the specified ID.
            BackgroundTransferRequest transferToRemove = BackgroundTransferService.Find(transferID);

            // try to remove the transfer from the background transfer service.
            try
            {
                BackgroundTransferService.Remove(transferToRemove);
            }
            catch (Exception)
            {

            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            // The ID for each transfer request is bound to the
            // Tag property of each Remove button.
            string transferID = ((Button)sender).Tag as string;

            // Delete the transfer request
            RemoveTransferRequest(transferID);

            // Refresh the list of file transfers
            UpdateUI();
        }

        private void CancelAllButton_Click(object sender, EventArgs e)
        {
            // Loop through the list of transfer requests
            foreach (var transfer in BackgroundTransferService.Requests)
            {
                RemoveTransferRequest(transfer.RequestId);
            }

            // Refresh the list of file transfer requests.
            UpdateUI();
        }

        private void AddBackgroundTransferButton_Click(object sender, EventArgs e)
        {
            // Navigate to the AddBackgroundTransfer page when the Add button
            // is tapped.
            NavigationService.Navigate(new Uri("/AddBackgroundTransfer.xaml", UriKind.RelativeOrAbsolute));
        }
    }
}
