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

namespace sdkMulticastCS
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        private void JoinButton_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(UsernameTextBox.Text))
            {
                // Display a message. Diagnostics.SafeShow is a wrapper that makes sure
                // we call the messagebox on the UI thread.
                DiagnosticsHelper.SafeShow("Please enter a username to join the game");
            }
            else
            {
                // Make sure the name we chose shows up in the players list.
                App.Players.Add(new PlayerInfo(UsernameTextBox.Text));

                // Go to the players list.
                NavigationService.Navigate(new Uri("/Players.xaml?player=" + UsernameTextBox.Text, UriKind.RelativeOrAbsolute));
            }
        }

        private void LeaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (App.GamePlay != null)
                App.GamePlay.Leave(false);
        }
    }
}
