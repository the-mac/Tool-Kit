/****************************** Module Header ******************************\
* Module Name:  AudioPlayer.cs
* Project:      CSWP8BackgroundMusic
* Copyright (c) Microsoft Corporation
*
* This is the AudioPlayerAgent.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
****************************************************************************/

using System;
using System.Diagnostics;
using System.Windows;
using Microsoft.Phone.BackgroundAudio;
using System.Collections.Generic;

namespace MyAudioPlaybackAgent
{
    public class AudioPlayer : AudioPlayerAgent
    {
        // Flag for Initialized.
        private static volatile bool isInitialized;

        // Index of current track.
        static int currentTrackNumber = 0;

        // A playlist made up of AudioTrack items.
        private static List<AudioTrack> playList = new List<AudioTrack>();

        // Current AudioTrack.
        static AudioTrack currentTrack = null;

        /// <remarks>
        /// AudioPlayer instances can share the same process.
        /// Static fields can be used to share state between AudioPlayer instances
        /// or to communicate with the Audio Streaming agent.
        /// </remarks>
        static AudioPlayer()
        {
            # region Test data
            AudioTrack audioTrack = null;
            for (int i = 1; i < 3; i++)
            {               
                audioTrack = new AudioTrack(new Uri("Ring0" + i + ".wma", UriKind.Relative),
                                    "Ringtone " + i,
                                    "Windows Phone",
                                    "Windows Phone Ringtones",
                                    null);

                playList.Add(audioTrack);
            }

            // A remote URI
            //audioTrack = new AudioTrack(new Uri("http://traffic.libsyn.com/wpradio/WPRadio_29.mp3", UriKind.Absolute),
            //                "Episode 29",
            //                "Windows Phone Radio",
            //                "Windows Phone Radio Podcast",
            //                new Uri("shared/media/Episode29.jpg", UriKind.Relative));
            //playList.Add(audioTrack);
            # endregion

            // Check whether it has been initialized.
            if (!isInitialized)
            {
                isInitialized = true;

                // Subscribe to the managed exception handler
                Deployment.Current.Dispatcher.BeginInvoke(delegate
                {
                    Application.Current.UnhandledException += UnhandledException;
                });
            }
        }

