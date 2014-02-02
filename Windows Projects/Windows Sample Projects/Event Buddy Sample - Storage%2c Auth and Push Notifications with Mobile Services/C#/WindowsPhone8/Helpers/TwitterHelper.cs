// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

namespace EventBuddy.WindowsPhone.Helpers
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Xml.Linq;

    public static class TwitterHelper
    {
        private static Action<TwitterUser> callbackAction;
        private static Action failureCallbackAction;

        public static void RetrieveUserInformation(string userId, Action<TwitterUser> callback, Action failureCallback)
        {
            var client = new WebClient();
            var url = string.Format("http://api.twitter.com/1/users/lookup.xml?user_id={0}", userId);

            callbackAction = callback;
            failureCallbackAction = failureCallback;
            client.DownloadStringCompleted += Client_DownloadStringCompleted;
            client.DownloadStringAsync(new Uri(url));
        }

        public static void Client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs args)
        {
            if (args.Error != null) 
            {
                failureCallbackAction();
            }

            try
            {
                var responseString = args.Result;

                var document = XDocument.Parse(responseString, LoadOptions.None);

                var screenName = document.Root.Descendants().Where(e => e.Name == "screen_name").First().Value;
                string imageUrl = document.Root.Descendants().Where(e => e.Name == "profile_image_url").First().Value;
                string handle = string.Format("@{0}", screenName);

                callbackAction(new TwitterUser { Handle = handle, PictureUrl = imageUrl });
            }
            catch (Exception)
            {
                failureCallbackAction();
            }
        }
    }
}