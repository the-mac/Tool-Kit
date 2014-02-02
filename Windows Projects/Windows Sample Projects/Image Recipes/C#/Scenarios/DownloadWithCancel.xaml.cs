/* 
    Copyright (c) 2012 - 2013 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://code.msdn.microsoft.com/wpapps
  
*/
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using sdkImages.Resources;
using System;
using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace sdkImages.Scenarios
{
    /*
     * The WebClient class is used in order to demonstrate the CancelAsync method, which allows the download to be cancelled. 
     * If the download happens too quickly, you can slow it down using the Simulation Dashboard, which is described at
     * http://msdn.microsoft.com/en-us/library/windowsphone/develop/jj206953(v=vs.105).aspx
     * Once the image is downloaded it is cached internally by the phone. 
     * If you have already tried this scenario, the internal caching mechanism of the phone/emulator means that the
     * image may appear instantaneously on subsequent runs of this scenario. To force the scenario  to download an image you can 
     * 
     * a. change the imageURL in the following code
     * b. uninstall and then reinstall the sample
     * c. restart the phone or emulator
     * d. append a unique string to the end of the imageURL each time you load the image, i.e imageUrl = iamgeUrl + "?" + Guid.NewGuid.ToString()
     *
     * */

    public partial class DownloadWithCancel : PhoneApplicationPage
    {
        string imageURL = "http://xbox.create.msdn.com/assets/cms/images/samples/windowsphonetestfile4.png";

        WebClient webClient = null;
        public DownloadWithCancel()
        {
            InitializeComponent();
            BuildLocalizedApplicationBar();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            LoadTestImage();
        }
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            Cleanup();

            // Navigating away from the page so clear the TestImage and free the memory.
            if (TestImage.Source != null && TestImage.Source is BitmapImage)
                ((BitmapImage)TestImage.Source).UriSource = null;

            base.OnNavigatingFrom(e);
        }

        private void LoadTestImage()
        {
            CancelDownloadButton.IsEnabled = true;
            webClient = new WebClient();
            webClient.OpenReadCompleted += wc_OpenReadCompleted;
            webClient.DownloadProgressChanged += wc_DownloadProgressChanged;

            // To force the phone to download the image each time LoadTestImage is called you can
            // append a dummy, unique string to the end of the URL:
            // bmi.UriSource = new Uri(imageURL + "?" + Guid.NewGuid().ToString(), UriKind.Absolute);
            webClient.OpenReadAsync(new Uri(imageURL, UriKind.Absolute));
        }

        void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            DownloadProgress.Value = (e.ProgressPercentage == 100) ? 0 : e.ProgressPercentage;
        }

        void wc_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {

            if (!e.Cancelled)
            {
                if (e.Error != null)
                {
                    MessageBox.Show(String.Format("Image download failed.\nError: {0}", e.Error.InnerException.Message));
                    Cleanup();
                    return;
                }

                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.SetSource(e.Result);
                    TestImage.Source = bitmapImage;
                    Cleanup();
                });
            }
            else
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    Cleanup();
                    MessageBox.Show("Download cancelled");
                });
            }
        }

        private void Cleanup()
        {
            CancelDownloadButton.IsEnabled = false;
            DownloadProgress.Value = 0;
            if (webClient != null)
            {
                CancelCurrentDownload();

                webClient.OpenReadCompleted -= wc_OpenReadCompleted;
                webClient.DownloadProgressChanged -= wc_DownloadProgressChanged;
                webClient = null;
            }
            

        }

        private void CancelCurrentDownload()
        {
            if (webClient != null)
            {
                if (webClient.IsBusy)
                {
                    webClient.CancelAsync();
                }
            }
        }

        private void CancelDownloadButton_Tap_1(object sender, System.Windows.Input.GestureEventArgs e)
        {
            CancelCurrentDownload();
        }

        // Sample code for building a localized ApplicationBar
        private void BuildLocalizedApplicationBar()
        {
            // Set the page's ApplicationBar to a new instance of ApplicationBar.
            ApplicationBar = new ApplicationBar();

            // Create a new button and set the text value to the localized string from AppResources.
            ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/appbar_info.png", UriKind.Relative));
            appBarButton.Click += appBarButton_Click;
            appBarButton.Text = AppResources.AppBarButtonInfoText;
            ApplicationBar.Buttons.Add(appBarButton);

        }

        void appBarButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show(AppResources.DownloadCancelHelpText, AppResources.InfoCaption, MessageBoxButton.OK);
        }
    }
}
