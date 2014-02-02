using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Authentication.Resources;
//TODO: using Microsoft.WindowsAzure.MobileServices;

namespace Authentication
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        private async void OnBtnMicrosoftAccountClick(object sender, RoutedEventArgs e)
        {
            try
            {
                ////TODO: Call LoginAsync: 
                //await App.MobileService.LoginAsync(MobileServiceAuthenticationProvider.MicrosoftAccount);
                //txtStatus.Text = string.Format("Logged in with: {0}", App.MobileService.CurrentUser.UserId);
            }
            catch (InvalidOperationException iopEx)
            {
                txtStatus.Text = string.Format("Error Occured: \n {0}", iopEx.ToString());
            }
            
        }

        private async void OnBtnFacebookAccountClick(object sender, RoutedEventArgs e)
        {
            try
            {
                ////TODO: Call LoginAsync: 
                //await App.MobileService.LoginAsync(MobileServiceAuthenticationProvider.Facebook);                
                //txtStatus.Text = string.Format("Logged in with: {0}", App.MobileService.CurrentUser.UserId);
            }
            catch (InvalidOperationException iopEx)
            {
                txtStatus.Text = string.Format("Error Occured: \n {0}", iopEx.ToString());
            }
        }

        private async void OnBtnTwitterAccountClick(object sender, RoutedEventArgs e)
        {
            try
            {
                ////TODO: Call LoginAsync:  
                //await App.MobileService.LoginAsync(MobileServiceAuthenticationProvider.Twitter);              
                //txtStatus.Text = string.Format("Logged in with: {0}", App.MobileService.CurrentUser.UserId);
            }
            catch (InvalidOperationException iopEx)
            {
                txtStatus.Text = string.Format("Error Occured: \n {0}", iopEx.ToString());
            }
        }

        private async void OnBtnGoogleAccountClick(object sender, RoutedEventArgs e)
        {
            try
            {
                ////TODO: Call LoginAsync: 
                //await App.MobileService.LoginAsync(MobileServiceAuthenticationProvider.Google);
                //txtStatus.Text = string.Format("Logged in with: {0}", App.MobileService.CurrentUser.UserId);
            }
            catch (InvalidOperationException iopEx)
            {
                txtStatus.Text = string.Format("Error Occured: \n {0}", iopEx.ToString());
            }
        }

        private void OnBtnLogoutClick(object sender, RoutedEventArgs e)
        {
            try
            {
                ////TODO: add logout
                //if (App.MobileService.CurrentUser != null && App.MobileService.CurrentUser.UserId != null)
                //     App.MobileService.Logout();

                txtStatus.Text = "Logged out";
            }
            catch (InvalidOperationException iopEx)
            {
                txtStatus.Text = string.Format("Error Occured: \n {0}", iopEx.ToString());
            }
        }

        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}