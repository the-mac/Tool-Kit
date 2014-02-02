/* 
    Copyright (c) 2012 - 2013 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://code.msdn.microsoft.com/wpapps
  
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;


// Directives
using Microsoft.Devices;
using System.Windows.Media.Imaging;
using System.Threading;


namespace sdkCameraGrayscaleCS
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Variables
        PhotoCamera cam = new PhotoCamera();
        private static ManualResetEvent pauseFramesEvent = new ManualResetEvent(true);
        private WriteableBitmap wb;
        private Thread ARGBFramesThread;
        private bool pumpARGBFrames;


        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        //Code for camera initialization event, and setting the source for the viewfinder
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {

            // Check to see if the camera is available on the device.
            if ((PhotoCamera.IsCameraTypeSupported(CameraType.Primary) == true) ||
                 (PhotoCamera.IsCameraTypeSupported(CameraType.FrontFacing) == true))
            {
                // Initialize the default camera.
                cam = new Microsoft.Devices.PhotoCamera();

                //Event is fired when the PhotoCamera object has been initialized
                cam.Initialized += new EventHandler<Microsoft.Devices.CameraOperationCompletedEventArgs>(cam_Initialized);

                //Set the VideoBrush source to the camera
                viewfinderBrush.SetSource(cam);
            }
            else
            {
                // The camera is not supported on the device.
                this.Dispatcher.BeginInvoke(delegate()
                {
                    // Write message.
                    txtDebug.Text = "A Camera is not available on this device.";
                });

                // Disable UI.
                GrayscaleOnButton.IsEnabled = false;
                GrayscaleOffButton.IsEnabled = false;
            }
        }

        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            if (cam != null)
            {
                // Dispose of the camera to minimize power consumption and to expedite shutdown.
                cam.Dispose();

                // Release memory, ensure garbage collection.
                cam.Initialized -= cam_Initialized;
            }
        }

        //Update UI if initialization succeeds
        void cam_Initialized(object sender, Microsoft.Devices.CameraOperationCompletedEventArgs e)
        {
            if (e.Succeeded)
            {
                this.Dispatcher.BeginInvoke(delegate()
                {
                    txtDebug.Text = "Camera initialized";
                });

            }
        }

        // ARGB frame pump
        void PumpARGBFrames()
        {
            // Create capture buffer.
            int[] ARGBPx = new int[(int)cam.PreviewResolution.Width * (int)cam.PreviewResolution.Height];

            try
            {
                PhotoCamera phCam = (PhotoCamera)cam;

                while (pumpARGBFrames)
                {
                    pauseFramesEvent.WaitOne();

                    // Copies the current viewfinder frame into a buffer for further manipulation.
                    phCam.GetPreviewBufferArgb32(ARGBPx);

                    // Conversion to grayscale.
                    for (int i = 0; i < ARGBPx.Length; i++)
                    {
                        ARGBPx[i] = ColorToGray(ARGBPx[i]);
                    }

                    pauseFramesEvent.Reset();
                    Deployment.Current.Dispatcher.BeginInvoke(delegate()
                    {
                        // Copy to WriteableBitmap.
                        ARGBPx.CopyTo(wb.Pixels, 0);
                        wb.Invalidate();

                        pauseFramesEvent.Set();
                    });
                }

            }
            catch (Exception e)
            {
                this.Dispatcher.BeginInvoke(delegate()
                {
                    // Display error message.
                    txtDebug.Text = e.Message;
                });
            }
        }

        internal int ColorToGray(int color)
        {
            int gray = 0;

            int a = color >> 24;
            int r = (color & 0x00ff0000) >> 16;
            int g = (color & 0x0000ff00) >> 8;
            int b = (color & 0x000000ff);

            if ((r == g) && (g == b))
            {
                gray = color;
            }
            else
            {
                // Calculate for the illumination.
                // I =(int)(0.109375*R + 0.59375*G + 0.296875*B + 0.5)
                int i = (7 * r + 38 * g + 19 * b + 32) >> 6;

                gray = ((a & 0xFF) << 24) | ((i & 0xFF) << 16) | ((i & 0xFF) << 8) | (i & 0xFF);
            }
            return gray;
        }

        // Start ARGB to grayscale pump.
        private void GrayOn_Clicked(object sender, RoutedEventArgs e)
        {
            MainImage.Visibility = Visibility.Visible;
            pumpARGBFrames = true;
            ARGBFramesThread = new System.Threading.Thread(PumpARGBFrames);

            wb = new WriteableBitmap((int)cam.PreviewResolution.Width, (int)cam.PreviewResolution.Height);
            this.MainImage.Source = wb;

            // Start pump.
            ARGBFramesThread.Start();
            this.Dispatcher.BeginInvoke(delegate()
            {
                txtDebug.Text = "ARGB to Grayscale";
            });
        }

        // Stop ARGB to grayscale pump.
        private void GrayOff_Clicked(object sender, RoutedEventArgs e)
        {
            MainImage.Visibility = Visibility.Collapsed;
            pumpARGBFrames = false;

            this.Dispatcher.BeginInvoke(delegate()
            {
                txtDebug.Text = "";
            });
        }
    }
}
