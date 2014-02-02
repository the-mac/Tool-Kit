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
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace sdkImages.Scenarios
{
    // Note:
    // This scenario shows how to downlaod an image and how to display progress to the user
    // while the image is being download. However, if you have a very fast internet connection, you
    // might not observe the progessbar in operation. You can simulate a slower network connection using the 
    // Simulation Dashboard, describe at http://msdn.microsoft.com/en-us/library/windowsphone/develop/jj206953(v=vs.105).aspx
    //
    // If you have already tried this scenario, the internal caching mechanism of the phone/emulator means that the
    // image may appear instantaneously on subsequent runs of this scenario. To force the scenario  to download an image
    // you can 
    // a. change the imageURL in the following code
    // b. uninstall and then reinstall the sample
    // c. restart the phone or emulator
    // d. append a unique string to the end of the imageURL each time you load the image, i.e imageUrl = iamgeUrl + "?" + Guid.NewGuid.ToString()
    //

    public partial class Download : PhoneApplicationPage
    {
        string imageURL = "http://xbox.create.msdn.com/assets/cms/images/samples/windowsphonetestfile5.png";

        public Download()
        {
            InitializeComponent();
            BuildLocalizedApplicationBar();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            LoadTestImage();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            // Navigating away from the page so clear the TestImage and free the memory.
            if (TestImage.Source != null && TestImage.Source is BitmapImage)
                ((BitmapImage)TestImage.Source).UriSource = null;
        }

        private void LoadTestImage()
        {
            BitmapImage bmi = new BitmapImage();
            bmi.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            bmi.ImageOpened += bmi_ImageOpened;
            bmi.ImageFailed += bmi_ImageFailed;
            bmi.DownloadProgress += bmi_DownloadProgress;

            // To force the phone to download the image each time LoadTestImage is called you can
            // append a dummy, unique string to the end of the URL:
            // bmi.UriSource = new Uri(imageURL + "?" + Guid.NewGuid().ToString(), UriKind.Absolute);
            bmi.UriSource = new Uri(imageURL, UriKind.Absolute);
        }

        void bmi_DownloadProgress(object sender, DownloadProgressEventArgs e)
        {
            DownloadProgress.Value = (e.Progress == 100) ? 0 : e.Progress;
            Debug.WriteLine(e.Progress);
        }

        void bmi_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            MessageBox.Show(String.Format("Failed to load {0}\n Error: {1}", imageURL, e.ErrorException.Message));
        }

        void bmi_ImageOpened(object sender, RoutedEventArgs e)
        {
            TestImage.Source = (BitmapImage)sender;
            Debug.WriteLine("Image load complete");
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
            MessageBox.Show(AppResources.DownloadHelpText, AppResources.InfoCaption, MessageBoxButton.OK);
        }
    }
}
