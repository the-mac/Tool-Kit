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
using System.Net;
using System.Windows;
using System.Text;

namespace sdkMulticastCS
{
    public class UdpPacketReceivedEventArgs : EventArgs
    {
        public string Message { get; set; }
        public IPEndPoint Source { get; set; }

        public UdpPacketReceivedEventArgs(byte[] data, IPEndPoint source)
        {
            this.Message = Encoding.UTF8.GetString(data, 0, data.Length);
            this.Source = source;
        }
    }
}
