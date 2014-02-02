/****************************** Module Header ******************************\
* Module Name:    MainPage.xaml.cs
* Project:        CSWP8AzureServiceBusRest
* Copyright (c) Microsoft Corporation
*
* This sample demonstrates how to use the Windows Azure Service Bus on
* Windows Phone. 
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
using System.Net;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Text;

namespace CSWP8AzureServiceBusRest
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Your Service Namespace
        static string serviceNamespace = "[Your Service Namespace]";
        static string baseAddress = string.Empty;
        static string token = string.Empty;
        const string sbHostName = "servicebus.windows.net";
        const string acsHostName = "accesscontrol.windows.net";
        
        // Your issuer name
        static string issuerName = "[Your issuer name]";
        
        // Your issuer secret
        static string issuerSecret = "[Your issuer secret]";
        
        // Note that the realm used when requesting a token uses the HTTP scheme, even though
        // calls to the service are always issued over HTTPS
        static string realm = string.Empty;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            Loaded += MainPage_Loaded;
        }

        /// <summary>
        /// Update UI.
        /// </summary>
        /// <param name="textBlock">TextBlock</param>
        /// <param name="strTip">Text to display.</param>
        private void UpdateUIThread(TextBlock textBlock, string strTip)
        {
            Deployment.Current.Dispatcher.BeginInvoke(delegate()
            {
                textBlock.Text = strTip;
            });
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            baseAddress = "https://" + serviceNamespace + "." + sbHostName + "/";
            try
            {
                string acsEndpoint = "https://" + serviceNamespace + "-sb." + acsHostName + "/WRAPv0.9/";
                realm = "http://" + serviceNamespace + "." + sbHostName + "/";

                string postdata = "wrap_name=" + Uri.EscapeDataString(issuerName)
                    + "&wrap_password=" + Uri.EscapeDataString(issuerSecret)
                    + "&wrap_scope=" + Uri.EscapeDataString(realm);

                string strResponse = await post(acsEndpoint, postdata);
                var responseProperties = strResponse.Split('&');
                var tokenProperty = responseProperties[0].Split('=');
                token = Uri.UnescapeDataString(tokenProperty[1]);
                token = "WRAP access_token=\"" + token + "\"";
                UpdateUIThread(tbDebug, token);

                string queueName = "Queue" + Guid.NewGuid().ToString();
                CreateQueue(queueName, token);
                SendMessage(queueName, "msg1");
                ReceiveMessage(queueName);

                GetResources("$Resources/Queues");
            }
            catch (WebException we)
            {
                using (HttpWebResponse response = we.Response as HttpWebResponse)
                {
                    if (response != null)
                    {
                        Debug.WriteLine(new StreamReader(response.GetResponseStream()).ReadToEnd());
                    }
                    else
                    {
                        Debug.WriteLine(we.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// Generate Token.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postdata"></param>      
        /// <returns></returns>
        private async Task<string> post(string url, string postdata)
        {
            var request = WebRequest.Create(new Uri(url)) as HttpWebRequest;
            request.ContentType = "application/x-www-form-urlencoded";
            request.Method = "POST";

            byte[] data = Encoding.UTF8.GetBytes(postdata);
            request.ContentLength = data.Length;

            using (var requestStream = await Task<Stream>.Factory.FromAsync(request.BeginGetRequestStream, request.EndGetRequestStream, request))
            {
                await requestStream.WriteAsync(data, 0, data.Length);
            }

            WebResponse responseObject = await Task<WebResponse>.Factory.FromAsync(request.BeginGetResponse, request.EndGetResponse, request);
            var responseStream = responseObject.GetResponseStream();
            var sr = new StreamReader(responseStream);
            string received = await sr.ReadToEndAsync();

            return received;
        }

        /// <summary>
        /// Create Queue.
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private void CreateQueue(string queueName, string token)
        {
            // Create the URI of the new Queue, note that this uses the HTTPS scheme
            var queueAddress = baseAddress + queueName;
            WebClient webClient = new WebClient();
            webClient.Headers[HttpRequestHeader.Authorization] = token;

            // Prepare the body of the create queue request
            var putData = @"<entry xmlns=""http://www.w3.org/2005/Atom"">
                                          <title type=""text"">" + queueName + @"</title>
                                          <content type=""application/xml"">
                                            <QueueDescription xmlns:i=""http://www.w3.org/2001/XMLSchema-instance"" xmlns=""http://schemas.microsoft.com/netservices/2010/10/servicebus/connect"" />
                                          </content>
                                        </entry>";

            webClient.UploadStringCompleted += new UploadStringCompletedEventHandler((sender, e) =>
            {
                if (e.Error != null)
                {
                    return;
                }
                Debug.WriteLine("\nCreating queue {0}", queueAddress);
                Debug.WriteLine(e.Result);
            });

            webClient.UploadStringAsync(new Uri(queueAddress), "PUT", putData, token);
        }

        /// <summary>
        /// Send Message.
        /// </summary>
        /// <param name="relativeAddress"></param>
        /// <param name="body"></param>
        private void SendMessage(string relativeAddress, string body)
        {
            string fullAddress = baseAddress + relativeAddress + "/messages" + "?timeout=60";

            WebClient webClient = new WebClient();
            webClient.Headers[HttpRequestHeader.Authorization] = token;

            webClient.UploadStringCompleted += new UploadStringCompletedEventHandler((sender, e) =>
            {
                Debug.WriteLine("\nSending message {0} - to address {1}", body, fullAddress);
                if (e.Error != null)
                {
                    return;
                }
                Debug.WriteLine(e.Result);
            });

            webClient.UploadStringAsync(new Uri(fullAddress), "POST", body, token);
        }

        /// <summary>
        /// Receive Message.
        /// </summary>
        /// <param name="relativeAddress"></param>
        /// <returns></returns>
        private void ReceiveMessage(string relativeAddress)
        {
            string fullAddress = baseAddress + relativeAddress + "/messages/head" + "?timeout=60";
            DownloadData(fullAddress, string.Format("\nRetrieving message from {0}", fullAddress));
        }

        /// <summary>
        /// Get an Atom feed with all the queues in the namespace
        /// </summary>
        /// <param name="relativeAddress"></param>
        /// <returns></returns>
        private static void GetResources(string relativeAddress)
        {
            string fullAddress = baseAddress + relativeAddress;
            DownloadData(fullAddress, string.Format("\nGetting resources from {0}", fullAddress));
        }

        /// <summary>
        /// Download Data.
        /// </summary>
        /// <param name="fullAddress">Uri string.</param>
        /// <param name="strMessage">Tip message.</param>
        private static void DownloadData(string fullAddress, string strMessage)
        {
            WebClient webClient = new WebClient();
            webClient.Headers[HttpRequestHeader.Authorization] = token;

            webClient.DownloadStringCompleted += ((sender, e) =>
            {
                Debug.WriteLine(strMessage);
                if (e.Error != null)
                {
                    return;
                }
                Debug.WriteLine(e.Result);
            });
            webClient.DownloadStringAsync(new Uri(fullAddress));
        }
    }
}