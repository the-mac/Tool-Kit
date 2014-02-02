// IpcWrapper.cpp
#include "pch.h"
#include "NamedEvent.h"
#include <ppltasks.h>

using namespace IpcWrapper;
using namespace Windows::Foundation;
using namespace Platform;
using namespace concurrency;

// Standard access for a named event (can wait on it and signal it)
#define DESIRED_ACCESS (SYNCHRONIZE | EVENT_MODIFY_STATE)

// To convert nanoseconds (from WinRT TimeSpan) into milliseconds (WaitForSingleObjectEx)
const int NANOSECONDS_TO_MILLISECONDS = 10000;

// Open a named event; throws if can't be opened
HANDLE Open(String^ name)
{
  HANDLE h = OpenEvent(DESIRED_ACCESS, FALSE, name->Data());
  if (h != NULL)
    return h;

  auto e = GetLastError();
  throw ref new Exception(HRESULT_FROM_WIN32(e));
}

// Creates a named event 
HANDLE Create(String^ name, bool autoReset, bool* previouslyCreated)
{
  auto flags = autoReset ? 0 : CREATE_EVENT_MANUAL_RESET;
  *previouslyCreated = false;

  HANDLE h = CreateEventEx(NULL, name->Data(), flags, DESIRED_ACCESS);
  if (h != NULL)
  {
    if (GetLastError() == ERROR_ALREADY_EXISTS)
      *previouslyCreated = true;

    return h;
  }

  auto e = GetLastError();
  throw ref new Exception(HRESULT_FROM_WIN32(e));
}

// Calls WaitForSingleObjectEx in a WinRT-awaitable fashion
IAsyncOperation<WaitAsyncResult>^ WaitHelper(HANDLE handle, DWORD milliseconds)
{
  return create_async([handle, milliseconds]()
  {
    auto x = WaitForSingleObjectEx(handle, milliseconds, FALSE);
    if (x == WAIT_OBJECT_0)
      return WaitAsyncResult::Success;
    else if (x == WAIT_TIMEOUT)
      return WaitAsyncResult::Timeout;

    throw ref new FailureException(L"Unknown return from WaitForSingleObjectEx!");
  });
}

// Attempts to open an already-existing named event
bool NamedEvent::TryOpen(Platform::String^ name, NamedEvent^* namedEvent)
{
  HANDLE h = OpenEvent(DESIRED_ACCESS, FALSE, name->Data());
  if (h == NULL)
    return false;

  *namedEvent = ref new NamedEvent(name, h);
  return true;
}

// Simple constructor to open existing event
NamedEvent::NamedEvent(String^ name) : m_name(name), m_event(Open(name)), m_previouslyCreated(true)
{
}

// Constructor to create a new event
NamedEvent::NamedEvent(String^ name, bool autoReset) : m_name(name), m_event(Create(name, autoReset, &m_previouslyCreated))
{
}

// Private constructor to take ownership of a handle
NamedEvent::NamedEvent(String^ name, HANDLE handle) : m_name(name), m_event(handle), m_previouslyCreated(true)
{
}

// Set the event
void NamedEvent::Set()
{
  ClosedCheck();
  SetEvent(m_event);
}

// Reset the event
void NamedEvent::Reset()
{
  ClosedCheck();
  ResetEvent(m_event);
}

// Wait indefinitely
IAsyncOperation<WaitAsyncResult>^ NamedEvent::WaitAsync()
{
  ClosedCheck();
  return WaitHelper(m_event, INFINITE);
}

// Wait for specified timespan
IAsyncOperation<WaitAsyncResult>^ NamedEvent::WaitAsync(TimeSpan timeout)
{
  ClosedCheck();
  auto ms = (DWORD)(timeout.Duration / NANOSECONDS_TO_MILLISECONDS);
  return WaitHelper(m_event, ms);
}

// Release the Win32 handle and make the object useless
void NamedEvent::Close()
{
  if (m_event != NULL)
  {
    CloseHandle(m_event);
    m_event = NULL;
  }
}

// Simple check for use of a disposed object
void NamedEvent::ClosedCheck()
{ 
  if (m_event == NULL) throw ref new Platform::ObjectDisposedException(); 
}

// Destructor
NamedEvent::~NamedEvent()
{
  Close();
}