        /// Code to execute on Unhandled Exceptions
        private static void UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                Debugger.Break();
            }
        }

        /// <summary>
        /// Called when the playstate changes, except for the Error state (see OnError)
        /// </summary>
        /// <param name="player">The BackgroundAudioPlayer</param>
        /// <param name="track">The track playing at the time the playstate changed</param>
        /// <param name="playState">The new playstate of the player</param>
        /// <remarks>
        /// Play State changes cannot be cancelled. They are raised even if the application
        /// caused the state change itself, assuming the application has opted-in to the callback.
        ///
        /// Notable playstate events:
        /// (a) TrackEnded: invoked when the player has no current track. The agent can set the next track.
        /// (b) TrackReady: an audio track has been set and it is now ready for playack.
        ///
        /// Call NotifyComplete() only once, after the agent request has been completed, including async callbacks.
        /// </remarks>
        protected override void OnPlayStateChanged(BackgroundAudioPlayer player, AudioTrack track, PlayState playState)
        {
            switch (playState)
            {
                case PlayState.TrackEnded:
                    // Set cycle.
                    SetStopToPlay(player);

                    // Set current Track.
                    currentTrack = GetNextTrack();

                    // Play Track.
                    PlayTrack(player);
                    break;
                case PlayState.TrackReady:
                    player.Play();
                    break;
                case PlayState.Shutdown:
                    // TODO: Handle the shutdown state here (e.g. save state)
                    break;
                case PlayState.Unknown:
                    break;
                case PlayState.Stopped:
                    // Set cycle.
                    SetStopToPlay(player);                  
                    break;
                case PlayState.Paused:
                    break;
                case PlayState.Playing:
                    break;
                case PlayState.BufferingStarted:
                    break;
                case PlayState.BufferingStopped:
                    break;
                case PlayState.Rewinding:
                    break;
                case PlayState.FastForwarding:
                    break;
            }

            NotifyComplete();
        }

        /// <summary>
        /// Called when the user requests an action using application/system provided UI
        /// </summary>
        /// <param name="player">The BackgroundAudioPlayer</param>
        /// <param name="track">The track playing at the time of the user action</param>
        /// <param name="action">The action the user has requested</param>
        /// <param name="param">The data associated with the requested action.
        /// In the current version this parameter is only for use with the Seek action,
        /// to indicate the requested position of an audio track</param>
        /// <remarks>
        /// User actions do not automatically make any changes in system state; the agent is responsible
        /// for carrying out the user actions if they are supported.
        ///
        /// Call NotifyComplete() only once, after the agent request has been completed, including async callbacks.
        /// </remarks>
        protected override void OnUserAction(BackgroundAudioPlayer player, AudioTrack track, UserAction action, object param)
        {
            switch (action)
            {
                case UserAction.Play:
                    // Set current Track.
                    currentTrack = playList[currentTrackNumber];

                    // Play Track.
                    PlayTrack(player);
                    break;
                case UserAction.Stop:
                    player.Stop();
                    break;
                case UserAction.Pause:
                    player.Pause();
                    break;
                case UserAction.FastForward:
                    player.FastForward();
                    break;
                case UserAction.Rewind:
                    player.Rewind();
                    break;
                case UserAction.Seek:
                    player.Position = (TimeSpan)param;
                    break;
                case UserAction.SkipNext:
                    // Set current Track.
                    currentTrack = GetNextTrack();

                    // Play Track.
                    PlayTrack(player);
                    break;
                case UserAction.SkipPrevious:
                    // Set current Track.
                    currentTrack = GetPreviousTrack();

                    // Play Track.
                    PlayTrack(player);
                    break;
            }

            NotifyComplete();
        }

        /// <summary>
        /// Implements the logic to get the next AudioTrack instance.
        /// In a playlist, the source can be from a file, a web request, etc.
        /// </summary>
        /// <remarks>
        /// The AudioTrack URI determines the source, which can be:
        /// (a) Isolated-storage file (Relative URI, represents path in the isolated storage)
        /// (b) HTTP URL (absolute URI)
        /// (c) MediaStreamSource (null)
        /// </remarks>
        /// <returns>an instance of AudioTrack, or null if the playback is completed</returns>
        private AudioTrack GetNextTrack()
        {
            AudioTrack track = null;

            // Specify the Track.
            if (++currentTrackNumber >= playList.Count)
            {
                currentTrackNumber = 0;
            }

            track = playList[currentTrackNumber];
            return track;
        }

        /// <summary>
        /// Implements the logic to get the previous AudioTrack instance.
        /// </summary>
        /// <remarks>
        /// The AudioTrack URI determines the source, which can be:
        /// (a) Isolated-storage file (Relative URI, represents path in the isolated storage)
        /// (b) HTTP URL (absolute URI)
        /// (c) MediaStreamSource (null)
        /// </remarks>
        /// <returns>an instance of AudioTrack, or null if previous track is not allowed</returns>
        private AudioTrack GetPreviousTrack()
        {
            AudioTrack track = null;

            // Specify the Track.
            if (--currentTrackNumber < 0)
            {
                currentTrackNumber = playList.Count - 1;
            }

            track = playList[currentTrackNumber];
            return track;
        }

        /// <summary>
        /// Called whenever there is an error with playback, such as an AudioTrack not downloading correctly
        /// </summary>
        /// <param name="player">The BackgroundAudioPlayer</param>
        /// <param name="track">The track that had the error</param>
        /// <param name="error">The error that occured</param>
        /// <param name="isFatal">If true, playback cannot continue and playback of the track will stop</param>
        /// <remarks>
        /// This method is not guaranteed to be called in all cases. For example, if the background agent
        /// itself has an unhandled exception, it won't get called back to handle its own errors.
        /// </remarks>
        protected override void OnError(BackgroundAudioPlayer player, AudioTrack track, Exception error, bool isFatal)
        {
            if (isFatal)
            {
                Abort();
            }
            else
            {
                NotifyComplete();
            }

        }

        /// <summary>
        /// Called when the agent request is getting cancelled
        /// </summary>
        /// <remarks>
        /// Once the request is Cancelled, the agent gets 5 seconds to finish its work,
        /// by calling NotifyComplete()/Abort().
        /// </remarks>
        protected override void OnCancel()
        {

        }

        /// <summary>
        /// Play Trace
        /// </summary>
        /// <param name="player">The BackgroundAudioPlayer</param>
        private static void PlayTrack(BackgroundAudioPlayer player)
        {
            if (currentTrack != null)
            {
                if (player.PlayerState == PlayState.Paused)
                {
                    player.Play();
                }
                else
                {
                    player.Track = currentTrack;
                }
            }
        }

        /// <summary>  
        /// This method will achieve loop playback.
        /// Check the Current State of the BackgroundAudioPlayer before allowing Play to be called.
        /// And Plays the track in our playlist at the currentTrackNumber position.
        /// </summary>
        /// <param name="player">The BackgroundAudioPlayer</param>      
        private static void SetStopToPlay(BackgroundAudioPlayer player)
        {
            if (player.PlayerState != PlayState.Playing)
            {
                player.Stop();
                player.Play();
            }
        }
    }
}