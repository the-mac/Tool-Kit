/****************************** Module Header ******************************\
* Module Name:    MainPage.aspx.cs
* Project:        CSWP8TextToImage
* Copyright (c) Microsoft Corporation
*
* This demo shows how to draw text to Image then display it or save it to hub.
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
using Microsoft.Phone.Controls;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.IO.IsolatedStorage;
using Microsoft.Xna.Framework.Media;

namespace CSWP8TextToImage
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        /// <summary>
        ///  Draw text
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDraw_Click(object sender, RoutedEventArgs e)
        {
            using (MemoryStream mem = new MemoryStream())
            {
                BitmapImage img = new BitmapImage();
                StreamResourceInfo r = System.Windows.Application.GetResourceStream(new Uri("test.jpg", UriKind.Relative));
                img.SetSource(r.Stream);

                WriteableBitmap wb = new WriteableBitmap(img);
                TextBlock tb = new TextBlock();
                tb.FontSize = 40;
                tb.Text = tbInput.Text + "\r\n" + "This is test text";

                // TranslateTransform
                TranslateTransform tf = new TranslateTransform();
                tf.X = 100;
                tf.Y = 100;
                wb.Render(tb, tf);
                wb.Invalidate();

                wb.SaveJpeg(mem, wb.PixelWidth, wb.PixelHeight, 0, 100);
                mem.Seek(0, System.IO.SeekOrigin.Begin);

                // Show image.
                BitmapImage bn = new BitmapImage();
                bn.SetSource(mem);
                img2.Source = bn;

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
                        Extensions.SaveJpeg(wb, isostream, wb.PixelWidth, wb.PixelHeight, 0, 85);
                        isostream.Close();
                    }

                    using (IsolatedStorageFileStream fileStream = iso.OpenFile(strImageName, FileMode.Open, FileAccess.Read))
                    {
                        MediaLibrary mediaLibrary = new MediaLibrary();
                        Picture pic = mediaLibrary.SavePicture(strImageName, fileStream);
                        fileStream.Close();
                    }
                }
            }
        }
    }
}