/* 
    Copyright (c) 2012 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Windows.Networking.Proximity;

namespace sdkURIAssociationWP8CS
{
    public partial class ProximityUserControl : UserControl
    {
        long _proximityMessageId;
        
        public ProximityUserControl(long proximityMessageId)
        {
            _proximityMessageId = proximityMessageId;
            InitializeComponent();
        }

        private void buttonCancelMessage_Click(object sender, RoutedEventArgs e)
        {
           // Create the proximity device.
           ProximityDevice device = ProximityDevice.GetDefault();

            // Make sure that NFC is supported.
           if (device != null)
           {
               // Stop publishing the Proximity message.
               device.StopPublishingMessage(_proximityMessageId);
           }

           // Close the Proximity "tap devices now" control.
           Popup p = this.Parent as Popup;
           p.IsOpen = false;
        }
    }
}
