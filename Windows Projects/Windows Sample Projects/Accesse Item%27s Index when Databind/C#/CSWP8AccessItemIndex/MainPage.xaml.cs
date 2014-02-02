/****************************** Module Header ******************************\
* Module Name:    MainPage.xaml.cs
* Project:        CSWP8AccessItemIndex
* Copyright (c) Microsoft Corporation
*
* This sample demonstrates how to get the item index of ItemsControl's item
* and how to use it.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\*****************************************************************************/

using Microsoft.Phone.Controls;
using System.Windows;

namespace CSWP8AccessItemIndex
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor   
        public MainPage()
        {
            InitializeComponent();
            Loaded += Page_Loaded;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // data bind
            this.ItemView1.DataContext = App.ViewModel;
            ItemView2.itmCustomType.ItemsSource = App.ViewModel.UserDatas;
        }
    }
}