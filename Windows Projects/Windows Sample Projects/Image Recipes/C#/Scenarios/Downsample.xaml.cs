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
using sdkImages.Scenarios;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace sdkImages
{
    // In this example, we are using buttons to dynamically change the amount by which the test image
    // is downsampled using the DecodePixelWidth and DecodePixelHeight properties of the BitmapImage.
    // In a real app, it is possible that these properties are set only once. If that is the case, it is easier
    // to do this in XAML as follows:
    //
    //  <Image Width="350" Height="200">
    //      <Image.Source>
    //          <BitmapImage UriSource="/Assets/test.jpg" DecodePixelHeight="200" DecodePixelWidth="350"/>
    //      </Image.Source> 
    //  </Image>
    //
    // If you are maintaining the aspect ration of the image, you only need to set either the DecodePixelHeight or DecodePixelWidth
    //
    // This is done here in code-behind to let you experiment with the effect of downsampling on the memory footprint of the app and the
    // quality of the image being displayed.

    public partial class Downsample : PhoneApplicationPage
    {
        // The test image and current memory stats are wrapped in a ViewModel, making it easy to
        // bind to the UI.
        public DownsampleViewModel ViewModel;

        public Downsample()
        {
            InitializeComponent();

            this.ViewModel = new DownsampleViewModel();
            this.DataContext = this.ViewModel;

            BuildLocalizedApplicationBar();
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
            MessageBox.Show(AppResources.DownsampleHelpText, AppResources.InfoCaption, MessageBoxButton.OK);
        }

        private void DecodePixelWidthIncrease_Click_1(object sender, RoutedEventArgs e)
        {
            this.ViewModel.IncrementDecodePixelWidth();
        }

        private void DecodePixelWidthDecrease_Click_1(object sender, RoutedEventArgs e)
        {
            this.ViewModel.DecrementDecodePixelWidth();
        }

        private void DecodePixelHeightDecrease_Click_1(object sender, RoutedEventArgs e)
        {
            this.ViewModel.DecrementDecodePixelHeight();
        }

        private void DecodePixelHeightIncrease_Click_1(object sender, RoutedEventArgs e)
        {
            this.ViewModel.IncrementDecodePixelHeight();
        }

    }
   
}
