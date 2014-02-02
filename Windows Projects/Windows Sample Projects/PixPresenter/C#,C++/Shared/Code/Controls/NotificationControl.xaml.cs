/* 
    Copyright (c) 2012 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
    To see all Code Samples for Windows Store apps, visit http://code.msdn.microsoft.com/windowsapps
  
*/
using System;
using System.Diagnostics;

#if NETFX_CORE
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#else
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media.Animation;
#endif

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace PixPresenter.Shared.Controls
{
    public class CloseMeEventArgs : EventArgs
    {
    }

    public sealed partial class NotificationControl : UserControl
    {
        public event EventHandler<CloseMeEventArgs> CloseRequested;

        public NotificationControl()
        {
            this.InitializeComponent();

            this.DataContext = this;

#if NETFX_CORE
            LayoutRoot.Tapped += ((s, args) =>
#else
            LayoutRoot.Tap += ((s, args) =>
#endif
            {
                HandleTap();
            });
        }
        
        // Status is a DependencyProperty since it is used as a TargetProperty in a Storyboard, which must be a DependencyProperty
        public static readonly DependencyProperty StatusProperty = DependencyProperty.Register("Status", typeof(String), typeof(NotificationControl), new PropertyMetadata(String.Empty, new PropertyChangedCallback(OnStatusChanged)));
        public String Status
        {
            get { return (String)GetValue(StatusProperty); }
        }

#if NETFX_CORE
        public static readonly Uri _disconnectedSound = new Uri("ms-appx:///assets/sounds/disconnected.wav");
        public static readonly Uri _connectedSound = new Uri("ms-appx:///assets/sounds/connected.wav");
#else
        private static readonly Uri _disconnectedSound = new Uri("Assets/Sounds/disconnected.wav", UriKind.Relative);
        private static readonly Uri _connectedSound = new Uri("Assets/Sounds/connected.wav", UriKind.Relative);
#endif

        public Uri DisconnectedSound
        {
            get
            {
                return _disconnectedSound;
            }
        }

        public Uri ConnectedSound
        {
            get
            {
                return _connectedSound;
            }
        }

        private static void OnStatusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                Debug.WriteLine("new value {0}", e.NewValue);
              PixPresenter.ViewModels.SharingViewModel.Instance.UIDispatcher(() =>
                {
                    var control = d as Control;
                    switch (e.NewValue.ToString())
                    {
                        case "Canceled":
                            VisualStateManager.GoToState(control, "Canceled", false);
                            break;
                        case "Completed":
                            VisualStateManager.GoToState(control, "Completed", false);
                            break;
                        case "Disconnected":
                            VisualStateManager.GoToState(control, "Disconnected", false);
                            break;
                        case "Failed":                            
                            VisualStateManager.GoToState(control, "Disconnected", false);
                            break;
                        case "PeerFound":
                            VisualStateManager.GoToState(control, "PeerFound", false);
                            break;
                        case "ReadyForTap":
                            VisualStateManager.GoToState(control, "ReadyForTap", false);
                            break;
                        case "TapNotSupported":
                            VisualStateManager.GoToState(control, "TapNotSupported", false);
                            break;
                    }
                });
            }
        }

        void HandleTap()
        {
            Debug.WriteLine("tapped");

            if (CloseRequested != null)
                CloseRequested(this, new CloseMeEventArgs());
        }
    }
}
