/* 
    Copyright (c) 2012 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
    To see all Code Samples for Windows Store apps, visit http://code.msdn.microsoft.com/windowsapps
  
*/
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;

namespace PixPresenterPortableLib
{
    /// <summary>
    /// Base class for all ViewModel classes. 
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        // Note the use of the CallerMemberName attribute.
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
        {
          if (Object.Equals(field, value))
            return false;

          field = value;
          RaisePropertyChanged(propertyName);
          return true;
        }

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
          var handler = PropertyChanged;
          if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;

        int busyLoading = 0;
        ManualResetEvent dataLoadedEvent = new ManualResetEvent(false);


        protected void InitializeOnceOrWait(Action action)
        {
          // Make sure we only load once
          int alreadyLoading = Interlocked.CompareExchange(ref busyLoading, 1, 0);
          if (alreadyLoading == 1)
          {
            dataLoadedEvent.WaitOne();
            return;
          }

          action();

          dataLoadedEvent.Set();
        }
    }
}
