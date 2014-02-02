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
using Microsoft.Phone.Tasks;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace ControlsCatalog
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string clicked = ((Button)sender).Name;
            NavigationService.Navigate(new Uri("/Index/" + clicked + ".xaml", UriKind.Relative));
        }
        private void MoreInfo_Click(object sender, RoutedEventArgs e)
        {
            WebBrowserTask moreInfo = new WebBrowserTask();
            moreInfo.Uri = new Uri("http://msdn.microsoft.com/library/windowsphone/develop/ff402561(v=vs.105).aspx");
            moreInfo.Show();
        }
    }
}
