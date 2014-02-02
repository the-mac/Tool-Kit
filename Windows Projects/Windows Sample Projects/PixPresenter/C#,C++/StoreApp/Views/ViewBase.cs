/* 
    Copyright (c) 2012 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Store apps, visit http://code.msdn.microsoft.com/windowsapps.
  
*/
using System;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using PixPresenter.ViewModels;

namespace PixPresenter.Views
{
    public partial class ViewBase : Page
    {
        public ViewBase()
        {
            this.SizeChanged += Page_SizeChanged;
        }

        // If page size changes, update the view state
        void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            GoToState(ApplicationView.Value.ToString());
        }


        public string AlbumName = "";

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                var args = e.Parameter as Windows.ApplicationModel.Activation.LaunchActivatedEventArgs;

                // If launched from a tap, complete the socket connection
                if (args != null && args.Kind == Windows.ApplicationModel.Activation.ActivationKind.Launch)
                {
                    if (args.Arguments == "Windows.Networking.Proximity.PeerFinder:StreamSocket")
                    {
                        SharingViewModel.Instance.StartSharingSession();
                    }
                }
            }

            // Register for events
            SharingViewModel.Instance.PictureReceived += new EventHandler<PictureReceivedEventArgs>(OnPictureReceived);
            SharingViewModel.Instance.ConnectionStatusChanged += new EventHandler<ConnectionStatusChangedEventArgs>(OnConnectionStatusChanged);

            if (SharingViewModel.Instance.IsConnected) 
            {
                GoToState("Connected");
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            // Unregister for events
            SharingViewModel.Instance.PictureReceived -= new EventHandler<PictureReceivedEventArgs>(OnPictureReceived);
            SharingViewModel.Instance.ConnectionStatusChanged -= new EventHandler<ConnectionStatusChangedEventArgs>(OnConnectionStatusChanged);
        }

        private void GoToState(string stateName)
        {
            VisualStateManager.GoToState(App.RootFrame, stateName, false); 
            VisualStateManager.GoToState(this, stateName, false); 
        }

        // Update visibility of connection-related UI items and play sounds when connection status changes
        private async void OnConnectionStatusChanged(object sender, ConnectionStatusChangedEventArgs e)
        {
            await App.RootFrame.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                () =>
                {
                    switch (e.Status)
                    {
                        case ConnectionStatus.ReadyForTap:
                            GoToState("ReadyForTap");
                            break;
                        case ConnectionStatus.Completed:
                            GoToState("Connected");
                            break;
                        case ConnectionStatus.Failed:
                        case ConnectionStatus.Canceled:
                            GoToState("TapCanceled");
                            break;
                        case ConnectionStatus.TapNotSupported:
                            break;
                        case ConnectionStatus.Disconnected:
                            GoToState("Disconnected");
                            break;
                        default:
                            break;
                    }
                });
        }


        // Handler for when a picture arrives from a connected device.
        protected async virtual void OnPictureReceived(object sender, PictureReceivedEventArgs e)
        {
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                () =>
                {            
            this.Frame.Navigate(typeof(PictureView), this.AlbumName);
            });
        }

        // Disconnect button handler from AppBar
        protected void DisconnectAlbumButton_Click(object sender, RoutedEventArgs e)
        {
            SharingViewModel.Instance.StopSharingSession();
        }

        // Start a tapped connection
        protected void ConnectAlbumButton_Click(object sender, RoutedEventArgs e)
        {
            if (!SharingViewModel.Instance.IsConnected)
                SharingViewModel.Instance.StartSharingSession();
        }

        // Opt out of a connection that has been started
        protected void connectionPopup_Tapped(object sender, TappedRoutedEventArgs e)
        {
            SharingViewModel.Instance.StopSharingSession();
        }

        protected void NotificationControlCloseRequested(object sender, Shared.Controls.CloseMeEventArgs e)
        {
            GoToState("TapCanceled");

            if (!SharingViewModel.Instance.IsConnected)
                SharingViewModel.Instance.StopSharingSession();
        }
    }
}
