/* 
    Copyright (c) 2012 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
using System;
using System.Windows;
using Microsoft.Phone.Controls;


namespace sdkPhotoExtensibilityWP8CS
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        // Help messages
        private void helpPhotosHub_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Tap Start, open the Photos Hub, then swipe over to the Apps panel.");
        }
        private void helpPhotoAppsPicker_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Tap Start, view a photo, then in the app bar, tap Apps.");
        }

        private void helpPhotoSharePicker_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Tap Start, view a photo, then in the app bar, tap Share.");
        }

        private void helpPhotoEditPicker_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Tap Start, view a photo, then in the app bar, tap Edit.");
        }

        private void helpRichMedia_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/PhotoSave.xaml", UriKind.Relative));
        }

        private void HyperlinkButton_Click_1(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/PhotoSave.xaml", UriKind.Relative));
        }




    }
}
