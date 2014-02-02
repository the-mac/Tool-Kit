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
using System.Windows.Input;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using sdkBluetoothA2AWP8CS.Resources;

namespace sdkBluetoothA2AWP8CS
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();
            this.DataContext = this;
            Loaded += MainPage_Loaded;
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            // Bluetooth is not available in the emulator. 
            if (Microsoft.Devices.Environment.DeviceType == Microsoft.Devices.DeviceType.Emulator)
            {
                MessageBox.Show(AppResources.Warn_EmulatorMode, AppResources.Warn_EmulatorModeTitle, MessageBoxButton.OK);
                btnJoin.IsEnabled = false;
            }
        }

        private void btnJoin_Tap_1(object sender, GestureEventArgs e)
        {
            if (String.IsNullOrEmpty(ChatName))
            {
                MessageBox.Show(AppResources.Err_NoChatName, AppResources.Err_NoChatNameCaption, MessageBoxButton.OK);
            }
            else
            {
                // Note: The chat name is not passed in as part of the query string, since it
                // is stored in ApplicationSettings and that is accessible from every page.
                this.NavigationService.Navigate(new Uri("/ChatPage.xaml", UriKind.RelativeOrAbsolute));
            }
        }

        /// <summary>
        /// The user's chat name
        /// </summary>
        public string ChatName
        {
            get
            {
                return App.ChatName;
            }
            set
            {
                if (value != App.ChatName)
                {
                    // Store the name in ApplicationSettings, so it can be used the next time the app is started.
                    App.ChatName = value;
                }
            }
        }
    }
}
