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
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;

namespace PixPresenter
{
    public partial class AlbumView : ViewBase
    {
        private AlbumViewModel _album;
        public AlbumView()
        {
            InitializeComponent();

            // Create AppBar
            BuildLocalizedApplicationBar();
        }


        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if (e.NavigationMode == NavigationMode.Back)
                return;

            State["current_token"] = _album.CurrentPictureIndex;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
          
            // internal navigation or fast resume
            if (_album != null)
                return;

            if (this.NavigationContext.QueryString.ContainsKey("albumname"))
            {
                string albumName = NavigationContext.QueryString["albumname"];
                foreach (var album in App.AlbumsViewModel.Albums)
                {
                    if (album.Name == albumName)
                    {
                        _album = album;
                        break;
                    }
                }
            }
            else
            {
                return;
            }

            await _album.LoadPicturesAsync();
            this.DataContext = _album;
            object token;
          
            if (State.TryGetValue("current_token", out token))
            _album.RestoreCurrentPictureToken(token);
        }

        

        void Convert_Click(object sender, EventArgs e)
        {
            _album.CurrentPicture.ToGrayScale();
        }

        void ShareCurrentPicture()
        {
            if (!SharingViewModel.Instance.IsConnected)
            {
                // Let's assume the intention was to connect and send
                SharingViewModel.Instance.StartSharingSession();
            }
            else
            {
              SharingViewModel.Instance.SendPictureToPeer(_album.CurrentPicture.ImageBytes);
              ShowSendingArrow();
            }
        }


        void CurrentImage_ManipulationCompleted_1(object sender, System.Windows.Input.ManipulationCompletedEventArgs e)
        {
            if (Math.Abs(e.FinalVelocities.LinearVelocity.X) > Math.Abs(e.FinalVelocities.LinearVelocity.Y))
            {
                if (e.TotalManipulation.Translation.X > 10)
                {
                    Debug.WriteLine("Calling Previous...");
                    _album.GetPreviousPicture();
                }
                else if (e.TotalManipulation.Translation.X < -10)
                {
                    Debug.WriteLine("Calling Next...");
                    _album.GetNextPicture();
                }
            }
            else
            {
                // Flick UP is the gesture to send the current picture
                if (e.TotalManipulation.Translation.Y < -10 && !_sending)
                {
                    Debug.WriteLine("Send picture");
                    ShareCurrentPicture();
                }
            }
        }

        bool _sending = false;
        void ShowSendingArrow()
        {
            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            myDoubleAnimation.From = 0;
            myDoubleAnimation.To = 1;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            myDoubleAnimation.AutoReverse = true;

            // Apply the animation to the image's Opacity property
            Storyboard sb = new Storyboard();
            sb.Children.Add(myDoubleAnimation);
            Storyboard.SetTarget(myDoubleAnimation, SendingArrow);
            Storyboard.SetTargetProperty(myDoubleAnimation, new PropertyPath("Opacity"));
            sb.Completed += sb_Completed;
            _sending = true;
            sb.Begin();
        }

        void sb_Completed(object sender, EventArgs e)
        {
            _sending = false;
        }

        void BuildLocalizedApplicationBar()
        {
            // Set the page's ApplicationBar to a new instance of ApplicationBar.
            ApplicationBar = new ApplicationBar();
            ApplicationBar.BackgroundColor = Color.FromArgb(255, 127, 186, 0);
            ApplicationBar.Mode = ApplicationBarMode.Minimized;

            ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.edit.rest.png", UriKind.Relative));
            appBarButton.Text = Strings.Caption_AppBarConvert;
            appBarButton.Click += Convert_Click;
            ApplicationBar.Buttons.Add(appBarButton);

            appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.share.rest.png", UriKind.Relative));
            appBarButton.Text = Strings.Caption_AppBarConnect;
            appBarButton.Click += Share_Click;
            ApplicationBar.Buttons.Add(appBarButton);
        }
    }
}
