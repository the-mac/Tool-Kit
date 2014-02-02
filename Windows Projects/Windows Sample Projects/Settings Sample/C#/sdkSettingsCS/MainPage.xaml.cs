/* 
    Copyright (c) 2012 - 2013 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://code.msdn.microsoft.com/wpapps
  
*/
using System;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace sdkSettingsCS
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            SupportedOrientations = SupportedPageOrientation.Portrait;

            // Add an Application Bar with a 'setting menu item.
            ApplicationBar = new ApplicationBar();
            ApplicationBar.IsMenuEnabled = true;
            ApplicationBar.IsVisible = true;
            ApplicationBar.Opacity = 1.0;

            ApplicationBarMenuItem settingsItem = new ApplicationBarMenuItem("settings");
            settingsItem.Click += new EventHandler(settings_Click);

            ApplicationBar.MenuItems.Add(settingsItem);
        }

        /// <summary>
        /// Settings button clicked event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void settings_Click(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/SettingsWithoutConfirmation.xaml", UriKind.Relative));
        }

    }
}
