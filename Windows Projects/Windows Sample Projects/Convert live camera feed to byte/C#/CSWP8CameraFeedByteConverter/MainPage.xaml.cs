/****************************** Module Header ******************************\
* Module Name:    MainPage.xaml.cs
* Project:        CSWP8CameraFeedByteConverter
* Copyright (c) Microsoft Corporation
*
* This sample will demo how to convert live camera feed from Windows Phone
* to byte[] and then from byte[] to VideoBrush for rendering.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\*****************************************************************************/

using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Windows.Media;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
namespace CSWP8CameraFeedByteConverter
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Store video data for transfer.
        byte[] data;
        // Use this video to initialize the VideoBrush which will be displayed the video from byte.
        string strLocalName = "howto.wmv";
        // Byte-generated video.
        public string strImageName = "test.wmv";

        // Viewfinder for capturing video.
        private VideoBrush videoRecorderBrush;

        // Source and device for capturing video.
        private CaptureSource captureSource;
        private VideoCaptureDevice videoCaptureDevice;

        // File details for storing the recording.        
        private IsolatedStorageFileStream isoVideoFile;
        private FileSink fileSink;
        private string isoVideoFileName = "CameraMovie.mp4";

        // For managing button and application state.
        private enum ButtonState { Initialized, Ready, Recording, Playback, Paused, NoChange, CameraNotSupported };
        private ButtonState currentAppState;

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            // Initialize the VideoBrush which will be displayed the video from byte.
            myMediaElement.Source = new Uri(strLocalName, UriKind.Relative);
            // Prepare ApplicationBar and buttons.
            PhoneAppBar = (ApplicationBar)ApplicationBar;
            PhoneAppBar.IsVisible = true;
            StartRecording = ((ApplicationBarIconButton)ApplicationBar.Buttons[0]);
            StopPlaybackRecording = ((ApplicationBarIconButton)ApplicationBar.Buttons[1]);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Initialize the video recorder.
            InitializeVideoRecorder();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // Dispose of camera and media objects.
            DisposeVideoPlayer();
            DisposeVideoRecorder();

            base.OnNavigatedFrom(e);
        }

        /// <summary>
        /// Update the buttons and text on the UI thread based on app state.
        /// </summary>
        /// <param name="currentButtonState"></param>
        /// <param name="statusMessage"></param>
        private void UpdateUI(ButtonState currentButtonState, string statusMessage)
        {
            // Run code on the UI thread.
            Dispatcher.BeginInvoke(delegate
            {
                switch (currentButtonState)
                {
                    // When the camera is not supported by the device.
                    case ButtonState.CameraNotSupported:
                        StartRecording.IsEnabled = false;
                        StopPlaybackRecording.IsEnabled = false;
                        break;
                    // First launch of the application, so no video is available.
                    case ButtonState.Initialized:
                        StartRecording.IsEnabled = true;
                        StopPlaybackRecording.IsEnabled = false;
                        break;
                    // Ready to record, so video is available for viewing.
                    case ButtonState.Ready:
                        StartRecording.IsEnabled = true;
                        StopPlaybackRecording.IsEnabled = false;
                        break;
                    // Video recording is in progress.
                    case ButtonState.Recording:
                        StartRecording.IsEnabled = false;
                        StopPlaybackRecording.IsEnabled = true;
                        break;
                    // Video playback is in progress.
                    case ButtonState.Playback:
                        StartRecording.IsEnabled = false;
                        StopPlaybackRecording.IsEnabled = true;
                        break;
                    // Video playback has been paused.
                    case ButtonState.Paused:
                        StartRecording.IsEnabled = false;
                        StopPlaybackRecording.IsEnabled = true;
                        break;
                    default:
                        break;
                }

                // Display a message.
                txtDebug.Text = statusMessage;

                // Note the current application state.
                currentAppState = currentButtonState;
            });
        }

        /// <summary>
        /// Initialize the video recorder.
        /// </summary>
        public void InitializeVideoRecorder()
        {
            if (captureSource == null)
            {
                // Create the VideoRecorder objects.
                captureSource = new CaptureSource();
                fileSink = new FileSink();

                videoCaptureDevice = CaptureDeviceConfiguration.GetDefaultVideoCaptureDevice();

                // Add eventhandlers for captureSource.
                captureSource.CaptureFailed += new EventHandler<ExceptionRoutedEventArgs>(OnCaptureFailed);

                // Initialize the camera if it exists on the device.
                if (videoCaptureDevice != null)
                {
                    // Create the VideoBrush for the viewfinder.
                    videoRecorderBrush = new VideoBrush();
                    videoRecorderBrush.SetSource(captureSource);

                    // Display the viewfinder image on the rectangle.
                    viewfinderRectangle.Fill = videoRecorderBrush;

                    // Start video capture and display it on the viewfinder.
                    captureSource.Start();

                    // Set the button state and the message.
                    UpdateUI(ButtonState.Initialized, "Tap record to start recording...");
                }
                else
                {
                    // Disable buttons when the camera is not supported by the device.
                    UpdateUI(ButtonState.CameraNotSupported, "A camera is not supported on this device.");
                }
            }
        }

        /// <summary>
        /// Set recording state: start recording.
        /// </summary> 
        private void StartVideoRecording()
        {
            try
            {
                // Connect fileSink to captureSource.
                if (captureSource.VideoCaptureDevice != null
                    && captureSource.State == CaptureState.Started)
                {
                    captureSource.Stop();

                    // Connect the input and output of fileSink.
                    fileSink.CaptureSource = captureSource;
                    fileSink.IsolatedStorageFileName = isoVideoFileName;
                }

                // Begin recording.
                if (captureSource.VideoCaptureDevice != null
                    && captureSource.State == CaptureState.Stopped)
                {
                    captureSource.Start();
                }

                // Set the button states and the message.
                UpdateUI(ButtonState.Recording, "Recording...");
            }

            // If recording fails, display an error.
            catch (Exception e)
            {
                this.Dispatcher.BeginInvoke(delegate()
                {
                    txtDebug.Text = "ERROR: " + e.Message.ToString();
                });
            }
        }

        /// <summary>
        /// Set the recording state: stop recording.
        /// </summary> 
        private void StopVideoRecording()
        {
            try
            {
                // Stop recording.
                if (captureSource.VideoCaptureDevice != null
                && captureSource.State == CaptureState.Started)
                {
                    captureSource.Stop();

                    isoVideoFile = new IsolatedStorageFileStream(isoVideoFileName,
                                      FileMode.Open, FileAccess.Read,
                                      IsolatedStorageFile.GetUserStoreForApplication());

                    data = ReadFully(isoVideoFile);

                    // Disconnect fileSink.
                    fileSink.CaptureSource = null;
                    fileSink.IsolatedStorageFileName = null;

                    // Set the button states and the message.
                    UpdateUI(ButtonState.NoChange, "Preparing viewfinder...");

                    StartVideoPreview();
                }
            }
            // If stop fails, display an error.
            catch (Exception e)
            {
                this.Dispatcher.BeginInvoke(delegate()
                {
                    txtDebug.Text = "ERROR: " + e.Message.ToString();
                });
            }
        }

        /// <summary>
        /// Set the recording state: display the video on the viewfinder.
        /// </summary>
        private void StartVideoPreview()
        {
            try
            {
                // Display the video on the viewfinder.
                if (captureSource.VideoCaptureDevice != null
                && captureSource.State == CaptureState.Stopped)
                {
                    // Add captureSource to videoBrush.
                    videoRecorderBrush.SetSource(captureSource);

                    // Add videoBrush to the visual tree.
                    viewfinderRectangle.Fill = videoRecorderBrush;

                    captureSource.Start();

                    // Set the button states and the message.
                    UpdateUI(ButtonState.Ready, "Ready to record.");
                }
            }
            // If preview fails, display an error.
            catch (Exception e)
            {
                this.Dispatcher.BeginInvoke(delegate()
                {
                    txtDebug.Text = "ERROR: " + e.Message.ToString();
                });
            }
        }

        /// <summary>
        /// Start the video recording.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartRecording_Click(object sender, EventArgs e)
        {
            // Avoid duplicate taps.
            StartRecording.IsEnabled = false;

            StartVideoRecording();
        }

        /// <summary>
        /// Handle stop requests.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StopPlaybackRecording_Click(object sender, EventArgs e)
        {
            // Avoid duplicate taps.
            StopPlaybackRecording.IsEnabled = false;

            // Stop during video recording.
            if (currentAppState == ButtonState.Recording)
            {
                StopVideoRecording();

                // Set the button state and the message.
                UpdateUI(ButtonState.NoChange, "Recording stopped.");
            }

            // Stop during video playback.
            else
            {
                // Remove playback objects.
                DisposeVideoPlayer();

                StartVideoPreview();

                // Set the button state and the message.
                UpdateUI(ButtonState.NoChange, "Playback stopped.");
            }
        }

        /// <summary>
        /// Remove playback objects.
        /// </summary>
        private void DisposeVideoPlayer()
        {
            if (VideoPlayer != null)
            {
                // Stop the VideoPlayer MediaElement.
                VideoPlayer.Stop();

                // Remove playback objects.
                VideoPlayer.Source = null;
                isoVideoFile = null;

                // Remove the event handler.
                VideoPlayer.MediaEnded -= VideoPlayerMediaEnded;
            }
        }

        private void DisposeVideoRecorder()
        {
            if (captureSource != null)
            {
                // Stop captureSource if it is running.
                if (captureSource.VideoCaptureDevice != null
                    && captureSource.State == CaptureState.Started)
                {
                    captureSource.Stop();
                }

                // Remove the event handlers for capturesource and the shutter button.
                captureSource.CaptureFailed -= OnCaptureFailed;

                // Remove the video recording objects.
                captureSource = null;
                videoCaptureDevice = null;
                fileSink = null;
                videoRecorderBrush = null;
            }
        }

        /// <summary>
        /// If recording fails, display an error message.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param> 
        private void OnCaptureFailed(object sender, ExceptionRoutedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(delegate()
            {
                txtDebug.Text = "ERROR: " + e.ErrorException.Message.ToString();
            });
        }

        /// <summary>
        /// Display the viewfinder when playback ends.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void VideoPlayerMediaEnded(object sender, RoutedEventArgs e)
        {
            // Remove the playback objects.
            DisposeVideoPlayer();

            StartVideoPreview();
        }

        /// <summary>
        /// Stream to byte.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[input.Length];

            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Handle btnUse's click event. We will use the acquired data to create a video
        /// then associated it with VideoBrush.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUse_Click(object sender, RoutedEventArgs e)
        {
            using (IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (iso.FileExists(strImageName))
                {
                    iso.DeleteFile(strImageName);
                }

                using (IsolatedStorageFileStream isostream = iso.CreateFile(strImageName))
                {
                    isostream.Write(data, 0, data.Length);
                    isostream.Close();
                }
            }

            // Create the file stream and attach it to the MediaElement.
            isoVideoFile = new IsolatedStorageFileStream(strImageName,
                                       FileMode.Open, FileAccess.Read,
                                       IsolatedStorageFile.GetUserStoreForApplication());

            myMediaElement.SetSource(isoVideoFile);
        }
    }
}