using System.Diagnostics;
using System.Windows;
using Microsoft.Phone.Scheduler;

using IpcWrapper;
using System.Threading;
using System;
using Microsoft.Phone.Shell;

namespace TestTask
{
  public class ScheduledAgent : ScheduledTaskAgent
  {
    const string STARTED_EVENT_NAME = "AGENT_STARTED";
    const string STOPPED_EVENT_NAME = "AGENT_STOPPED";
    const string RUNNING_EVENT_NAME = "AGENT_RUNNING";

    protected override void OnInvoke(ScheduledTask task)
    {

      Debug.WriteLine("*AGENT* Creating events and sleeping for 2 seconds");
      NamedEvent startedEvent = new NamedEvent(STARTED_EVENT_NAME, true);
      NamedEvent stoppedEvent = new NamedEvent(STOPPED_EVENT_NAME, true);
      NamedEvent runningEvent = new NamedEvent(RUNNING_EVENT_NAME, true);

      Debug.WriteLine("*AGENT* Press 'start waiting' button NOW!");
      Thread.Sleep(2000);

      Debug.WriteLine("*AGENT* Agent setting started / running events and taking 5");
      startedEvent.Set();
      runningEvent.Set();
      Thread.Sleep(5000);
      if (new Random().Next(3) <= 1)
      {
        Debug.WriteLine("*AGENT* Stopping normally");
        stoppedEvent.Set();
      }
      else
      {
        Debug.WriteLine("*AGENT* Simulating a crash; foreground should timeout");
      }

      Debug.WriteLine("*AGENT* Closing events and killing itself");
      startedEvent.Dispose();
      stoppedEvent.Dispose();
      runningEvent.Dispose();

      // Toast will pop if foreground app is not running
      var toast = new ShellToast();
      toast.Title = "Named Events";
      toast.Content = "Tap to cancel agent";
      toast.NavigationUri = new Uri("/MainPage.xaml", UriKind.Relative);
      toast.Show();

      NotifyComplete();
    }
  }
}