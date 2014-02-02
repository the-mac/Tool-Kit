/* 
    Copyright (c) 2012 - 2013 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://code.msdn.microsoft.com/wpapps
  
*/
using System;
using System.Net;
using System.IO;
using System.Text;

 


namespace SendTile
{
    public partial class SendTile : System.Web.UI.Page
    {
        /// <summary>
        /// Handle the Send Tile Notification button click event by forming the post message
        /// and sending it to the Microsoft Push Notification Server.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ButtonSendTile_Click(object sender, EventArgs e)
        {


            try
            {
                // Get the Uri that the Microsoft Push Notification Service returns to the Push Client when creating a notification channel.
                // Normally, a web service would listen for Uri's coming from the web client and maintain a list of Uri's to send
                // notifications out to.
                string subscriptionUri = TextBoxUri.Text.ToString();


                HttpWebRequest sendNotificationRequest = (HttpWebRequest)WebRequest.Create(subscriptionUri);

                // We will create a HTTPWebRequest that posts the tile notification to the Microsoft Push Notification Service.
                // HTTP POST is the only allowed method to send the notification.
                sendNotificationRequest.Method = "POST";

                // The optional custom header X-MessageID uniquely identifies a notification message. 
                // If it is present, the // same value is returned in the notification response. It must be a string that contains a UUID.
                // sendNotificationRequest.Headers.Add("X-MessageID", "<UUID>");

                // Create the tile message.
                string tileMessage = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                "<wp:Notification xmlns:wp=\"WPNotification\">" +
                    "<wp:Tile>" +
                      "<wp:BackgroundImage>" + TextBoxBackgroundImage.Text + "</wp:BackgroundImage>" +
                      "<wp:Count>" + TextBoxCount.Text + "</wp:Count>" +
                      "<wp:Title>" + TextBoxTitle.Text + "</wp:Title>" +
                      "<wp:BackBackgroundImage>" + TextBoxBackBackgroundImage.Text + "</wp:BackBackgroundImage>" +
                      "<wp:BackTitle>" + TextBoxBackTitle.Text + "</wp:BackTitle>" +
                      "<wp:BackContent>" + TextBoxBackContent.Text + "</wp:BackContent>" +
                   "</wp:Tile> " +
                "</wp:Notification>";

                // Sets the notification payload to send.
                byte[] notificationMessage = Encoding.Default.GetBytes(tileMessage);

                // Sets the web request content length.
                sendNotificationRequest.ContentLength = notificationMessage.Length;
                sendNotificationRequest.ContentType = "text/xml";
                sendNotificationRequest.Headers.Add("X-WindowsPhone-Target", "token");
                sendNotificationRequest.Headers.Add("X-NotificationClass", "1");


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
