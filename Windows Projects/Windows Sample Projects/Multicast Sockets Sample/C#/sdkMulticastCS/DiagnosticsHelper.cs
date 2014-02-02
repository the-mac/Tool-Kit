/* 
    Copyright (c) 2011 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
using System;
using System.Windows;

namespace sdkMulticastCS
{
    /// <summary>
    /// This helper is used to make sure we always instantiate a MessageBox on the UI thread. 
    /// This is useful when you are dealing with alot of asynchronous operations and their callbacks. 
    /// These occur on a backgroudn thread and interacting with the UI from another thread is not allowed and will throw errors.
    /// </summary>
    public class DiagnosticsHelper
    {
        public static MessageBoxResult SafeShow(string messageBoxText, string caption, MessageBoxButton button)
        {
            MessageBoxResult result = MessageBoxResult.None;

            // Am I already on the UI thread?
            if (System.Windows.Deployment.Current.Dispatcher.CheckAccess())
            {
                result =  MessageBox.Show(messageBoxText, caption, button);
            }
            else
            {
                System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    result = MessageBox.Show(messageBoxText, caption, button);

                });
            }

            return result;
        }

        public static MessageBoxResult SafeShow(string messageBoxText)
        {
            return SafeShow(messageBoxText, String.Empty, MessageBoxButton.OK);
        }
    }
}
