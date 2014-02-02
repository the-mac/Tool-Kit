/****************************** Module Header ******************************\
* Module Name:    MainPage.xaml.cs
* Project:        CSWP8TimeoutHttpwebrequest
* Copyright (c) Microsoft Corporation
*
* This sample will demo how to set Timeout for httpwebrequest. 
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
using CSWP8TimeoutHttpwebrequest.Resources;
using System.Text;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace CSWP8TimeoutHttpwebrequest
{
    public partial class MainPage : PhoneApplicationPage
    {
        public static string strRequestUri = "http://www.contoso.com";       // Test URL
        private readonly int timeoutMilliseconds = 5 * 1000;                 // 5 seconds timeout
        public static ManualResetEvent allDone = new ManualResetEvent(false);// ManualResetEvent instance
        const int BUFFER_SIZE = 1024;                                        // Size of receive buffer.
        static string stringContent;                                         // Response string.

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            Thread t = new Thread(new ThreadStart(mainDelegate));
            t.IsBackground = true;
            t.Start();
        }

        public void mainDelegate()
        {
            Thread tr = Thread.CurrentThread;
            string strResult = string.Empty; // Store message.

            try
            {
                Uri uri = new Uri(strRequestUri);
                // Create a HttpWebrequest object to the desired URL.
                HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(uri);

                // Create an instance of the RequestState and assign the previous myHttpWebRequest1
                // object to it's request field.  
                RequestState myRequestState = new RequestState();
                myRequestState.request = myHttpWebRequest;

                // Set the event to nonsignaled state.
                allDone.Reset();

                Debug.WriteLine("\nRequest data!");

                // Start the asynchronous request.
                IAsyncResult result =
                  (IAsyncResult)myHttpWebRequest.BeginGetResponse(new AsyncCallback(RespCallback), myRequestState);

                bool isSuccess = allDone.WaitOne(timeoutMilliseconds);

                if (isSuccess)
                {
                    // Request succeeded before the timeout                  
                    UpdateUIThread(tbResult, "Loaded Successful!");
                    UpdateUIThread(tbResult, stringContent);
                }
                else
                {
                    // allDone wasn't signaled by ReadCallback; the request timed out   
                    if (myHttpWebRequest != null)
                    {
                        myHttpWebRequest.Abort();
                        // Return an error, etc.   
                        UpdateUIThread(tbResult, "Loaded Failed!");
                        return;
                    }
                }
            }
            catch (WebException e)
            {
                strResult += "Exception raised!\n";
                strResult += "Message: ";
                strResult += e.Message;
                strResult += "\nStatus: ";
                strResult += e.Status;
            }
            catch (Exception e)
            {
                strResult += "\nException raised!\n";
                strResult += "Message: ";
                strResult += e.Message;
            }
            finally
            {
                UpdateUIThread(tbResult, strResult);
            }
        }

        /// <summary>
        /// AsyncCallback delegate that is invoked when the asynchronous response is complete. 
        /// </summary>
        /// <param name="asynchronousResult"></param>
        private void RespCallback(IAsyncResult asynchronousResult)
        {
            try
            {
                // State of request is asynchronous.
                RequestState myRequestState = (RequestState)asynchronousResult.AsyncState;
                HttpWebRequest myHttpWebRequest2 = myRequestState.request;
                myRequestState.response = (HttpWebResponse)myHttpWebRequest2.EndGetResponse(asynchronousResult);

                // Read the response into a Stream object.
                Stream responseStream = myRequestState.response.GetResponseStream();
                myRequestState.streamResponse = responseStream;

                // Begin the Reading of the contents of the HTML page and print it to the console.
                IAsyncResult asynchronousInputRead = responseStream.BeginRead(myRequestState.BufferRead, 0, BUFFER_SIZE, new AsyncCallback(ReadCallBack), myRequestState);

            }
            catch (WebException e)
            {
                // Need to handle the exception
                UpdateUIThread(tbResult, e.Message);
            }
            catch (Exception e)
            {
                UpdateUIThread(tbResult, e.Message);
            }

        }

        /// <summary>
        /// AsyncCallback delegate that is invoked when the asynchronous read is complete. 
        /// </summary>
        /// <param name="asyncResult"></param>
        private void ReadCallBack(IAsyncResult asyncResult)
        {
            try
            {
                RequestState myRequestState = (RequestState)asyncResult.AsyncState;
                Stream responseStream = myRequestState.streamResponse;
                int read = responseStream.EndRead(asyncResult);

                // Read the HTML page and then do something with it
                if (read > 0)
                {
                    myRequestState.requestData.Append(Encoding.UTF8.GetString(myRequestState.BufferRead, 0, read));
                    IAsyncResult asynchronousResult = responseStream.BeginRead(myRequestState.BufferRead, 0, BUFFER_SIZE, new AsyncCallback(ReadCallBack), myRequestState);
                }
                else
                {
                    if (myRequestState.requestData.Length > 1)
                    {
                        stringContent = myRequestState.requestData.ToString();
                        // do something with the response stream here                   

                        //UpdateUIThread(tbResult, stringContent);
                    }

                    responseStream.Close();
                    // Signal the main thread to continue.
                    allDone.Set();
                }
            }
            catch (WebException e)
            {
                // Need to handle the exception
                UpdateUIThread(tbResult, e.Message);
            }
            catch (Exception e)
            {
                // Need to handle the exception
                UpdateUIThread(tbResult, e.Message);
            }
        }

        /// <summary>
        /// Write message to the UI thread.
        /// </summary>
        /// <param name="textBlock">The control to update.</param>
        /// <param name="strTip">The message to show.</param>
        private void UpdateUIThread(TextBlock textBlock, string strTip)
        {
            Deployment.Current.Dispatcher.BeginInvoke(delegate()
            {
                textBlock.Text = textBlock.Text + "\n" + strTip;
            });
        }
    }
}

/// <summary>
/// This class stores the State of the request.
/// </summary>
public class RequestState
{
    const int BUFFER_SIZE = 1024;
    public StringBuilder requestData;
    public byte[] BufferRead;
    public HttpWebRequest request;
    public HttpWebResponse response;
    public Stream streamResponse;

    public RequestState()
    {
        BufferRead = new byte[BUFFER_SIZE];
        requestData = new StringBuilder("");
        request = null;
        streamResponse = null;
    }
}

