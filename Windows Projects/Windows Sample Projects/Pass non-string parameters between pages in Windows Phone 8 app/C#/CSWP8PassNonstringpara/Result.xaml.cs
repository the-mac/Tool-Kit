/****************************** Module Header ******************************\
* Module Name:    Result.xaml.cs
* Project:        CSWP8PassNonstringpara
* Copyright (c) Microsoft Corporation
*
* This sample will demo how to pass no-string data between two pages.
* This is result page.
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
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using System.IO.IsolatedStorage;
using System.IO;
using System.Runtime.Serialization.Json;

namespace CSWP8PassNonstringpara
{
    public partial class Result : PhoneApplicationPage
    {
        public Result()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Store test data.
            List<string> listString = new List<string>();

            // Request parameter. The identification of the source page.
            string parameter = NavigationContext.QueryString["name"];
                     
            switch (parameter)
            {
                case "1":
                    var myParameter = NavigationService.GetLastNavigationData();

                    if (myParameter != null)
                    {
                        listString = (List<string>)myParameter;
                    }
                    break;
                case "2":
                    if (App.ObjectNavigationData != null)
                    {
                        listString = (List<string>)App.ObjectNavigationData;
                    }
                    
                    // Reset.
                    App.ObjectNavigationData = null;
                    break;
                case "3":
                    string filePath = "objectData";
                    using (IsolatedStorageFile isolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        if (isolatedStorage.FileExists(filePath))
                        {                         
                            using (IsolatedStorageFileStream fileStream = isolatedStorage.OpenFile(filePath, FileMode.Open, FileAccess.Read))
                            {
                                string strObjData = string.Empty;
                                using (StreamReader reader = new StreamReader(fileStream))
                                {
                                    strObjData = reader.ReadToEnd();
                                }
                                byte[] byteArray = System.Text.Encoding.Unicode.GetBytes(strObjData);
                                MemoryStream stream = new MemoryStream(byteArray);

                                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<string>));
                                listString = (List<string>)serializer.ReadObject(stream);
                            }
                        }

                        // Reset.
                        isolatedStorage.DeleteFile(filePath);
                    }
                    break;
                default:
                    break;
            }

            // Bind to control.
            longListSelector.ItemsSource = listString;
        }
    }
}