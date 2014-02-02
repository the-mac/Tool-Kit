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
using PixPresenter.ViewModels;
using System;
using System.Diagnostics;
using System.Windows;

namespace PixPresenter
{
    public class ViewBase : PhoneApplicationPage
    {
        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            SharingViewModel.Instance.ConnectionStatusChanged -= new EventHandler<ConnectionStatusChangedEventArgs>(OnConnectionStatusChanged);
            SharingViewModel.Instance.PictureReceived -= new EventHandler<PictureReceivedEventArgs>(OnPictureReceived);
            base.OnNavigatedFrom(e);
        }
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            var uri = e.Uri.OriginalString;
            Debug.WriteLine(uri);

            if (uri.IndexOf("ms_nfp_launchargs") > 0
                && uri.IndexOf("Windows.Networking.Proximity.PeerFinder:StreamSocket") > 0)
            {
                SharingViewModel.Instance.StartSharingSession();
            }

            SharingViewModel.Instance.ConnectionStatusChanged += new EventHandler<ConnectionStatusChangedEventArgs>(OnConnectionStatusChanged);
            SharingViewModel.Instance.PictureReceived += new EventHandler<PictureReceivedEventArgs>(OnPictureReceived);
            App.NotificationPopup.CloseRequested += NotificationPopup_CloseRequested;

            if (SharingViewModel.Instance.IsConnected)
            {
                VisualStateManager.GoToState(this, "Connected", false);
            }
            
            base.OnNavigatedTo(e);
        }

        void OnPictureReceived(object sender, PictureReceivedEventArgs e)
        {
            App.RootFrame.Dispatcher.BeginInvoke(() =>
            {
                SoundHelper.PlaySound("Assets/Sounds/picreceived.wav");
                if (!App.RootFrame.CurrentSource.OriginalString.Contains("/Views/PictureView.xaml"))
                {
                    App.RootFrame.Navigate(new Uri("/Views/PictureView.xaml", UriKind.Relative));
                }
            });
        }

        void OnConnectionStatusChanged(object sender, ConnectionStatusChangedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(
            () =>
            {
                switch (e.Status)
                {
                    case ConnectionStatus.ReadyForTap:
                        VisualStateManager.GoToState(App.RootFrame, "ReadyForTap", false);
                        break;
                    case ConnectionStatus.Completed:
                        VisualStateManager.GoToState(App.RootFrame, "Connected", false);
                        break;
                    case ConnectionStatus.Failed:
                    case ConnectionStatus.Canceled:
                        VisualStateManager.GoToState(App.RootFrame, "TapCanceled", false);
                        break;
                    case ConnectionStatus.TapNotSupported:
                    case ConnectionStatus.Disconnected:
                        VisualStateManager.GoToState(App.RootFrame, "Disconnected", false);
                        break;
                    default:
                        break;
                }
            });
        }

        void NotificationPopup_CloseRequested(object sender, Shared.Controls.CloseMeEventArgs e)
        {
            VisualStateManager.GoToState(App.RootFrame, "TapCanceled", false);
            if (!SharingViewModel.Instance.IsConnected)
                SharingViewModel.Instance.StopSharingSession();
        }

        internal void Share_Click(object sender, EventArgs e)
        {
            if (!SharingViewModel.Instance.IsConnected)
            {
                SharingViewModel.Instance.StartSharingSession();
            }
            else
            {
                SharingViewModel.Instance.StopSharingSession();
            }
        }

    }
}
