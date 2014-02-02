/****************************** Module Header ******************************\
Module Name:  MainPage.xaml.cs
Project:      CSAzureServiceBusWithWindowsPhone
Copyright (c) Microsoft Corporation.
 
The sample code demonstrates how to expose an on-premise REST service to Internet
via Service Bus, then you can access this service by Windows Phone application.
The service includes normal string, generics and image methods.

This is the Windows Phone Application MainPage, here we use three buttons to call
Service Bus service and display the result on the phone page.

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL
All other rights reserved.
 
THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/



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
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using ServiceBusServices;
using System.Xml;
using System.Windows.Media.Imaging;

namespace CSAzureServiceBusWithWindowsPhone
{
    public partial class MainPage : PhoneApplicationPage
    {
        HttpWebRequest request;
        static string servicebusNamespace = "[Your Namespace]";
        string helloUri = String.Format("http://{0}.servicebus.windows.net/Hello?name=New User", servicebusNamespace);
        string personUri = String.Format("http://{0}.servicebus.windows.net/Person", servicebusNamespace);
        string imageUri = String.Format("http://{0}.servicebus.windows.net/Image", servicebusNamespace);
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Invoke Hello service method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            request = (HttpWebRequest)HttpWebRequest.Create(helloUri);
            request.Method = "GET";
            btnHello.Content = "Wait for your information..";
            IAsyncResult result = request.BeginGetResponse(new AsyncCallback(Process), "");
        }

        /// <summary>
        /// Give information to button's content property.
        /// </summary>
        /// <param name="result"></param>
        private void Process(IAsyncResult result)
        {
            HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(result);
            Stream stream = response.GetResponseStream();
            string s = string.Empty;
            using (StreamReader reader = new StreamReader(stream))
            {
                s = reader.ReadToEnd();
                s = Regex.Replace(s, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
            }
            Dispatcher.BeginInvoke(new Action(() => 
            {
                btnHello.Content = s;
                btnPerson.Content = "Get Person";
                btnImage.Content = "Get Image";
                lstData.Visibility = Visibility.Collapsed;
                imgSource.Visibility = Visibility.Collapsed;
            }));
        }

        /// <summary>
        /// Invoke Person service method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, RoutedEventArgs e)
        {
            request = (HttpWebRequest)HttpWebRequest.Create(personUri);
            request.Method = "GET";
            btnPerson.Content = "Wait for your information..";
            IAsyncResult result = request.BeginGetResponse(new AsyncCallback(PersonProcess), "");
        }

        /// <summary>
        /// Give Person model class entity information to ListBox control.
        /// </summary>
        /// <param name="result"></param>
        private void PersonProcess(IAsyncResult result)
        {
            Dispatcher.BeginInvoke(new Action(() =>
                {
                    lstData.Items.Clear();
                }));
            HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(result);
            Stream stream = response.GetResponseStream();
            string str = string.Empty;
            List<Person> list = new List<Person>();
            using (StreamReader reader = new StreamReader(stream))
            {
                str += reader.ReadToEnd();

                MatchCollection collection = Regex.Matches(str, @"<Person.*?>(.*?)</Person>");
                foreach (Match c in collection)
                {
                    if (!c.Value.Equals(string.Empty))
                    {
                        Person person = new Person();
                        person.Name = Regex.Replace(Regex.Match(c.Value, @"<Name.*?>(.*?)</Name>").Value, @"<[^>]*>", string.Empty, RegexOptions.IgnoreCase);
                        person.Comments = Regex.Replace(Regex.Match(c.Value, @"<Comments>((\s)*(.*?)(\s)*(.*?)(\s)*(.*?)(\s)*(.*?)(\s)*)</Comments>").Value, @"<[^>]*>", string.Empty, RegexOptions.IgnoreCase);
                        list.Add(person);
                    }
                }

                Dispatcher.BeginInvoke(new Action(() =>
                    {
                        foreach (Person p in list)
                        {
                            lstData.Items.Add("Person Name:" + p.Name);
                            lstData.Items.Add("Person Comments:" + p.Comments);
                            btnPerson.Content = "OK";
                            lstData.Visibility = Visibility.Visible;
                            imgSource.Visibility = Visibility.Collapsed;
                            btnImage.Content = "Get Image";
                            btnHello.Content = "Get Hello";
                        }
                    }));
            }

        }

        /// <summary>
        /// Invoke Image service method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, RoutedEventArgs e)
        {
            request = (HttpWebRequest)HttpWebRequest.Create(imageUri);
            request.Method = "GET";
            btnImage.Content = "Wait for your image..";
            IAsyncResult result = request.BeginGetResponse(new AsyncCallback(ImageProcess), null);
        }

        /// <summary>
        /// Give Image stream to Image control.
        /// </summary>
        /// <param name="result"></param>
        private void ImageProcess(IAsyncResult result)
        {
            HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(result);
            Stream stream = response.GetResponseStream();
            Dispatcher.BeginInvoke(new Action(() =>
            {
                BitmapImage source = new BitmapImage();
                source.SetSource(stream);
                imgSource.Source = source;
                lstData.Visibility = Visibility.Collapsed;
                imgSource.Visibility = Visibility.Visible;
                btnImage.Content = "OK";
                btnPerson.Content = "Get Person";
                btnHello.Content = "Get Hello";
            }));

        }
    }
}