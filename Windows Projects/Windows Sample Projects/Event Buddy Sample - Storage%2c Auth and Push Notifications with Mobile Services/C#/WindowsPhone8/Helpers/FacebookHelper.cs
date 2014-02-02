// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

namespace EventBuddy.WindowsPhone.Helpers
{
    using System;
    using System.Net;
    using Newtonsoft.Json.Linq;

    public static class FacebookHelper
    {
        private static Action<FacebookUser> callbackAction;
        private static Action failureCallbackAction;
        private static string facebookUrl = "http://graph.facebook.com/{0}";

        public static void RetrieveUserInformation(string userId, Action<FacebookUser> callback, Action failureCallback)
        {
            var client = new WebClient();
            var url = string.Format(facebookUrl, userId);
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
                var jsonResult = JObject.Parse(args.Result);
                var fullName = jsonResult.SelectToken("name").ToString();
                var userName = jsonResult.SelectToken("username").ToString();
                var userId = jsonResult.SelectToken("id").ToString();
                var url = string.Format(facebookUrl, userId);
                var imageUrl = string.Format("{0}/{1}", url, "picture");

                callbackAction(new FacebookUser { UserName = userName, FullName = fullName, PictureUrl = imageUrl });
            }
            catch (Exception)
            {
                failureCallbackAction();
            }
        }
    }
}
