/****************************** Module Header ******************************\
* Module Name:    MainPage.xaml.cs
* Project:        CSWP8ImageFromIsolatedStorage
* Copyright (c) Microsoft Corporation
*
* This sample willl demo how to load an Image from Isolated Storage and display
* it on the Device
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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.IO;
using System.IO.IsolatedStorage;
using System.Net;
using Microsoft.Phone.Controls;
using System.Diagnostics;

namespace CSWP8ImageFromIsolatedStorage
{
    public partial class MainPage : PhoneApplicationPage
    {
        // ImageName
        string strImageName = "defaultName.png";

        // ImagePath
        string strPath = string.Empty;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Loading local or network picture.
            CreateImage();

            // Sample code to localize the ApplicationBar
            LoadImageFromIsolatedStorage();
        }
       
        /// <summary>
        /// Loading local or network picture. According to whether it contains the http protocol
        /// to determine whether the image from the web server.
        /// </summary>
        private void CreateImage()
        {
            // Local Image
            //strPath = "WP8Logo.png";
            // Network Image
            strPath = "http://i.s-microsoft.com/global/ImageStore/PublishingImages/logos/hp/logo-lg-1x.png";

            if (strPath.Contains("http://"))
            {
                strImageName = strPath.Substring(strPath.LastIndexOf("/") + 1);
                SaveImageToIsolatedStorage(1);
            }
            else
            {
                strImageName = strPath;
                SaveImageToIsolatedStorage(0);
            }
        }

        /// <summary>
        /// Save image to IsolatedStorage.
        /// </summary>
        /// <param name="flag">1:web server;0:Local</param>
        private void SaveImageToIsolatedStorage(int flag)
        {
            if (flag == 1)
            {
                // Use WebClient to download web server's images.
                WebClient webClientImg = new WebClient();
                webClientImg.OpenReadCompleted += new OpenReadCompletedEventHandler(client_OpenReadCompleted);
                webClientImg.OpenReadAsync(new Uri(strPath, UriKind.Absolute));
            }
            else
            {
                // Use Uri to get local images.
                StreamResourceInfo sri = null;
                Uri uri = new Uri(strPath, UriKind.Relative);
                sri = Application.GetResourceStream(uri);

                // Save the local image's stream into a jpeg picture.
                SaveToJpeg(sri.Stream);
            }
        }

        /// <summary>
        /// Save stream to jpeg when the asynchronous resource-read operation is completed.
        /// </summary>
        /// <param name="sender">WebClient</param>
        /// <param name="e">OpenReadCompleted Event</param>
        void client_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            // Save the returned image stream into a jpeg picture.
            SaveToJpeg(e.Result);
        }

        /// <summary>
        /// Save stream to Jpeg.First, determine and delete the file with the same name
        /// in IsolatedStorage, and then create a new one.
        /// </summary>
        /// <param name="stream">The stream of local image or network image</param>
        private void SaveToJpeg(Stream stream)
        {
            using (IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (iso.FileExists(strImageName))
                {
                    iso.DeleteFile(strImageName);
                }

                using (IsolatedStorageFileStream isostream = iso.CreateFile(strImageName))
                {                   
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.SetSource(stream);
                    WriteableBitmap wb = new WriteableBitmap(bitmap);

                    // Encode WriteableBitmap object to a JPEG stream.
                    Extensions.SaveJpeg(wb, isostream, wb.PixelWidth, wb.PixelHeight, 0, 85);
                    isostream.Close();
                }
            }
        }

        /// <summary>
        /// Sample code for loading image from IsolatedStorage
        /// </summary> 
        private void LoadImageFromIsolatedStorage()
        {
            // The image will be read from isolated storage into the following byte array
            byte[] data;

            // Read the entire image in one go into a byte array
            try
            {
                using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    // Open the file - error handling omitted for brevity
                    // Note: If the image does not exist in isolated storage the following exception will be generated:
                    // System.IO.IsolatedStorage.IsolatedStorageException was unhandled 
                    // Message=Operation not permitted on IsolatedStorageFileStream 
                    using (IsolatedStorageFileStream isfs = isf.OpenFile(strImageName, FileMode.Open, FileAccess.Read))
                    {
                        // Allocate an array large enough for the entire file
                        data = new byte[isfs.Length];

                        // Read the entire file and then close it
                        isfs.Read(data, 0, data.Length);
                        isfs.Close();
                    }
                }

                // Create memory stream and bitmap
                MemoryStream ms = new MemoryStream(data);
                BitmapImage bi = new BitmapImage();

                // Set bitmap source to memory stream
                bi.SetSource(ms);

                // Create an image UI element – Note: this could be declared in the XAML instead
                Image image = new Image();

                // Set size of image to bitmap size for this demonstration
                image.Height = bi.PixelHeight;
                image.Width = bi.PixelWidth;

                // Assign the bitmap image to the image’s source
                image.Source = bi;

                // Add the image to the grid in order to display the bit map
                ContentPanel.Children.Add(image);
            }
            catch (Exception e)
            {
                // handle the exception
                Debug.WriteLine(e.Message);
            }
        }
    }
}