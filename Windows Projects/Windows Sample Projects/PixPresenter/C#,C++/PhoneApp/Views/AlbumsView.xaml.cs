/* 
    Copyright (c) 2012 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
using Microsoft.Phone.Shell;
using PixPresenter.ViewModels;
using PixPresenterPortableLib;
using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;

namespace PixPresenter
{
    /// <summary>
    /// Code behind for AlbumsView.xaml
    /// </summary>
    public partial class AlbumsView : ViewBase
    {
        private bool _initialised = false;

        // Constructor
        public AlbumsView()
        {
            InitializeComponent();
            BuildLocalizedApplicationBar();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // Make sure no album is selected, so that when we come back we can select again
            AlbumsList.SelectedItem = null;
            base.OnNavigatedFrom(e);
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (_initialised)
            return;

            // Need to await since this modifies the data context on a background thread
            await App.AlbumsViewModel.LoadAlbumsAsync();
            this.DataContext = App.AlbumsViewModel;

            _initialised = true;
        }

        void ListBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            ListBox albums = sender as ListBox;
            if (albums.SelectedItem == null)
            return;

            // User has selected an album, so navigate to the PicturePresenter page.
            AlbumViewModel avm = albums.SelectedItem as AlbumViewModel;
            NavigationService.Navigate(new Uri(String.Format("/Views/AlbumView.xaml?albumname={0}", avm.Name), UriKind.Relative));
        }

        void BuildLocalizedApplicationBar()
        {
            // Set the page's ApplicationBar to a new instance of ApplicationBar.
            ApplicationBar = new ApplicationBar();

            ApplicationBar.BackgroundColor = Color.FromArgb(255, 127, 186, 0);
            ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.share.rest.png", UriKind.Relative));
            appBarButton.Text = Strings.Caption_AppBarConnect;
            appBarButton.Click += Share_Click;
            ApplicationBar.Buttons.Add(appBarButton);

            ApplicationBar.Mode = ApplicationBarMode.Minimized;
        }
    }
}
