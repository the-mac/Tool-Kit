/* 
    Copyright (c) 2012 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
#include "pch.h"
#include "FastFilters.h"
#include <robuffer.h>
#include <ppltasks.h>

#if NETFX_CORE
using namespace FastFiltersWin8;
#else
using namespace FastFilters;
#endif
using namespace Platform;
using namespace Windows::Storage::Streams;
using namespace concurrency;

Windows::Foundation::IAsyncAction^ ImageFilter::ToGrayScale(IBuffer^ buffer)
{
  if (buffer == nullptr)
    throw ref new Platform::NullReferenceException(L"buffer cannot be null");

  // To access the buffer, we need to use old-style COM to get an interface to IBufferByteAccess
  IUnknown* pUnk = reinterpret_cast<IUnknown*>(buffer);
  IBufferByteAccess* pAccess = NULL;
  byte* bytes = NULL;
  HRESULT hr = pUnk->QueryInterface(__uuidof(IBufferByteAccess), (void **)&pAccess);
  if (SUCCEEDED(hr))
  {
    hr = pAccess->Buffer(&bytes);
    if (SUCCEEDED(hr))
    {
      // Simple async task to update all the pixels to the average of RGB.
      return create_async([=]()
      {
        auto length = buffer->Length;
        for (unsigned int* p = (unsigned int*) bytes; p < (unsigned int*)(bytes + length); p++)
        {
          unsigned int average = (((*p & 255) + ((*p >> 8) & 255) + ((*p >> 16) & 255)) / 3);
          *p = (average + (average << 8) + (average << 16) + (255 << 24));
        }

        pAccess->Release();
      });
    }
    else
    {
      pAccess->Release();
      throw ref new Platform::Exception(hr, L"Couldn't get bytes from the buffer");
    }
  }
  else
    throw ref new Platform::Exception(hr, L"Couldn't access the buffer");
}

Windows::Foundation::IAsyncAction^ ImageFilter::Blur(IBuffer^ buffer, int pixelheight, int pixelwidth)
{
  if (buffer == nullptr || pixelheight == 0 || pixelwidth == 0)
    throw ref new Platform::NullReferenceException(L"Invalid image");

  // To access the buffer, we need to use old-style COM to get an interface to IBufferByteAccess
  IUnknown* pUnk = reinterpret_cast<IUnknown*>(buffer);
  IBufferByteAccess* pAccess = NULL;
  byte* bytes = NULL;
  HRESULT hr = pUnk->QueryInterface(__uuidof(IBufferByteAccess), (void **)&pAccess);
  if (SUCCEEDED(hr))
  {
    hr = pAccess->Buffer(&bytes);
    if (SUCCEEDED(hr))
    {
      // Simple async task to update all the pixels to the average of RGB.
      return create_async([=]()
      {
        auto length = buffer->Length;
        for (unsigned int* p = (unsigned int*) bytes; p < (unsigned int*)(bytes + length); p++)
        {
          unsigned int average = (((*p & 255) + ((*p >> 8) & 255) + ((*p >> 16) & 255)) / 3);
          *p = (average + (average << 8) + (average << 16) + (255 << 24));
        }

        pAccess->Release();
      });
    }
    else
    {
      pAccess->Release();
      throw ref new Platform::Exception(hr, L"Couldn't get bytes from the buffer");
    }
  }
  else
    throw ref new Platform::Exception(hr, L"Couldn't access the buffer");
}
