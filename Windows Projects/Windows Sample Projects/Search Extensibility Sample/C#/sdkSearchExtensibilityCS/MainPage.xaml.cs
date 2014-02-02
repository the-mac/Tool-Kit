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
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

namespace sdkSearchExtensibilityCS
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Set the data context of the listbox control to the sample data.
            DataContext = App.MainViewModel;
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        // Load data from the MainViewModel.
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            // Text that describes how the sample app works.
            string body = "Welcome to the search extensibility sample application named TreyResearch. \r\rTo use this application, tap the Search button on your phone and search for a baby, nusery, or toy product. Then tap a product under the products list in the web pivot page. \r\rIn the product card that appears, swipe to the apps pivot page and launch the TreyResearch application.";

            // If the app data has not been loaded yet, load it.
            if (!App.MainViewModel.IsDataLoaded)
            {
                // Load data here
                App.MainViewModel.LoadData();

                // Bind the description text to the TextBlock.
                App.MainViewModel.Body = body;
            }
        }

        // PhoneApplicationPage_Loaded implementation.
        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
        }
    }
}
