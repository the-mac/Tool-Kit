/* 
    Copyright (c) 2012 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
        To see all Code Samples for Windows Store apps, visit http://code.msdn.microsoft.com/windowsapps.
  
*/
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using PixPresenter.ViewModels;
using PixPresenterPortableLib;

namespace PixPresenter.Views
{
    public sealed partial class AlbumView : ViewBase
    {
        public AlbumView()
        {
            this.InitializeComponent();
        }

        private AlbumViewModel _album;
        private ObservableCollection<PictureViewModel> _pictureCarousel = new ObservableCollection<PictureViewModel>();

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                if (e.Parameter.GetType() == typeof(string))
                {
                    this.AlbumName = e.Parameter.ToString();

                    _album = App.AlbumsViewModel.Albums.First((a) => a.Name == this.AlbumName);

                    // Load the pictures from the album
                    await _album.LoadPicturesAsync();

                    if (_album.Pictures.Count == 0)
                      return;

                    this.DataContext = _album;

                    // Prepare data for carousel view. This creates a copy of the 
                    // pictures in an album, adds the first picture to the end of the list,
                    // and the last picture to the beginning of the list. The selected index
                    // is adjusted in the FlipView.SelectionChanged event to simulate a
                    // "carousel" view of the pictures.
                    var firstPicture = _album.Pictures[0];
                    var lastPicture = _album.Pictures.Last();

                    _pictureCarousel.Add(lastPicture);
                    foreach (var p in _album.Pictures) { _pictureCarousel.Add(p); }
                    _pictureCarousel.Add(firstPicture);

                    picturesViewSource.Source = _pictureCarousel;
                }
            }

            base.OnNavigatedTo(e);
        }

        // Navigate through the pictures in "carousel" form
        void carouselFlipView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (carouselFlipView.SelectedIndex == 0) { carouselFlipView.SelectedIndex = _pictureCarousel.Count - 2; }
            if (carouselFlipView.SelectedIndex == _pictureCarousel.Count - 1) { carouselFlipView.SelectedIndex = 1; }

            _album.CurrentPictureIndex = carouselFlipView.SelectedIndex - 1;
        }


        // Navigate back to albums view
        void backButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(AlbumsView));
        }


        // Flick to send
        void innerFlipView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FlipView innerView = (FlipView)sender;

            var image = innerView.FindName("CurrentImage") as Image;

            // send the picture for the vertical "flick" gesture
            if (SharingViewModel.Instance.IsConnected && innerView.SelectedIndex == 1)
                SharingViewModel.Instance.SendPictureToPeer(_album.CurrentPicture.ImageBytes);

            innerView.SelectedIndex = 0; 
        }

        void convertButton_Click(object sender, RoutedEventArgs e)
        {
            _album.CurrentPicture.ToGrayScale();
            bottomAppBar.IsOpen = false;
        }
    }
}
