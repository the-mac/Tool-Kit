/* 
    Copyright (c) 2011 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Threading;
using Microsoft.Devices;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;

namespace sdkMusicVideosHubCS
{
    public partial class MainPage : PhoneApplicationPage
    {
        #region MemberVariables

        Song _playingSong;                          // The song that is currently playing or will play when the user presses the Play button.
        bool _historyItemLaunch;                    // Indicates if the app was launched from a MediaHistoryItem.
        const String _playSongKey = "playSong";     // Key for MediaHistoryItem key-value pair.

        #endregion MemberVariables


        // Constructor
        /// <summary>
        /// Initializes member variables and sets up a DispatcherTimer 
        /// to run XNA internals because MediaPlayer is from XNA.
        /// </summary>
        public MainPage()
        {
            _playingSong = null;
            _historyItemLaunch = false;

            InitializeComponent();

            // Timer to run the XNA internals (MediaPlayer is from XNA)
            DispatcherTimer dt = new DispatcherTimer();
            dt.Interval = TimeSpan.FromMilliseconds(33);
            dt.Tick += delegate { try { FrameworkDispatcher.Update(); } catch { } };
            dt.Start();

            // Event handler to manage button states based on whether or not a song in playing.
            MediaPlayer.MediaStateChanged += new EventHandler<EventArgs>(MediaPlayer_MediaStateChanged);
        }


        #region EventHandlers

        /// <summary>
        /// Sets the _playingSong member variable based on how the app was started.
        /// If a song was already playing in the media player, set _playingSong to the currently active song.
        /// If the app was started from a history item, set _playingSong using the data from the history token.
        /// If no song was playing, find a random song in the media library and set _playingSong to that song.
        /// </summary>
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            MediaLibrary library = new MediaLibrary();

            if (NavigationContext.QueryString.ContainsKey(_playSongKey))
            {
                // We were launched from a history item.
                // Change _playingSong even if something was already playing 
                // because the user directly chose a song history item.

                // Use the navigation context to find the song by name.
                String songToPlay = NavigationContext.QueryString[_playSongKey];

                foreach (Song song in library.Songs)
                {
                    if (0 == String.Compare(songToPlay, song.Name))
                    {
                        _playingSong = song;
                        break;
                    }
                }

                // Set a flag to indicate that we were started from a 
                // history item and that we should immediately start 
                // playing the song once the UI has finished loading.
                _historyItemLaunch = true;
            }
            else if (MediaPlayer.State == MediaState.Playing)
            {
                // A song was already playing when we started.
                _playingSong = MediaPlayer.Queue.ActiveSong;
            }
            else
            {
                // We were launched with no NavigationContext and 
                // there was not a song already playing, so choose 
                // a random song from the library on this device.
                Random rand = new Random();

                if (library.Songs.Count > 0)
                {
                    _playingSong = library.Songs[rand.Next(library.Songs.Count)];
                }
                else
                {
                    // Song library is empty. 
                    SongName.Text = "no songs in library";
                    PlayButton.IsEnabled = false;
                }
            }

            base.OnNavigatedTo(e);
        }

        /// <summary>
        /// Called once the UI has finished loading. Now it is safe to 
        /// initialize the UI elements and to start playing a song, if 
        /// we were launched from a history item.
        /// </summary>
        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            // Set the album art and song name.
            PopulateSongMetadata();

            // Set the IsEnabled state of the buttons.
            SetInitialButtonStates();

            if (_historyItemLaunch)
            {
                // We were launched from a history item, 
                // start playing the song immediately.
                PlaySong();
            }
        }

        /// <summary>
        /// Start playing the song and add a history item.
        /// </summary>
        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            PlaySong();
            AddToHistory();
        }

        /// <summary>
        /// Stop playing the song.
        /// </summary>
        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            StopPlayingSong();
        }

        /// <summary>
        /// Sets the state of the Play and Stop buttons based on the state of the MediaPlayer.
        /// </summary>
        void MediaPlayer_MediaStateChanged(object sender, EventArgs e)
        {
            switch (MediaPlayer.State)
            {
                case MediaState.Playing:
                    PlayButton.IsEnabled = false;
                    StopButton.IsEnabled = true;
                    break;

                case MediaState.Stopped:
                case MediaState.Paused:
                    PlayButton.IsEnabled = true;
                    StopButton.IsEnabled = false;
                    break;
            }
        }

        #endregion EventHandlers


        #region MediaPlayer

        /// <summary>
        /// Starts playing the song and sets the button states 
        /// to allow the user to stop playing the song.
        /// </summary>
        private void PlaySong()
        {
            if (_playingSong != null)
            {
                MediaPlayer.Play(_playingSong);
            }
        }

        /// <summary>
        /// Stops playing the song and sets the button states 
        /// to allow the user to start playback again.
        /// </summary>
        private void StopPlayingSong()
        {
            if (MediaPlayer.State == MediaState.Playing)
            {
                MediaPlayer.Stop();
            }
        }

        #endregion MediaPlayer


        /// <summary>
        /// Sets the album art and song name in the corresponding UI elements.
        /// </summary>
        private void PopulateSongMetadata()
        {
            if (_playingSong != null)
            {
                // Initialize the SongName TextBlock found in the XAML file.
                SongName.Text = _playingSong.Name;

                // Try to get the album art.
                // NOTE! You cannot debug this application while the Zune software 
                // is running because the media database is locked by Zune. You will
                // get an InvalidOperationException here if the Zune software is running.
                Stream albumArtStream = _playingSong.Album.GetAlbumArt();

                if (albumArtStream == null)
                {
                    // No album art found, use a generic place holder image.
                    StreamResourceInfo albumArtPlaceholder = Application.GetResourceStream(new Uri("AlbumArtPlaceholder.png", UriKind.Relative));
                    albumArtStream = albumArtPlaceholder.Stream;
                }

                // Initialize the Image element named 
                // SongAblbumArtImage in the XAML file.
                BitmapImage albumArtImage = new BitmapImage();
                albumArtImage.SetSource(albumArtStream);
                SongAlbumArtImage.Source = albumArtImage;
            }
        }

        /// <summary>
        /// Sets the initial state of the Play and Stop buttons.
        /// </summary>
        private void SetInitialButtonStates()
        {
            // Initialize buttons because the MediaStateChanged 
            // event will not occur if we are already playing.
            switch (MediaPlayer.State)
            {
                case MediaState.Playing:
                    PlayButton.IsEnabled = false;
                    StopButton.IsEnabled = true;
                    break;

                case MediaState.Stopped:
                case MediaState.Paused:
                    PlayButton.IsEnabled = true;
                    StopButton.IsEnabled = false;
                    break;
            }
        }

        /// <summary>
        /// Creates a MediaHistoryItem for the song we are playing and 
        /// adds it to the history area of the Music + Videos Hub.
        /// </summary>
        private void AddToHistory()
        {
            if (_playingSong != null)
            {
                MediaHistoryItem historyItem = new MediaHistoryItem();
                historyItem.Title = _playingSong.Name;
                historyItem.Source = "";

                // TODO: Use a more unique image here that better identifies 
                // the history item as having come from this app.
                historyItem.ImageStream = _playingSong.Album.GetThumbnail();

                if (historyItem.ImageStream == null)
                {
                    // No album art found, use a generic place holder image.
                    StreamResourceInfo sri = Application.GetResourceStream(new Uri("AlbumThumbnailPlaceholder.png", UriKind.Relative));
                    historyItem.ImageStream = sri.Stream;
                }

                // If we get activated by the MediaHistoryItem we're creating here, 
                // our NavigationContext will have a key-value pair ("playSong", "<Song Name>")
                // where <Song Name> is the Name property of the _playingSong object.
                historyItem.PlayerContext[_playSongKey] = _playingSong.Name;

                // Add our item to the MediaHistory area of the Music + Videos Hub.
                MediaHistory mediaHistory = MediaHistory.Instance;
                mediaHistory.WriteRecentPlay(historyItem);
            }
        }
    }
}
