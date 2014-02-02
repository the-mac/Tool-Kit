#pragma once
#include <Windows.h>

namespace IpcWrapper
{
  public enum class WaitAsyncResult
  {
    Success,
    Timeout,
  };

    /// <summary>
    /// Represents a Win32 named event object that can be used to signal across foreground / background processes
    /// </summary>
  public ref class NamedEvent sealed
  {
  public:
    /// <summary>
    /// Attempts to open an existing event, if it already exists
    /// </summary>
    /// <param name="name">The name of the event</param>
    /// <param name="namedEvent">Returns the event, if opened</param>
    /// <returns>True if the event was opened; false otherwise</returns>
    static bool TryOpen(Platform::String^ name, NamedEvent^* namedEvent);

    /// <summary>
    /// Opens an existing named event. The event must already exist
    /// </summary>
    /// <param name="name">The name of the event to open</param>
    NamedEvent(Platform::String^ name);

    /// <summary>
    /// Creates a new named event, or opens it if it already exists
    /// </summary>
    /// <param name="name">The name of the event to open</param>
    /// <param name="autoReset">True if this is an auto-reset event; false otherwise</param>
    /// <remarks>Use the PreviouslyCreated property to tell if this was previously created or not</remarks>
    NamedEvent(Platform::String^ name, bool autoReset);

    /// <summary>
    /// Signals the event. If it is an auto-reset event, will automatically be reset
    /// </summary>
    void Set();

    /// <summary>
    /// Resets the event. Only useful if this is a manual-reset event
    /// </summary>
    void Reset();

    /// <summary>
    /// Waits asynchronously for the event to become signalled
    /// </summary>
    /// <returns>Success when the event is signalled</returns>
    Windows::Foundation::IAsyncOperation<WaitAsyncResult>^ WaitAsync();

    /// <summary>
    /// Waits asynchronously for the event to become signalled, or until a timeout is reached
    /// </summary>
    /// <param name="timeout">The length of time to wait before giving up</param>
    /// <returns>Success when the event is signalled, or Timeout if the timeout expires</returns>
    Windows::Foundation::IAsyncOperation<WaitAsyncResult>^ WaitAsync(Windows::Foundation::TimeSpan timeout);

    /// <summary>
    /// Whether or not the event was created by someone else
    /// </summary>
    property bool PreviouslyCreated { bool get() { ClosedCheck(); return m_previouslyCreated; } }

    /// <summary>
    /// The name of the event
    /// </summary>
    property Platform::String^ Name { Platform::String^ get() { ClosedCheck(); return m_name; } }

    virtual ~NamedEvent();

  private:
    NamedEvent(Platform::String^ name, HANDLE handle);
    void Close();
    void ClosedCheck();

    Platform::String^ m_name;
    HANDLE m_event;
    bool m_previouslyCreated;
  };
}
