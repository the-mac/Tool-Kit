/* 
    Copyright (c) 2012 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
using Microsoft.Phone.Scheduler;
using System;
using System.Diagnostics;
using System.Text;
using System.Windows;

namespace ScheduledTaskAgent1
{
  public class ScheduledAgent : ScheduledTaskAgent
  {
    /// <remarks>
    /// ScheduledAgent constructor, initializes the UnhandledException handler
    /// </remarks>

    static ScheduledAgent()
    {
      // Subscribe to the managed exception handler
      Deployment.Current.Dispatcher.BeginInvoke(delegate
      {
        Application.Current.UnhandledException += UnhandledException;
      });
    }

    /// Code to execute on Unhandled Exceptions
    private static void UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
    {
      if (Debugger.IsAttached)
      {
        // An unhandled exception has occurred; break into the debugger
        Debugger.Break();
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
      ServiceUploadHelper serviceUploadHelper = new ServiceUploadHelper();

      // This callback function will be called at the end of the upload cycle.
      serviceUploadHelper.InitializeServiceUpload(NotifyComplete);
    }
  }
}
