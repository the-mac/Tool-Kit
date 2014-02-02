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
using System.IO;
using System.Text;



namespace SendRaw
{
    public partial class SendRaw : System.Web.UI.Page
    {
        /// <summary>
        /// Event handler for the Send Raw Notification button.  Forms the post message and
        /// sends it to the Microsoft Push Notification Server.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ButtonSendRaw_Click(object sender, EventArgs e)
        {
            try
            {
                // Get the Uri that the Microsoft Push Notification Service returns to the Push Client when creating a notification channel.
                // Normally, a web service would listen for Uri's coming from the web client and maintain a list of Uri's to send
                // notifications out to.
                string subscriptionUri = TextBoxUri.Text.ToString();


                HttpWebRequest sendNotificationRequest = (HttpWebRequest)WebRequest.Create(subscriptionUri);

                // We will create a HTTPWebRequest that posts the raw notification to the Microsoft Push Notification Service.
                // HTTP POST is the only allowed method to send the notification.
                sendNotificationRequest.Method = "POST";

                // Create the raw message.
                string rawMessage = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                "<root>" +
                    "<Value1>" + TextBoxValue1.Text.ToString() + "<Value1>" +
                    "<Value2>" + TextBoxValue2.Text.ToString() + "<Value2>" +
                "</root>";

                // Sets the notification payload to send.
                byte[] notificationMessage = Encoding.Default.GetBytes(rawMessage);

                // Sets the web request content length.
                sendNotificationRequest.ContentLength = notificationMessage.Length;
                sendNotificationRequest.ContentType = "text/xml";
                sendNotificationRequest.Headers.Add("X-NotificationClass", "3");


                using (Stream requestStream = sendNotificationRequest.GetRequestStream())
                {
                    requestStream.Write(notificationMessage, 0, notificationMessage.Length);
                }

                // Send the notification and get the response.
                HttpWebResponse response = (HttpWebResponse)sendNotificationRequest.GetResponse();
                string notificationStatus = response.Headers["X-NotificationStatus"];
                string notificationChannelStatus = response.Headers["X-SubscriptionStatus"];
                string deviceConnectionStatus = response.Headers["X-DeviceConnectionStatus"];

                // Display the response from the Microsoft Push Notification Service.  
                // Normally, error handling code would be here.  In the real world, because data connections are not always available,
                // notifications may need to be throttled back if the device cannot be reached.
                TextBoxResponse.Text = notificationStatus + " | " + deviceConnectionStatus + " | " + notificationChannelStatus;
            }
            catch (Exception ex)
            {
                TextBoxResponse.Text = "Exception caught sending update: " + ex.ToString();
            }

        }
    }
}
