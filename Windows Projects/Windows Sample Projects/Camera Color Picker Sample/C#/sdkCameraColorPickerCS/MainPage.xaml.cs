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

using Microsoft.Devices;
using System.Threading;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using System.IO;
using Microsoft.Xna.Framework.Media;

namespace sdkCameraColorPickerCS
{
    public partial class MainPage : PhoneApplicationPage, INotifyPropertyChanged
    {
        PhotoCamera cam;
        Thread bgThread;
        volatile bool bgPleaseExit = false;
        private WriteableBitmap wbCbCrColorPlane = new WriteableBitmap(256, 256);
        public event PropertyChangedEventHandler PropertyChanged;

        // Internal YCbCr values used by the public properities.
        private byte Y { get; set; }
        private int Cb { get; set; }
        private int Cr { get; set; }

        // For binding from the color plane label.
        public string CbText
        {
            get { return string.Format("Cb = {0}", Cb); }
        }

        // For binding from the color plane label.
        public string CrText
        {
            get { return string.Format("Cr = {0}", Cr); }
        }

        // For binding the location of the Cb/Cr lines and Y arrow.
        public double CrOffset { get { return 255 - (Cr + 127); } }
        public double CbOffset { get { return Cb + 127; } }
        public double YOffset { get { return 255 - Y; } }

        // For binding from the ARGB text block.
        public string ArgbText
        {
            get { return "#" + YCbCrToArgb(Y, Cb, Cr).ToString("X"); }
        }

        // For binding from the rectangle that displays the crosshairs color.
        public Brush ArgbBrush
        {
            get
            {
                int argb = YCbCrToArgb(Y, Cb, Cr);
                int r = (argb >> 16) & 0xFF;
                int g = (argb >> 8) & 0xFF;
                int b = argb & 0xFF;

                return new SolidColorBrush(Color.FromArgb(255, (byte)r, (byte)g, (byte)b));
            }
        }

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Draw CbCr color plane bitmap.
            DrawCbCrColorPlaneBitmap();

            // Bind the UI to the this page.
            this.DataContext = this;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            // Check to see if the camera is available on the device.
            if ((PhotoCamera.IsCameraTypeSupported(CameraType.Primary) == true) ||
                 (PhotoCamera.IsCameraTypeSupported(CameraType.FrontFacing) == true))
            {
                cam = new PhotoCamera();
                cam.Initialized += new EventHandler<CameraOperationCompletedEventArgs>(cam_Initialized);
                viewfinderBrush.SetSource(cam);
            }
            else
            {
                MessageBox.Show("This sample requires a camera.");
                CameraCrosshairs.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            if (cam != null)
            {
                // Notify the background worker to stop processing.
                bgPleaseExit = true;
                bgThread.Join();

                // Dispose of the camera object to free memory.
                cam.Dispose();
            }
        }

        void cam_Initialized(object sender, CameraOperationCompletedEventArgs e)
        {
            if (cam != null)
            {
                Dispatcher.BeginInvoke(delegate()
                {
                    // Set the orientation of the viewfinder.
                    viewfinderBrushTransformation.Angle = cam.Orientation;
                });

                // Start the background worker thread that processes the camera preview buffer frames.
                bgPleaseExit = false;
                bgThread = new Thread(colorConversionBackgroundWorker);
                bgThread.Start();
            }
        }

        private void colorConversionBackgroundWorker()
        {
            // Grouping the property change notifications in a batch.
            List<PropertyChangedEventArgs> changeCache = new List<PropertyChangedEventArgs>();
            changeCache.Add(new PropertyChangedEventArgs("CbText"));
            changeCache.Add(new PropertyChangedEventArgs("CrText"));
            changeCache.Add(new PropertyChangedEventArgs("CrOffset"));
            changeCache.Add(new PropertyChangedEventArgs("CbOffset"));
            changeCache.Add(new PropertyChangedEventArgs("YOffset"));
            changeCache.Add(new PropertyChangedEventArgs("ArgbText"));
            changeCache.Add(new PropertyChangedEventArgs("ArgbBrush"));

            // Obtain the YCbCr layout settings used by the camera buffer.
            var bufferLayout = cam.YCbCrPixelLayout;

            // Allocate the appropriately sized preview buffer.
            byte[] currentPreviewBuffer = new byte[bufferLayout.RequiredBufferSize];

            // Continue processing until asked to stop in OnNavigatingFrom.
            while (!bgPleaseExit)
            {
                // Get the current preview buffer from the camera.
                cam.GetPreviewBufferYCbCr(currentPreviewBuffer);

                // The output parameters used in the following method.
                byte y;
                int cr;
                int cb;

                // Extract details about the pixel where the camera crosshairs meet.
                // This location is estimated to be X=320, Y=240. Adjust as desired.
                GetYCbCrFromPixel(bufferLayout, currentPreviewBuffer, 320, 240, out y, out cr, out cb);

                // Set page-level properties to the new YCbCr values.
                Y = y;
                Cb = cb;
                Cr = cr;

                Dispatcher.BeginInvoke(delegate()
                {
                    // not threadsafe, but unlikely to be a problem in this case

                    // Consolidating change notifications
                    if (PropertyChanged != null)
                    {
                        foreach (var change in changeCache)
                            PropertyChanged(this, change);
                    }
                });
            }
        }

