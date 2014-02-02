/* 
    Copyright (c) 2012 - 2013 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://code.msdn.microsoft.com/wpapps
  
*/
using Microsoft.Phone.Info;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media.Imaging;

namespace sdkImages.Scenarios
{
    public class DownsampleViewModel : INotifyPropertyChanged
    {
        // Image dimensions are fixed based on the dimensions of test.jpg.
        // If you use another image, make sure to update these values.
        const int TEST_IMAGE_HEIGHT = 2552;
        const int TEST_IMAGE_WIDTH = 3888;
        const int BYTES_PER_MB = 1048576;
        const string TEST_IMAGE_RELATIVE_PATH = "/Assets/test.jpg";
        const int DOWNSAMPLE_INCREMENT_PERCENT = 30; // The percentage by which the DecodePixelWidth and DecodePixelHeight are changed when the user taps on the "+" and "-" buttons
        const int DOWNSAMPLE_MIN_PERCENT = 1; // The minimum downsampling percentage

        double _currentDPHPercent = 100;
        double _currentDPWPercent = 100;

        long _beforeMB = 0; // The app memory usage before the current downsample change is made.
        long _afterMB = 0;  // The app memory usage after the current downsample change has been made.

        internal DownsampleViewModel()
        {
            LoadTestImage(false);
        }

        private void LoadTestImage(bool refresh)
        {
            // Store the value of current memory usage of the app, before this update is  made.
            MemBeforeMB = DeviceStatus.ApplicationCurrentMemoryUsage / BYTES_PER_MB;
            if (refresh)
            {
                CurrentBitmap.UriSource = null;
                CurrentBitmap.DecodePixelHeight = (int)this.DecodeHeight;
                CurrentBitmap.DecodePixelWidth = (int)this.DecodeWidth;
                CurrentBitmap.UriSource = new Uri(TEST_IMAGE_RELATIVE_PATH, UriKind.RelativeOrAbsolute);
            }
            else
            {
                // First load of the image.
                _currentBitmap = new BitmapImage();

                _currentBitmap.ImageOpened += bmi_ImageOpened;
                _currentBitmap.ImageFailed += bmi_ImageFailed;
                _currentBitmap.UriSource = new Uri(TEST_IMAGE_RELATIVE_PATH, UriKind.RelativeOrAbsolute);
            }
        }

        void bmi_ImageOpened(object sender, RoutedEventArgs e)
        {
            // If I haven't initialized the CurrentBitmap object, do that now
            RaisePropertyChanged("CurrentBitmap");

            // Measure the app's current memory usage as soon as the image has been loaded.
            // Value is in bytes, but is converted to MB to make it more readable.
            MemAfterMB = DeviceStatus.ApplicationCurrentMemoryUsage / BYTES_PER_MB;
        }

        internal void IncrementDecodePixelHeight()
        {
            // Only increment the percent by which we increase the DecodePixelHeight if it is not already at the max (100)
            _currentDPHPercent = (_currentDPHPercent == 100 || _currentDPHPercent + DOWNSAMPLE_INCREMENT_PERCENT >= 100) 
                                    ? 100 : _currentDPHPercent + DOWNSAMPLE_INCREMENT_PERCENT;
            UpdateDecodeHeight();
        }

        internal void DecrementDecodePixelHeight()
        {
            // Only decrement the percent by which we decrease the DecodePixelHeight if it is not already at the min (DOWNSAMPLE_MIN_PERCENT)
            _currentDPHPercent = (_currentDPHPercent == DOWNSAMPLE_MIN_PERCENT || _currentDPHPercent - DOWNSAMPLE_INCREMENT_PERCENT <= DOWNSAMPLE_MIN_PERCENT) 
                                    ? DOWNSAMPLE_MIN_PERCENT : _currentDPHPercent - DOWNSAMPLE_INCREMENT_PERCENT;
            UpdateDecodeHeight();
        }

        internal void IncrementDecodePixelWidth()
        {
            // Only increment the percent by which we increase the DecodePixelWidth if it is not already at the max (100)
            _currentDPWPercent = (_currentDPWPercent == 100 || _currentDPWPercent + DOWNSAMPLE_INCREMENT_PERCENT >= 100) 
                                    ? 100 : _currentDPWPercent + DOWNSAMPLE_INCREMENT_PERCENT;
            UpdateDecodeWidth();
        }

