/* 
    Copyright (c) 2012 - 2013 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://code.msdn.microsoft.com/wpapps
  
*/
using System.Windows;
using Microsoft.Phone.Controls;

using System.IO.IsolatedStorage;


namespace sdkKidsCornerModeWP8CS
{
    public partial class MainPage : PhoneApplicationPage
    {
        /// <summary>
        /// This is a property that wraps the ApplicationProfile.Modes property.
        /// If the value of the property is ApplicationProfileModes.Alternate,
        /// then the app is currently running in Kid's Corner.
        /// </summary>
        public bool IsRunningInKidsCorner
        {
            get
            {
                if (Windows.Phone.ApplicationModel.ApplicationProfile.Modes ==
                    Windows.Phone.ApplicationModel.ApplicationProfileModes.Alternate)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
       

        /// <summary>
        /// The DisableButtonInKidsCorner property is 2-way bound to the KidsCornerCheckbox control
        /// defined in MainPage.xaml. As the checkbox is checked and unchecked, a boolean value is stored
        /// using IsolatedStorageSettings.
        /// </summary>
        public bool DisableButtonInKidsCorner
        {
            get
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains("DisableButtonInKidsCorner"))
                {
                    return (bool)IsolatedStorageSettings.ApplicationSettings["DisableButtonInKidsCorner"];
                }
                else
                {
                    // default value is false
                    return false;
                }
            }
            set
            {
                IsolatedStorageSettings.ApplicationSettings["DisableButtonInKidsCorner"] = value;
                IsolatedStorageSettings.ApplicationSettings.Save();
            }
        }


        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();

            // Set the data context to the current page to to enable binding the checkbox to the DisableButtonInKidsCorner property
            this.DataContext = this;

            // Check the property that wraps the ApplicationProfile.Modes property to determine if the app is currently
            // running in Kid's Corner and then adjust your app's UI and behavior as needed.
            if (IsRunningInKidsCorner)
            {
                // Update the status textblock to indicate that the app is running in Kid's Corner.
                StatusTextBlock.Text = "Running in Kid's Corner";

                // Hide the check box while running in Kid's Corner
                KidsCornerCheckbox.Visibility = Visibility.Collapsed;

                // Check the IsolatedStorageSettings value that is stored by the DisableButtonInKidsCorner property
                if (DisableButtonInKidsCorner)
                {
                    // In this example, the test button is disabled when runnin in Kid's Mode
                    TestButton.IsEnabled = false;
                }
            }
            else
            {
                StatusTextBlock.Text = "Running in normal mode";
            }
        }

        /// <summary>
        /// Click handler for the test button. Show a message box.
        /// </summary>
        private void TestButton_Click_1(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("You clicked the button.");
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
