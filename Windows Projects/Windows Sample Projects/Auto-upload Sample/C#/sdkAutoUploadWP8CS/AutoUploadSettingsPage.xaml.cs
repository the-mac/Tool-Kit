/* 
    Copyright (c) 2012 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;

namespace sdkAutoUploadWP8CS
{
  public partial class AutoUploadSettingsPage : PhoneApplicationPage
  {
    public string agentName = "ScheduledAgent";
    
    // When a user navigates to this page, determine if the agent is running.
    // Visually indicate the agent's status.
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      checkbox.IsChecked = isAgentRunning();
    }

    public AutoUploadSettingsPage()
    {
      InitializeComponent();
    }

    // Returns the status of the agent (i.e., running / not running).
    public bool isAgentRunning()
    {
      var lastTask = ScheduledActionService.Find(agentName) as ScheduledTask;

      if (null != lastTask) return true; else return false;
    }

    // Used to start the agent.
    public void startAgent()
    {
      stopAgent(); // Prevent two agents from running.

      ResourceIntensiveTask task = new ResourceIntensiveTask(agentName);
      task.Description = "This task uploads photos from the Camera Roll.";

      ScheduledActionService.Add(task);
    }

    public void stopAgent()
    {
      if (isAgentRunning()) // test to ensure we do not hit an exception
      {
        ScheduledActionService.Remove(agentName);
      }
    }

    // These handlers are used if the user taps on the check box.
    private void checkbox_Checked(object sender, RoutedEventArgs e)
    {
      startAgent();
    }

    private void checkbox_Unchecked(object sender, RoutedEventArgs e)
    {
      stopAgent();
    }
  }
}
