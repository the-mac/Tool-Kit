/****************************** Module Header ******************************\
* Module Name:    MainPage.xaml.cs
* Project:        CSWP8ImageOperation
* Copyright (c) Microsoft Corporation
*
* The project illustrates how to dynamically create ,display and remove images
* at run time. 
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
using System.Windows.Media.Imaging;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace CSWP8ImageOperation
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            DynamicImages();
            RotationSetUp();
        }

        int intCurrentIndex = 0;      // The index of the selected image.
        string imgDefaultName = "img";// Default Name of the image.
        Image newImg = null;          // The selected image.

        /// <summary>
        /// Create images and associated events to them.
        /// </summary>
        void DynamicImages()
        {
            for (int ctr = 0; ctr < 4; ctr++)
            {
                Image img = new Image();
                img.Stretch = Stretch.Uniform;
                img.Name = imgDefaultName + ctr.ToString();
                img.Height = 240;
                img.Width = 180;
                img.MouseLeftButtonDown += new MouseButtonEventHandler(img_MouseLeftButtonDown);

                #region Layout
                if (ctr < 2)
                {
                    img.Margin = new Thickness(ctr * (img.ActualWidth / 10) + 20, img.ActualHeight / 10, 0, 0);
                    canvas1.Children.Add(img);
                    Canvas.SetLeft(img, ctr * img.ActualWidth);
                    Canvas.SetTop(img, img.ActualHeight / 10);
                }
                else
                {
                    img.Margin = new Thickness((ctr - 2) * (img.ActualWidth / 10) + 20, img.ActualHeight / 10, 0, 0);
                    canvas1.Children.Add(img);
                    Canvas.SetLeft(img, ((ctr - 2) * img.ActualWidth));
                    Canvas.SetTop(img, img.ActualHeight + img.ActualHeight / 10);
                }
                #endregion

                img.Source = new BitmapImage(new Uri("1code.jpg", UriKind.Relative));
            }
        }

        /// <summary>
        /// Remove Images
        /// </summary>       
        public void DynamicImagesDel()
        {
            // Demo the remove the selected images.
            canvas1.Children.RemoveAt(intCurrentIndex);

            // [-or-] 

            // Demo the remove all images.
            //canvas1.Children.Clear();   

            // [-or-] 

            // Demo the remove all images.
            //List<UIElement> listDel = new List<UIElement>();
            //listDel = canvas1.Children.ToList();
            //foreach (UIElement item in listDel)
            //{
            //    newImg = item as Image;
            //    intCurrentIndex = canvas1.Children.IndexOf(newImg);
            //    canvas1.Children.RemoveAt(intCurrentIndex);
            //}                   
        }

        #region Properties for Animation.
        PlaneProjection pp1 = new PlaneProjection();
        Storyboard sB1 = new Storyboard();
        DoubleAnimation dA1 = new DoubleAnimation();
        TimeSpan tSpan = new TimeSpan(0, 0, 0, 0, 3000);
        #endregion

        /// <summary>
        /// Animation of the selected images.
        /// </summary>
        void RotationSetUp()
        {
            dA1.Duration = tSpan;
            dA1.From = 0;
            dA1.To = -180;

            pp1.CenterOfRotationZ = -.5;
            Storyboard.SetTarget(dA1, pp1);
            Storyboard.SetTargetProperty(dA1, new PropertyPath(PlaneProjection.RotationZProperty));
            sB1.Completed += new EventHandler(sB1_Completed);
            sB1.AutoReverse = true;
            sB1.Children.Add(dA1);
        }

        // Don't allow another animations until current one is finished.   
        bool done = true;

        /// <summary>
        /// This will handle the Storyboard's Completed event. And reset the flag(done).          
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void sB1_Completed(object sender, EventArgs e)
        {
            // Remove the projection or it will rotate with another pictures. 
            newImg.Projection = null;
            // Reset the flag.
            done = true;
            // Remove the image.
            DynamicImagesDel();
        }

        /// <summary>
        /// We will use the images' MouseLeftButtonDown event to start Storyboard.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void img_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!done)
            {
                return;     // The animation can't start yet.   
            }

            done = false;   // Clear the flag. 

            newImg = e.OriginalSource as Image;
            newImg.Projection = pp1;
           
            // Store the index of the selected image.
            intCurrentIndex = canvas1.Children.IndexOf(newImg);
            
            // Animation of the selected images.
            sB1.Begin();
        }
    }
}