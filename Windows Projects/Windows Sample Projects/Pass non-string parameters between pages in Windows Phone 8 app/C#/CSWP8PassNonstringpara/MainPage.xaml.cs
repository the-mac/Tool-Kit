/****************************** Module Header ******************************\
* Module Name:    MainPage.xaml.cs
* Project:        CSWP8PassNonstringpara
* Copyright (c) Microsoft Corporation
*
* This sample will demo how to pass no-string data between two pages.
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
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using System.IO.IsolatedStorage;
using System.IO;
using System.Runtime.Serialization.Json;

namespace CSWP8PassNonstringpara
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Test data.
        List<string> listString = new List<string>();

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Test data initialization.
            for (int i = 0; i < 20; i++)
            {
                listString.Add("Current Item:" + i);
            }
        }

        /// <summary>
        /// Custom Navigation Extensions.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMethod1_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate("/Result.xaml?name=1", listString);
        }

        /// <summary>
        /// Static properties
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMethod2_Click(object sender, RoutedEventArgs e)
        {
            App.ObjectNavigationData = listString;
            NavigationService.Navigate(new Uri("/Result.xaml?name=2", UriKind.Relative));
        }

        /// <summary>
        /// Json + IsolatedStorage
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMethod3_Click(object sender, RoutedEventArgs e)
        {
            string filePath = "objectData";

            using (IsolatedStorageFile isolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (isolatedStorage.FileExists(filePath))
                {
                    isolatedStorage.DeleteFile(filePath);
                }

                using (IsolatedStorageFileStream fileStream = isolatedStorage.OpenFile(filePath, FileMode.Create, FileAccess.Write))
                {
                    string writeDate = string.Empty;

                    // Json serialization.
                    using (MemoryStream ms = new MemoryStream())
                    {
                        var ser = new DataContractJsonSerializer(typeof(List<string>));
                        ser.WriteObject(ms, listString);
                        ms.Seek(0, SeekOrigin.Begin);
                        var reader = new StreamReader(ms);
                        writeDate = reader.ReadToEnd();
                    }

                    // Save to IsolatedStorage.
                    using (StreamWriter writer = new StreamWriter(fileStream))
                    {
                        writer.WriteLine(writeDate);
                    }
                }
            }

            NavigationService.Navigate(new Uri("/Result.xaml?name=3", UriKind.Relative));
        }

    }
}