/****************************** Module Header ******************************\
* Module Name:    MainPage.xaml.cs
* Project:        CSWP8ProgressIndicator
* Copyright (c) Microsoft Corporation
*
* This sample will demo how to work with ProgressIndicator in WP8 and hide 
* it after the event.
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
using CSWP8ProgressIndicator.Resources;
using System.ComponentModel;
using System.Threading;

namespace CSWP8ProgressIndicator
{
    public partial class MainPage : PhoneApplicationPage
    {
        static string strMsg = string.Empty;   // Message of result.

        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        private void btnWork_Click(object sender, RoutedEventArgs e)
        {
            // Sample code to localize the ApplicationBar
            ProgressIndicator prog = new ProgressIndicator();
            prog.IsVisible = true;
            prog.IsIndeterminate = true;
            prog.Text = "Working...";
            SystemTray.SetProgressIndicator(this, prog);

            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                btnWork.Visibility = Visibility.Collapsed;
                tbMessage.Text = "The work process begins, please wait for 5 seconds.";
            });

            RunProcessAsync(DateTime.Now);
        }

        /// <summary>
        /// Run Process
        /// </summary>
        /// <param name="dumpDate"></param>
        public void RunProcessAsync(DateTime dumpDate)
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.RunWorkerAsync(dumpDate);
        }

        /// <summary>
        /// Handler for DoWork
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = new BackgroundWorker();

            // Do Work here. 
            Thread.Sleep(5 * 1000);  // 5 seconds;

            await worker.RunWorkerTaskAsync();
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            worker.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
            worker.DoWork -= new DoWorkEventHandler(worker_DoWork);

            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                tbMessage.Text = "Work is complete.";              
            });

            SystemTray.ProgressIndicator.IsVisible = false;
        }
    }
}