        internal void DecrementDecodePixelWidth()
        {
            // Only decrement the percent by which we decrease the DecodePixelWidth if it is not already at the min (DOWNSAMPLE_MIN_PERCENT)
            _currentDPWPercent = (_currentDPWPercent == DOWNSAMPLE_MIN_PERCENT || _currentDPWPercent - DOWNSAMPLE_INCREMENT_PERCENT <= DOWNSAMPLE_MIN_PERCENT) 
                                    ? DOWNSAMPLE_MIN_PERCENT : _currentDPWPercent - DOWNSAMPLE_INCREMENT_PERCENT;
            UpdateDecodeWidth();
        }

        private void UpdateDecodeHeight()
        {
            // The DecodePixelHeight of the Bitmap image is modified by the current percentage value of the slider
            this.DecodeHeight = (int)(TEST_IMAGE_HEIGHT * (_currentDPHPercent / 100));

            // Keep DecodePixelWidth in sync if we are maintaining aspect ratio
            if (_maintainAspectRatio)
            {
                _currentDPWPercent = _currentDPHPercent;
                this.DecodeWidth = (int)(TEST_IMAGE_WIDTH * (_currentDPWPercent / 100));
            }

            LoadTestImage(true);
        }

        private void UpdateDecodeWidth()
        {
            // The DecodePixelWidth of the Bitmap image is modified by the current percentage value of the slider
            this.DecodeWidth = (int)(TEST_IMAGE_WIDTH * (_currentDPWPercent / 100));

            // Keep DecodePixelHeight in sync if we are maintaining aspect ratio
            if (_maintainAspectRatio)
            {
                _currentDPHPercent = _currentDPWPercent;
                this.DecodeHeight = (int)(TEST_IMAGE_HEIGHT * (_currentDPHPercent / 100));
            }

            LoadTestImage(true);
        }

        void bmi_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            // Log to the output window if something goes wrong while loading the image
            Debug.WriteLine(e.ErrorException.Message);
        }

        private BitmapImage _currentBitmap = null;

        /// <summary>
        /// The Source for the TestImage.
        /// </summary>
        public BitmapImage CurrentBitmap
        {
            get
            {
                return _currentBitmap;
            }
            set
            {
                if (_currentBitmap != value)
                {
                    _currentBitmap = value;
                    RaisePropertyChanged();
                }
            }
        }

        private int _decodeHeight = TEST_IMAGE_HEIGHT;

        /// <summary>
        /// The DecodePixelHeight value to set on the TestImage.
        /// </summary>
        public int DecodeHeight
        {
            get
            {
                return _decodeHeight;
            }
            set
            {
                if (value != _decodeHeight)
                {
                    Debug.WriteLine("Decode Height: {0}", value);
                    _decodeHeight = value;
                    RaisePropertyChanged();
                }
            }
        }

        private int _decodeWidth = TEST_IMAGE_WIDTH;

        /// <summary>
        /// The DecodePixelWidth value to set on the TestImage.
        /// </summary>
        public int DecodeWidth
        {
            get
            {
                return _decodeWidth;
            }
            set
            {
                if (value != _decodeWidth)
                {
                    Debug.WriteLine("Decode Width: {0}", value);
                    _decodeWidth = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// The app memory usage, in megabytes, before the current downsample image is applied.
        /// </summary>
        public long MemBeforeMB
        {
            get
            {
                return _beforeMB;
            }
            set
            {
                _beforeMB = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// The app memory usage, in megabytes, after the current downsample image has been applied.
        /// </summary>
        public long MemAfterMB
        {
            get
            {
                return _afterMB;
            }
            set
            {
                _afterMB = value;
                RaisePropertyChanged();
            }
        }

        private bool _maintainAspectRatio = true;
        /// <summary>
        /// If true, maintain the aspect ratio of the image when updating the DecodePixelWidth and/or DecodePixelheight
        /// </summary>
        public bool MaintainAspectRatio
        {
            get
            {
                return _maintainAspectRatio;
            }
            set
            {
                _maintainAspectRatio = value;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
