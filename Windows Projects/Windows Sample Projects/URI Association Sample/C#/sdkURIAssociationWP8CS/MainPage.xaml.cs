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
using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using Windows.Networking.Proximity;

namespace sdkURIAssociationWP8CS
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        private async void launchContoso_Click(object sender, RoutedEventArgs e)
        {
            string uriScheme = "contoso:ShowHandlerPage?CategoryID=aea6ae1f-9894-404e-8bca-ec47ec5b9c6c";
            currentUriSchemeText.Text = uriScheme;

            // Launch URI.
            await Windows.System.Launcher.LaunchUriAsync(new System.Uri(uriScheme));
        }

        private async void launchLitwareButton_Click(object sender, RoutedEventArgs e)
        {
            string uriScheme = "litware:ShowHandlerPage";
            currentUriSchemeText.Text = uriScheme;

            // Launch URI.
            await Windows.System.Launcher.LaunchUriAsync(new System.Uri(uriScheme));
        }

        private async void launchFabrikamButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Note: This sample deliberately does not include an app to handle the fabrikam URI scheme.");

            string uriScheme = "Fabrikam:NoHandlerAppsPresent";
            currentUriSchemeText.Text = uriScheme;

            // Launch URI.
            await Windows.System.Launcher.LaunchUriAsync(new System.Uri(uriScheme));
        }

        private async void launchCurrentButton_Click(object sender, RoutedEventArgs e)
        {
            string uriScheme = currentUriSchemeText.Text.Trim();

            if (uriScheme == "")
            {
                MessageBox.Show("First enter a URI scheme in the TextBox.");
            }
            else
            {
                try
                {
                    // Launch URI.
                    await Windows.System.Launcher.LaunchUriAsync(new System.Uri(uriScheme));
                }
                catch (Exception x)
                {
                    MessageBox.Show(x.Message);
                }
            }
        }

        private void launchProximityButton_Click(object sender, RoutedEventArgs e)
        {
            string uriScheme = currentUriSchemeText.Text.Trim();

            if (uriScheme == "")
            {
                MessageBox.Show("First enter a URI scheme in the TextBox.");
            }
            else
            {
                // Create the Proximity device and Proximity "tap phone now" popup.
                ProximityDevice device = ProximityDevice.GetDefault();
                Popup p = new Popup();

                // Make sure that NFC is supported.
                if (device != null)
                {
                    // Publish the current URI.
                    long proximityMessageId = device.PublishUriMessage(new System.Uri(uriScheme),
                          (ProximityDevice pSender, long messageId) =>
                          {
                              Dispatcher.BeginInvoke(() =>
                              {
                                  // Hide the Proximity control.
                                  p.IsOpen = false;
                                  // Stop publishing the Proximity message.
                                  pSender.StopPublishingMessage(messageId);
                              });
                          });

                    // Open the Proximity "tap phones now" control.
                    p.Child = new ProximityUserControl(proximityMessageId);
                    p.IsOpen = true;

                }
            }
        }
    }
}
