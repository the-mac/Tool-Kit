/* 
    Copyright (c) 2011 Microsoft Corporation.  All rights reserved.
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

namespace sdkTilesCS
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Navigate to the page for modifying Application Tile properties
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonChangeApplicationTile_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/ApplicationTile.xaml", UriKind.Relative));
        }

        /// <summary>
        /// Navigate to the page for modifying secondary Tile properties. 
        /// Pass a parameter that lets the SecondaryTile page know that it was navigated to from MainPage.
        /// (DefaultTitle will equal 'FromTile' when the user navigates to the SecondaryTile page from a Tile.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonChangeSecondaryTile_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/SecondaryTile.xaml?DefaultTitle=FromMain", UriKind.Relative));
        }
    }
}
