/****************************** Module Header ******************************\
* Module Name:    MainPage.xaml.cs
* Project:        CSWP8ListBoxItemMultiColumn
* Copyright (c) Microsoft Corporation
*
* This sample will demo how to custom ListBox Items with Dual-column or 
* multi-column.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\*****************************************************************************/

using System.Windows;
using Microsoft.Phone.Controls;
using System.Collections.ObjectModel;

namespace CSWP8ListBoxItemMultiColumn
{
    public partial class MainPage : PhoneApplicationPage
    {
        private ObservableCollection<MyListViewModel> myVM;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            Loaded += MainPage_Loaded;
        }

        /// <summary>
        /// Page loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadTestData();

            BindData();
        }

        /// <summary>
        /// Test data.
        /// </summary>
        private void LoadTestData()
        {
            myVM = new ObservableCollection<MyListViewModel>
                   {
                       new MyListViewModel
                           {
                               Field1 = "A1",
                               Field2 = "A2",
                               Field3 = "A3",
                               Field4 = "A4",
                               Field5 = "A5"
                           }, 
                       new MyListViewModel
                           {
                               Field1 = "B1",
                               Field2 = "B2",
                               Field3 = "B3",
                               Field4 = "B4",
                               Field5 = "B5"
                           }
                   };
        }

        /// <summary>
        /// Add item to listBox.
        /// </summary>
        /// <param name="sender">btnAdd</param>
        /// <param name="e"></param>
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            myVM.Add(new MyListViewModel
            {
                Field1 = tbInput.Text,
                Field2 = tbInput.Text,
                Field3 = tbInput.Text,
                Field4 = tbInput.Text,
                Field5 = tbInput.Text
            });

            BindData();
        }

        /// <summary>
        /// Data bind to ListBox
        /// </summary>
        private void BindData()
        {
            listData.ItemsSource = myVM;
            listForUserControl.ItemsSource = myVM;
        }     
    }

    /// <summary>
    /// Entity for test　Data
    /// </summary>
    public class MyListViewModel
    {
        public string Field1 { get; set; }

        public string Field2 { get; set; }

        public string Field3 { get; set; }

        public string Field4 { get; set; }

        public string Field5 { get; set; }
    }
}