        private void GetYCbCrFromPixel(YCbCrPixelLayout layout, byte[] currentPreviewBuffer, int xFramePos, int yFramePos, out byte y, out int cr, out int cb)
        {
            // Find the bytes corresponding to the pixel location in the frame.
            int yBufferIndex = layout.YOffset + yFramePos * layout.YPitch + xFramePos * layout.YXPitch;
            int crBufferIndex = layout.CrOffset + (yFramePos / 2) * layout.CrPitch + (xFramePos / 2) * layout.CrXPitch;
            int cbBufferIndex = layout.CbOffset + (yFramePos / 2) * layout.CbPitch + (xFramePos / 2) * layout.CbXPitch;

            // The luminance value is always positive.
            y = currentPreviewBuffer[yBufferIndex];

            // The preview buffer contains an unsigned value between 255 and 0.
            // The buffer value is cast from a byte to an integer.
            cr = currentPreviewBuffer[crBufferIndex];

            // Convert to a signed value between 127 and -128.
            cr -= 128;

            // The preview buffer contains an unsigned value between 255 and 0.
            // The buffer value is cast from a byte to an integer.
            cb = currentPreviewBuffer[cbBufferIndex];

            // Convert to a signed value between 127 and -128.
            cb -= 128;
        }

        private int YCbCrToArgb(byte y, int cb, int cr)
        {
            // Individual RGB components.
            int r, g, b;

            // Used for building a 32-bit ARGB pixel.
            uint argbPixel;

            // Assumes Cb & Cr have been converted to signed values (ranging from 127 to -128).

            // Integer-only division.
            r = y + cr + (cr >> 2) + (cr >> 3) + (cr >> 5);
            g = y - ((cb >> 2) + (cb >> 4) + (cb >> 5)) - ((cr >> 1) + (cr >> 3) + (cr >> 4) + (cr >> 5));
            b = y + cb + (cb >> 1) + (cb >> 2) + (cb >> 6);

            // Clamp values to 8-bit RGB range between 0 and 255.
            r = r <= 255 ? r : 255;
            r = r >= 0 ? r : 0;
            g = g <= 255 ? g : 255;
            g = g >= 0 ? g : 0;
            b = b <= 255 ? b : 255;
            b = b >= 0 ? b : 0;

            // Pack individual components into a single pixel.
            argbPixel = 0xff000000; // Alpha
            argbPixel |= (uint)b;
            argbPixel |= (uint)(g << 8);
            argbPixel |= (uint)(r << 16);

            // Return the ARGB pixel.
            return unchecked((int)argbPixel);
        }

        // Creates the CbCr color plane image used to display Cb and Cr values.
        // Uses a fixed Y value of 0.5.
        private void DrawCbCrColorPlaneBitmap()
        {
            // Generate CbCr color plane, with Y == 0.5
            int[] wb = wbCbCrColorPlane.Pixels;

            for (int x = 0; x < 255; x++)
            {
                for (int y = 0; y < 255; y++)
                {
                    int cb = x - 128;
                    int cr = (255 - y) - 128;

                    wb[y * 256 + x] = YCbCrToArgb(128, cb, cr);
                }
            }

            // Re-draw bitmap with new values.
            wbCbCrColorPlane.Invalidate();

            // Set bitmap to image control source.
            imgCbCrColorPlane.Source = wbCbCrColorPlane;
        }        
    }
}
