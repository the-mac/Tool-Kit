/****************************** Module Header ******************************\
* Module Name:  MainPage.xaml.cs
* Project:		CSWP7ListAsTree
* Copyright (c) Microsoft Corporation.
* 
* This sample demonstrates how use ListBox Control as a tree in Windows Phone. 
* All pictures in media library will show in ListBox Control with indention by recursion.
*
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework.Media;
using System.IO;
using System.Windows.Media.Imaging;

namespace CSWP7ListAsTree
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            using (var library = new MediaLibrary())
            {
                ShowAlbum(library.RootPictureAlbum, "|");
            }
        }

        // Show album pictures as a tree
        void ShowAlbum(PictureAlbum theAlbum, string indention)
        {
            // Show Album Name
            treeList.Items.Add(string.Concat(indention, "Album: ", theAlbum.Name));

            // List Albums in this Album
            foreach (PictureAlbum subAlbum in theAlbum.Albums)
            {
                ShowAlbum(subAlbum, string.Concat(indention, "-"));
            }

            // List Pictures
            foreach (Picture thePicture in theAlbum.Pictures)
            {
                // Get the Picture Stream
                Stream imageStream = thePicture.GetThumbnail();

                // Wrap it with a BitmapImage object
                var bitmap = new BitmapImage();
                bitmap.SetSource(imageStream);

                // Create an Image element and set the bitmap
                var image = new Image();
                image.Width = 60;
                image.Height = 60;
                image.Source = bitmap;

                StackPanel outPanel = new StackPanel();
                StackPanel stackPanel = new StackPanel();
                stackPanel.Orientation = System.Windows.Controls.Orientation.Horizontal;
                TextBlock textBlock = new TextBlock();
                textBlock.Text = thePicture.Name;

                stackPanel.Children.Add(image);
                stackPanel.Children.Add(textBlock);

                outPanel.Orientation = System.Windows.Controls.Orientation.Horizontal;
                TextBlock indentionBlock = new TextBlock();
                indentionBlock.Text = string.Concat(indention, "-");
                outPanel.Children.Add(indentionBlock);
                outPanel.Children.Add(stackPanel);

                treeList.Items.Add(outPanel);
            }
        }
    }
}