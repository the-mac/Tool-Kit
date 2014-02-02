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
using System.Linq;
using System.Windows;
/* 
    Copyright (c) 2011 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
    
*/
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace sdkBackstackCS
{
    public partial class Page1 : PhoneApplicationPage
    {
        // The URI string of the next page to navigate to from this page.
        // String.Empty here means that there is no next page.
        private string nextPage;

        public Page1()
        {
            InitializeComponent();

            // Set the application title - use the same application title on each page
            ApplicationTitle.Text = "SDK BACKSTACK SAMPLE";

            // Set unique page title. In this example we will use "page 1", "page 2" etc.
            PageTitle.Text = "page 1";

            // Set the URI string of the next page, or String.Empty if there is no next page.
            nextPage = "/Page2.xaml";

        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Show the Next button, if we have defined a next page.
            btnNext.Visibility = (String.IsNullOrWhiteSpace(nextPage)) ? Visibility.Collapsed : Visibility.Visible;

            if (ShellTile.ActiveTiles.FirstOrDefault(o => o.NavigationUri.ToString().Contains(NavigationService.Source.ToString())) == null)
                PinToStartCheckBox.IsChecked = false;
            else
                PinToStartCheckBox.IsChecked = true;
        }

        /// <summary>
        /// Navigate to the next page.
        /// </summary>
        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            // Make sure to only attempt navigation if we have defined a next page.
            if (!String.IsNullOrWhiteSpace(nextPage))
            {
                this.NavigationService.Navigate(new Uri(nextPage, UriKind.Relative));
            }
        }

        /// <summary>
        /// Toggle pinning this a tile for this page on the start menu.
        /// </summary>
        private void PinToStartCheckBox_Click(object sender, RoutedEventArgs e)
        {
            // Try to find a tile that has this page's URI
            ShellTile tile = ShellTile.ActiveTiles.FirstOrDefault(o => o.NavigationUri.ToString().Contains(NavigationService.Source.ToString()));

            if (tile == null)
            {
                // No tile was found, so add one for this page.
                StandardTileData tileData = new StandardTileData { Title = PageTitle.Text };
                ShellTile.Create(new Uri(NavigationService.Source.ToString(), UriKind.Relative), tileData);
            }
            else
            {
                // A tile was found, so remove it.
                tile.Delete();
            }
        }
    }
}
