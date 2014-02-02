/****************************** Module Header ******************************\
* Module Name:    MainPage.xaml.cs
* Project:        CSWP8Nestbinding
* Copyright (c) Microsoft Corporation
*
* This sample will demo how to nested data-bound controls in WP.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\*****************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using CSWP8Nestbinding.Resources;

namespace CSWP8Nestbinding
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }     

        private void btnValue_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/ListFromIValueConverterAsDatasource.xaml", UriKind.Relative));
        }

        private void btnNested_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/NestedListAsDatasource.xaml", UriKind.Relative));
        }      
    }
}