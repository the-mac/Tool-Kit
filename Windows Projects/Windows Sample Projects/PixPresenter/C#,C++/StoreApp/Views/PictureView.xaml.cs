/* 
    Copyright (c) 2012 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Store apps, visit http://code.msdn.microsoft.com/windowsapps.
  
*/
using PixPresenter.ViewModels;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace PixPresenter.Views
{
    public sealed partial class PictureView : ViewBase
    {
        public PictureView()
        {
            this.InitializeComponent();
            this.DataContext = SharingViewModel.Instance;
        }

        private string _albumName;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // if navigated to from an album, store the album name for back navigation
            if (e.Parameter != null) { _albumName = e.Parameter.ToString(); }

            base.OnNavigatedTo(e);
        }

        protected override void OnPictureReceived(object sender, PictureReceivedEventArgs e)
        {
            // Override OnPictureReceived so that only one instance of the PictureView is used.
        }

        // Navigate back to previous view
        void backButton_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(_albumName))
                this.Frame.Navigate(typeof(AlbumsView));
            else
                this.Frame.Navigate(typeof(AlbumView), _albumName);

        }

        // Picture zoom
        void Image_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            Image img = (Image)sender;
            img.Width = img.ActualWidth * e.Delta.Scale;
            img.Height = img.ActualHeight * e.Delta.Scale;
        }
    }
}
