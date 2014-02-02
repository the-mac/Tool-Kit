/****************************** Module Header ******************************\
Module Name:  MainPage.xaml.cs
Project:      CSWP8ScheduledDemo
Copyright (c) Microsoft Corporation.

This example demonstrates how to use Scheduled Task in Windows phone.
It mainly covers 3 parts:

1.       How to create a scheduled task.
2.       How to catch the variety errors threw by scheduled task.
3.       How to set the process safe by class Mutex.
  
This source is subject to the Microsoft Public License.
See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/
using System;
using System.Windows;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Scheduler;
using System.Threading;
using System.IO.IsolatedStorage;

namespace CSWP8ScheduledTaskAgent
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }
        private void StartPeriodicTask()
        {
            PeriodicTask periodicTask = new PeriodicTask("PeriodicTaskDemo");
            periodicTask.Description = "Are presenting a periodic task";
            try
            {
                ScheduledActionService.Add(periodicTask);
                ScheduledActionService.LaunchForTest("PeriodicTaskDemo", TimeSpan.FromSeconds(3));
                MessageBox.Show("Open the background agent success");
            }
            catch (InvalidOperationException exception)
            {
                if (exception.Message.Contains("exists already"))
                {
                    MessageBox.Show("Since then the background agent success is already running");
                }
                if (exception.Message.Contains("BNS Error: The action is disabled"))
                {
                    MessageBox.Show("Background processes for this application has been prohibited");
                }
                if (exception.Message.Contains("BNS Error: The maximum number of ScheduledActions of this type has already been added."))
                {
                    MessageBox.Show("You open the daemon has exceeded the hardware limitations");
                }
            }
            catch (SchedulerServiceException)
            {

            }
        }
        private void StopPeriodicTask()
        {
            try
            {
                ScheduledActionService.Remove("PeriodicTaskDemo");
                MessageBox.Show("Turn off the background agent successfully");
            }
            catch (InvalidOperationException exception)
            {
                if (exception.Message.Contains("doesn't exist"))
                {
                    MessageBox.Show("Since then the background agent success is not running");
                }
            }
            catch (SchedulerServiceException)
            {

            }
        }
        private void StartPeriodicTask_Click(object sender, RoutedEventArgs e)
        {
            StartPeriodicTask();
            SetData();
        }
        private void StopPeriodicTask_Click(object sender, RoutedEventArgs e)
        {
            StopPeriodicTask();
        }
        public void SetData()
        {
            Mutex mutex = new Mutex(false, "ScheduledAgentData");
            mutex.WaitOne();
            IsolatedStorageSettings setting = IsolatedStorageSettings.ApplicationSettings;
            if (!setting.Contains("ScheduledAgentData"))
            {
                setting.Add("ScheduledAgentData", "Foreground data");
            }
            mutex.ReleaseMutex();
        }
    }
}