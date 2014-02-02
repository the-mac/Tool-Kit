using IpcWrapper;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;

namespace Win32NamedEventTest
{
  /// <summary>
  /// Provides a .NET event-based wrapper around named Win32 events
  /// </summary>
  public class NamedEventHelper : IDisposable
  {
    // List of registered handlers. Note that values are NEVER removed from this
    // dictionary, so don't create too many named events.
    static Dictionary<string, NamedEventHelper> knownHelpers = new Dictionary<string, NamedEventHelper>();

    /// <summary>
    /// Returns an object that wraps an existing Win32 named event
    /// </summary>
    /// <param name="name">The name of the event</param>
    /// <returns>The wrapper</returns>
    /// <remarks>Throws ObjectDisposed if the event has already been disposed</remarks>
    public static NamedEventHelper GetEvent(string name)
    {
      NamedEventHelper helper;
      lock (typeof(NamedEventHelper))
      {
        if (!knownHelpers.TryGetValue(name, out helper))
        {
          helper = new NamedEventHelper(name);
          knownHelpers.Add(name, helper);
        }
      }
      if (helper.listening == false)
        throw new ObjectDisposedException("Event has already been disposed");

      return helper;
    }

    NamedEvent namedEvent;
    bool listening;

    /// <summary>
    /// Private constructor (you can only get instances from <see cref="GetEvent"/>
    /// </summary>
    /// <param name="name">The name of the event</param>
    /// <remarks>This creates the object and sets up an infinite loop
    /// that just waits on the Win32 event and then raises the <see cref="Signaled"/>
    /// event when it is set. The loop exits when the object is disposed.</remarks>
    NamedEventHelper(string name)
    {
      namedEvent = new NamedEvent(name);
      listening = true;
      ThreadPool.QueueUserWorkItem(async delegate
      {
        while (listening)
        {
          try
          {
            // wait forever
            await namedEvent.WaitAsync();
            if (!listening)
              break;

            Deployment.Current.Dispatcher.BeginInvoke(delegate
            {
              RaiseSignaled();
            });
          }
          catch
          {
            break;
          }
        }

        RaiseDisposed();
      });
    }

    /// <summary>
    /// Raises the Signaled event, if there are any listeners
    /// </summary>
    void RaiseSignaled()
    {
      var handler = Signaled;
      if (handler == null)
        return;

      handler(this, EventArgs.Empty);
    }

    /// <summary>
    /// Raises the Disposed event, if there are any listeners
    /// </summary>
    void RaiseDisposed()
    {
      var handler = Disposed;
      if (handler == null)
        return;

      handler(this, EventArgs.Empty);
    }

    /// <summary>
    /// Raised whenever the underlying Win32 event is set
    /// </summary>
    public event EventHandler Signaled;

    /// <summary>
    /// Raised when the object is disposed (and no more events will be raised)
    /// </summary>
    public event EventHandler Disposed;

    /// <summary>
    /// Cleanup
    /// </summary>
    public void Dispose()
    {
      listening = false;
      namedEvent.Dispose();
      namedEvent = null;
    }
  }
}
