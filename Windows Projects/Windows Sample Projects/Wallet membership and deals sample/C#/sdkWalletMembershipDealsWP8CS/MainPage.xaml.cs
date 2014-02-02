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
using System.Windows.Input;
using Microsoft.Phone.Controls;

namespace sdkWalletMembershipDealsWP8CS
{
    public partial class MainPage : PhoneApplicationPage
    {
        /// <summary>
        /// Constructor for the Main Page
        /// </summary>
        public MainPage()
        {
            InitializeComponent();
        }

        // Tap membership click event
        private void Membership_Tap_1(object sender, GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/MembershipPage.xaml", UriKind.Relative));
        }

        // Tap deals click event
        private void Coupons_Tap_1(object sender, GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/CouponsPage.xaml", UriKind.Relative));
        }
    }
}
