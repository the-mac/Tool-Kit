using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.SharePoint.Client;
using NTLMSampleCode.Resources;
using System.IO.IsolatedStorage;

namespace NTLMSampleCode
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

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(url.Text))
            {
                MessageBox.Show("url cannot be empty");
                return;
            }

            if(!IsolatedStorageSettings.ApplicationSettings.Contains(Constants.UserName) || !IsolatedStorageSettings.ApplicationSettings.Contains(Constants.Password))
            {
                NavigationService.Navigate(new Uri("/CustomLoginPage.xaml", UriKind.Relative));
                return;
            }

            ClientContext context = new ClientContext(url.Text);
            context.Credentials = new NetworkCredential((string)IsolatedStorageSettings.ApplicationSettings[Constants.UserName], (string)IsolatedStorageSettings.ApplicationSettings[Constants.Password]);
            Web web = context.Web;
            context.Load(web);
            context.ExecuteQueryAsync(
                (object s, ClientRequestSucceededEventArgs args) =>
                {
                    this.Dispatcher.BeginInvoke(() =>
                            {
                                MessageBox.Show(web.Title);
                            });
                },

                (object s, ClientRequestFailedEventArgs args) =>
                {
                    if (Authenticator.IsRequestUnauthorized(args))
                    {
                        //Username password may be wrong.
                        //Go to login page again
                        NavigationService.Navigate(new Uri("/CustomLoginPage.xaml", UriKind.Relative));
                        return;
                    }

                    this.Dispatcher.BeginInvoke(() =>
                    {
                        MessageBox.Show(args.Exception.Message);
                    });
                });            
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