/****************************** Module Header ******************************\
Module Name:  ScheduledAgent.cs
Project:      ScheduledDemoTaskAgent
Copyright (c) Microsoft Corporation.

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System.Windows;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;
using System;
using System.Threading;
using System.IO.IsolatedStorage;

namespace ScheduledDemoTaskAgent
{
    public class ScheduledAgent : ScheduledTaskAgent
    {
        private static volatile bool _classInitialized;

        /// <remarks>
        /// ScheduledAgent constructor, initializes the UnhandledException handler
        /// </remarks>
        public ScheduledAgent()
        {
            if (!_classInitialized)
            {
                _classInitialized = true;
                // Subscribe to the managed exception handler
                Deployment.Current.Dispatcher.BeginInvoke(delegate
                {
                    Application.Current.UnhandledException += ScheduledAgent_UnhandledException;
                });
            }
        }

        /// Code to execute on Unhandled Exceptions
        private void ScheduledAgent_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        protected override void OnInvoke(ScheduledTask task)
        {
            if (task.Name.Equals("PeriodicTaskDemo", StringComparison.OrdinalIgnoreCase))
            {   
                ShellToast toast = new ShellToast();
                Mutex mutex = new Mutex(true, "ScheduledAgentData");
                mutex.WaitOne();
                IsolatedStorageSettings setting = IsolatedStorageSettings.ApplicationSettings;
                toast.Title = setting["ScheduledAgentData"].ToString();
                mutex.ReleaseMutex();
                toast.Content = "Task Running";
                toast.Show();
            }
            ScheduledActionService.LaunchForTest(task.Name, TimeSpan.FromSeconds(3));
            NotifyComplete();
        }
    }
}