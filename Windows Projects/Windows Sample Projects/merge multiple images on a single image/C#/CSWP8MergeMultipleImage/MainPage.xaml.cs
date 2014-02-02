/****************************** Module Header ******************************\
* Module Name:    MainPage.xaml.cs
* Project:        CSWP8MergeMultipleImage
* Copyright (c) Microsoft Corporation
*
* This sample will demo how to merge multiple images into a single image.
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
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Media;
using System.IO.IsolatedStorage;
using Microsoft.Xna.Framework.Media;

namespace CSWP8MergeMultipleImage
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        private void btnMerge_Click(object sender, RoutedEventArgs e)
        {
            string[] files = new string[] { "2.jpg", "test.jpg", "3.jpg", "4.jpg", "5.jpg", "test.jpg" }; // Image list.
            List<BitmapImage> images = new List<BitmapImage>(); // BitmapImage list.
            int width = 0; // Final width.
            int height = 0; // Final height.

            foreach (string image in files)
            {
                // Create a Bitmap from the file and add it to the list                
                BitmapImage img = new BitmapImage();
                StreamResourceInfo r = System.Windows.Application.GetResourceStream(new Uri(image, UriKind.Relative));
                img.SetSource(r.Stream);

                WriteableBitmap wb = new WriteableBitmap(img);

                // Update the size of the final bitmap
                width = wb.PixelWidth > width ? wb.PixelWidth : width;
                height = wb.PixelHeight > height ? wb.PixelHeight : height;

                images.Add(img);
            }

            // Create a bitmap to hold the combined image 
            BitmapImage finalImage = new BitmapImage();
            StreamResourceInfo sri = System.Windows.Application.GetResourceStream(new Uri("White.jpg", 
                UriKind.Relative));
            finalImage.SetSource(sri.Stream);
            WriteableBitmap wbFinal = new WriteableBitmap(finalImage);

            using (MemoryStream mem = new MemoryStream())
            {
                int tempWidth = 0;   // Parameter for Translate.X
                int tempHeight = 0;  // Parameter for Translate.Y

                foreach (BitmapImage item in images)
                {
                    Image image = new Image();
                    image.Height = item.PixelHeight;
                    image.Width = item.PixelWidth;
                    image.Source = item;

                    // TranslateTransform                      
                    TranslateTransform tf = new TranslateTransform();
                    tf.X = tempWidth;
                    tf.Y = tempHeight;
                    wbFinal.Render(image, tf);

                    tempHeight += item.PixelHeight;
                }

                wbFinal.Invalidate();
                wbFinal.SaveJpeg(mem, width, height, 0, 100);
                mem.Seek(0, System.IO.SeekOrigin.Begin);

                // Show image.               
                img2.Source = wbFinal;
            }

            // Save image.
            using (IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication())
            {
                String strImageName = "demo.jpg";  // File name.

                if (iso.FileExists(strImageName))
                {
                    iso.DeleteFile(strImageName);
                }

                using (IsolatedStorageFileStream isostream = iso.CreateFile(strImageName))
                {
                    // Encode WriteableBitmap object to a JPEG stream.
                    Extensions.SaveJpeg(wbFinal, isostream, wbFinal.PixelWidth, wbFinal.PixelHeight, 0, 85);
                }

                using (IsolatedStorageFileStream fileStream = iso.OpenFile(strImageName, FileMode.OpenOrCreate, 
                    FileAccess.Read))
                {
                    MediaLibrary mediaLibrary = new MediaLibrary();
                    Picture pic = mediaLibrary.SavePicture(strImageName, fileStream);
                }
            }
        }
    }
}