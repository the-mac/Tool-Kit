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
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using PixPresenter.ViewModels;
using PixPresenter.Views;
using PixPresenterPortableLib;
using PixPresenter.Shared.Controls;

namespace PixPresenter
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        public static NotificationControl NotificationPopup;

        private static AlbumsViewModel _avm;
        /// <summary>
        /// Provides access to picture Albums.
        /// </summary>
        public static AlbumsViewModel AlbumsViewModel
        {
            get
            {
                lock (typeof(App))
                {
                    if (_avm == null)
                    {
                        _avm = new AlbumsViewModel(AlbumsService);
                    }
                }

                return _avm;
            }
        }

        private static IAlbumsService _albumsService;
        /// <summary>
        /// Provides access to picture albums on the local device.
        /// </summary>
        public static IAlbumsService AlbumsService
        {
            get
            {
                lock (typeof(App))
                {
                    if (_albumsService == null)
                    {
                        _albumsService = new AlbumsService();
                    }
                }

                return _albumsService;
            }
        }

        private static IAlbumService _albumService;
        /// <summary>
        /// Provides access to a picture album on the local device.
        /// </summary>
        public static IAlbumService AlbumService
        {
            get
            {
                lock (typeof(App))
                {
                    if (_albumService == null)
                    {
                        _albumService = new AlbumService();
                    }
                }

                return _albumService;
            }
        }

        private static IPictureService _pictureService;
        /// <summary>
        /// Provides access to a picture on the local device.
        /// </summary>
        public static IPictureService PictureService
        {
            get
            {
                lock (typeof(App))
                {
                    if (_pictureService == null)
                    {
                        _pictureService = new PictureService();
                    }
                }

                return _pictureService;
            }
        }

        /// <summary>
        /// Provides easy access to the root frame of the app.
        /// </summary>
        /// <returns>The root frame of the app.</returns>
        public static Frame RootFrame { get; private set; }

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            SharingViewModel.Instance.UIDispatcher = async (a) => await RootFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, new DispatchedHandler(a));
        }


        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            if (Window.Current.Content == null)
            {
                RootFrame = new Frame();

                // Set the template for the RootFrame to the new template we created in the Application.Resources in App.xaml
                RootFrame.Template = Resources["NewFrameTemplate"] as ControlTemplate;
                RootFrame.ApplyTemplate();
                NotificationPopup = (VisualTreeHelper.GetChild(RootFrame, 0) as FrameworkElement).FindName("notifyControl") as NotificationControl;
                NotificationPopup.CloseRequested += NotificationPopup_CloseRequested;

                RootFrame.Navigate(typeof(AlbumsView), args);
                Window.Current.Content = RootFrame;                
            }
            else
            {
                RootFrame = Window.Current.Content as Frame;
                RootFrame.Navigate(typeof(AlbumsView), args);
            }

            // Ensure the current window is active
            Window.Current.Activate();
        }

        void NotificationPopup_CloseRequested(object sender, CloseMeEventArgs e)
        {
            VisualStateManager.GoToState(App.RootFrame, "TapCanceled", false);
        }


        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            deferral.Complete();
        }
    }
}
