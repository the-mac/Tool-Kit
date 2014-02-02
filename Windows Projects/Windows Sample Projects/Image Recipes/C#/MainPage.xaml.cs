/* 
    Copyright (c) 2012 - 2013 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://code.msdn.microsoft.com/wpapps
  
*/
using Microsoft.Phone.Controls;
using System;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace sdkImages
{
    // This page is used in this sample as a menu to the actual scenarios. As such, it does nothing else but take
    // the user to whichever scenario they clicked on. 
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }


        private void HyperlinkButton_Tap_1(object sender, System.Windows.Input.GestureEventArgs e)
        {
            // For simplicity, the same Tap event handler is used for every HyperlinkButton on the page. 
            // The target of the link is derived from the string value stored in the Tag property of each button. 
            NavigationService.Navigate(new Uri(((HyperlinkButton)sender).Tag.ToString(), UriKind.Relative));
        }
    }
}
