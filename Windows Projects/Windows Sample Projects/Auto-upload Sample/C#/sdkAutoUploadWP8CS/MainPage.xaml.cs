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
using System.Net;
using System.IO.IsolatedStorage;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;
using sdkAutoUploadWP8CS.Resources;
using System.IO;

namespace sdkAutoUploadWP8CS
{
  public partial class MainPage : PhoneApplicationPage 
  {
    private IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;

    public MainPage()
    {
      InitializeComponent();
    }

    private void LaunchSettings(object sender, RoutedEventArgs e)
    {
      NavigationService.Navigate(new Uri("/AutoUploadSettingsPage.xaml", UriKind.Relative));
    }

    private void SignIn(object sender, RoutedEventArgs e)
    {
        // TODO: Launch web-service sign-in flow and receive the authentication key.
        MessageBox.Show("TODO: Launch the web-service sign-in flow and receive auth key.");
        
        // Example key from web service.
        string exampleAuthKey = "979C65FCAA04C19E07C01836BB80A208A6B2F756E15DD681B3C3F41786220FB5";

        // Encrypt authentication key for use by background agent.
	settings["AccessToken"] = ProtectedData.Protect(Encoding.UTF8.GetBytes(exampleAuthKey), null);
	settings.Save();
    }
  }
}
