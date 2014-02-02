/* 
    Copyright (c) 2012 - 2013 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://code.msdn.microsoft.com/wpapps
  
*/
#define DEBUG_AGENT
using System;
using System.Windows;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Info;
using System.Linq;

namespace ScheduledTaskAgent1
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

        /// <summary>
        /// Agent that runs a scheduled task
        /// </summary>
        /// <param name="task">
        /// The invoked task
        /// </param>
        /// <remarks>
        /// This method is called when a periodic or resource intensive task is invoked
        /// </remarks>
        protected override void OnInvoke(ScheduledTask task)
        {
            //TODO: Add code to perform your task in background
             string toastTitle = "";

            // If your application uses both PeriodicTask and ResourceIntensiveTask
            // you can branch your application code here. Otherwise, you don't need to.
            switch (task.Name)
            {
                case "ToastPeriodicAgent":
                    // Execute periodic task actions.
                    toastTitle = "Periodic ";
                    startToastTask(task, toastTitle);
                    break;
                case "TilePeriodicAgent":
                    // Execute periodic task actions.
                    startTileTask(task);
                    break;
                default:
                    // do nothing
                    break;
            }
        }

        private void startToastTask(ScheduledTask task, string toastTitle)
        {
            string toastMessage = "Current time: " + string.Format("{0:T}", DateTime.Now);

            // Launch a toast to show that the agent is running.
            // The toast will not be shown if the foreground application is running.
            ShellToast toast = new ShellToast();
            toast.Title = toastTitle;
            toast.Content = toastMessage;
            toast.Show();

            // If debugging is enabled, launch the agent again in one minute.
#if DEBUG_AGENT
            ScheduledActionService.LaunchForTest(task.Name, TimeSpan.FromSeconds(60));
#endif

            // Call NotifyComplete to let the system know the agent is done working.
            NotifyComplete();
        }

        private void startTileTask(ScheduledTask task)
        { 
            // You must use IconicTileData because TemplateIconic tile template is set in WMAppMainifest.xml.
            // However, if you change the tile template in WMAppMainifest.xml, you must change the constructor below to appropriately match, 
            // such as CycleTileData or StandardTileData.
            IconicTileData tileData = new IconicTileData    
            {
                IconImage = new Uri("ApplicationIcon.png", UriKind.RelativeOrAbsolute),
                Title = "My Tile title",
                Count = new Random().Next(1,10),
            };

            ShellTile mainTile = ShellTile.ActiveTiles.First();
            if (mainTile != null)
            {
                mainTile.Update(tileData);
            }

            // If debugging is enabled, launch the agent again in one minute.
#if DEBUG_AGENT
            ScheduledActionService.LaunchForTest(task.Name, TimeSpan.FromSeconds(60));
#endif

            // Call NotifyComplete to let the system know the agent is done working.
            NotifyComplete();
        }
    }
}
