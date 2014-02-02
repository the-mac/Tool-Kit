using IpcWrapper;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Scheduler;
using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace Win32NamedEventTest
{
  public partial class MainPage : PhoneApplicationPage
  {
    NamedEvent agentStartedEvent;
    NamedEventHelper agentRunning;
    DispatcherTimer timer;
    bool agentScheduled;
    bool waiting;

    // Same strings as used in the agent
    const string STARTED_EVENT_NAME = "AGENT_STARTED";
    const string STOPPED_EVENT_NAME = "AGENT_STOPPED";
    const string RUNNING_EVENT_NAME = "AGENT_RUNNING";

    public MainPage()
    {
      InitializeComponent();

      CheckAgentState();
      UpdateControls();
      StartTimerIfNecessary();
    }

    private void StartTimerIfNecessary()
    {
      if (!agentScheduled)
        return;

      LaunchAgent();
      timer = new DispatcherTimer();
      timer.Interval = TimeSpan.FromSeconds(15);
      timer.Tick += delegate { LaunchAgent(); };
      timer.Start();
    }

    private void CheckAgentState()
    {
      if (ScheduledActionService.Find("agent") != null)
        agentScheduled = true;
      else
        agentScheduled = false;
    }

    // This sample doesn't use databinding, but you could use ICommands instead
    void UpdateControls()
    {
      createAgent.IsEnabled = false;
      cancelAgent.IsEnabled = false;
      startWaiting.IsEnabled = false;
      stopWaiting.IsEnabled = false;

      if (agentScheduled)
      {
        cancelAgent.IsEnabled = true;
        if (agentStartedEvent == null)
        {
          startWaiting.IsEnabled = true;
          stopWaiting.Content = "stop waiting";
        }
        else
          stopWaiting.IsEnabled = true;
      }
      else
        createAgent.IsEnabled = true;
    }

    private void CreateClicked(object sender, RoutedEventArgs e)
    {
      Debug.WriteLine("Creating agent...");

      PeriodicTask t = new PeriodicTask("agent");
      t.Description = "test for IPC";
      ScheduledActionService.Add(t);
      agentScheduled = true;

      StartTimerIfNecessary();

      UpdateControls();
    }

    /// <summary>
    /// TODO: Remember to remove any code like this before sumbitting your app to marketplace
    /// LaunchForTest is not supported in non-developer apps
    /// </summary>
    void LaunchAgent()
    {
      Debug.WriteLine("Launching agent shortly...");
      ScheduledActionService.LaunchForTest("agent", TimeSpan.FromSeconds(2));
    }

    private void StartClicked(object sender, RoutedEventArgs e)
    {
      // This just illustrates how to open an already-created event without actually
      // creating it. Normally you would just create the event (since the agent doesn't care)
      if (!NamedEvent.TryOpen(STARTED_EVENT_NAME, out agentStartedEvent))
      {
        Debug.WriteLine("Couldn't open yet; try again once created");
        return;
      }

      // Use the event-handler wrapper as well
      if (agentRunning == null)
        agentRunning = NamedEventHelper.GetEvent(RUNNING_EVENT_NAME);

      agentRunning.Signaled += AgentStartedRunning;

      UpdateControls();
      waiting = true;

      ThreadPool.QueueUserWorkItem(new WaitCallback(async delegate
        {
          // Simple "run until I tell you to stop" loop
          while (waiting)
          {
            // First, wait for the 'started' event to be signalled, and update the UI
            await agentStartedEvent.WaitAsync();

            // Check if we were asked to stop while we were waiting, and if so bail out
            if (!waiting)
              break;

            UpdateAgentUI(agentRunningText);
            WaitAsyncResult result;

            // Now wait for the 'stopped' event to be signalled. 
            // Note the agent will intentionally 'crash' by not signalling the stop event
            // approximatly one third of the time.
            // Show using a 'using' block to automatically dispose of the object
            using (var stopped = new NamedEvent(STOPPED_EVENT_NAME))
            {
              result = await stopped.WaitAsync(TimeSpan.FromSeconds(7));
            }

            // Again, bail if we were asked to stop
            if (!waiting)
              break;

            if (result == WaitAsyncResult.Timeout)
              UpdateAgentUI(agentCrashedText);
            else
              UpdateAgentUI(agentNotRunningText);
          }

          // Fell off the loop since we were told to stop
          agentStartedEvent.Dispose();
          agentStartedEvent = null;
          agentRunning.Signaled -= AgentStartedRunning;
          UpdateAgentUI(agentUnknownText);
        }));
    }

    void AgentStartedRunning(object sender, EventArgs e)
    {
      agentRunningStoryboard.Begin();
    }

    void UpdateAgentUI(UIElement visibleBlock)
    {
      Dispatcher.BeginInvoke(delegate
      {
        agentRunningText.Visibility = Visibility.Collapsed;
        agentNotRunningText.Visibility = Visibility.Collapsed;
        agentCrashedText.Visibility = Visibility.Collapsed;
        agentUnknownText.Visibility = Visibility.Collapsed;

        visibleBlock.Visibility = Visibility.Visible;
        UpdateControls();
      });
    }

    /// <summary>
    /// Stop waiting. Because the thread that is doing the work might currently
    /// be blocked on a Wait call, we disable the button and change its text
    /// (it will get reset once the waiting thread exits its loop)
    /// </summary>
    private void StopClicked(object sender, RoutedEventArgs e)
    {
      waiting = false;
      stopWaiting.IsEnabled = false;
      stopWaiting.Content = "stopping...";
    }

    private void CancelClicked(object sender, RoutedEventArgs e)
    {
      CancelAgent();
      waiting = false;
      UpdateControls();
      timer.Stop();
      timer = null;
    }

    private void CancelAgent()
    {
      if (ScheduledActionService.Find("agent") != null)
        ScheduledActionService.Remove("agent");

      agentScheduled = false;
    }
  }
}