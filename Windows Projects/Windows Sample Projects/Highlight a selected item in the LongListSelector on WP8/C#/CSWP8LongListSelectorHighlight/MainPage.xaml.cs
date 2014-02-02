/****************************** Module Header ******************************\
* Module Name:    MainPage.xaml.cs
* Project:        CSWP8LongListSelectorHighlight
* Copyright (c) Microsoft Corporation
*
* This demo shows how to highlight a selected item in the LongListSelector 
* on WP8.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\*****************************************************************************/

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using System.Windows.Media;

namespace CSWP8LongListSelectorHighlight
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Test data.
            List<string> listData = new List<string>();
            listData.Add("ASPNET");
            listData.Add("WCF");
            listData.Add("WPF");
            listData.Add("Windows Store");
            listData.Add("Windows Phone");

            // Bind to control.
            MyLongListSelector1.ItemsSource = MyLongListSelector2.ItemsSource = listData;
        }

        private void MyLongListSelector1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Get item of LongListSelector.
            List<CustomUserControl> userControlList = new List<CustomUserControl>();
            GetItemsRecursive<CustomUserControl>(MyLongListSelector1, ref userControlList);

            // Selected.
            if (e.AddedItems.Count > 0 && e.AddedItems[0] != null)
            {
                foreach (CustomUserControl userControl in userControlList)
                {
                    if (e.AddedItems[0].Equals(userControl.DataContext))
                    {
                        VisualStateManager.GoToState(userControl, "Selected", true);
                    }
                }
            }
            // Unselected.
            if (e.RemovedItems.Count > 0 && e.RemovedItems[0] != null)
            {
                foreach (CustomUserControl userControl in userControlList)
                {
                    if (e.RemovedItems[0].Equals(userControl.DataContext))
                    {
                        VisualStateManager.GoToState(userControl, "Normal", true);
                    }
                }
            }
        }

        private void MyLongListSelector2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Get item of LongListSelector.
            List<UserControl> listItems = new List<UserControl>();
            GetItemsRecursive<UserControl>(MyLongListSelector2, ref listItems);

            // Selected.
            if (e.AddedItems.Count > 0 && e.AddedItems[0] != null)
            {
                foreach (UserControl userControl in listItems)
                {
                    if (e.AddedItems[0].Equals(userControl.DataContext))
                    {
                        VisualStateManager.GoToState(userControl, "Selected", true);
                    }
                }
            }
            // Unselected.
            if (e.RemovedItems.Count > 0 && e.RemovedItems[0] != null)
            {
                foreach (UserControl userControl in listItems)
                {
                    if (e.RemovedItems[0].Equals(userControl.DataContext))
                    {
                        VisualStateManager.GoToState(userControl, "Normal", true);
                    }
                }
            }
        }

        /// <summary>
        /// Recursively get the item.
        /// </summary>
        /// <typeparam name="T">The item to get.</typeparam>
        /// <param name="parents">Parent container.</param>
        /// <param name="objectList">Item list</param>
        public static void GetItemsRecursive<T>(DependencyObject parents, ref List<T> objectList) where T : DependencyObject
        {
            var childrenCount = VisualTreeHelper.GetChildrenCount(parents);

            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parents, i);

                if (child is T)
                {
                    objectList.Add(child as T);
                }

                GetItemsRecursive<T>(child, ref objectList);
            }

            return;
        }

    }
}