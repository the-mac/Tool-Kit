// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

namespace EventBuddy.Helpers
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Windows.Data.Json;

    public static class FacebookHelper
    {
        public static async Task<FacebookUser> RetrieveUserInformation(string userId)
        {
            var client = new HttpClient();
            var url = string.Format("http://graph.facebook.com/{0}", userId);
            var response = await client.GetAsync(new Uri(url));
            var responseString = await response.Content.ReadAsStringAsync();

            var jsonParsed = JsonValue.Parse(responseString);
            
            var fullName = jsonParsed.GetObject().GetNamedString("name");
            var userName = jsonParsed.GetObject().GetNamedString("username");
            var imageUrl = string.Format("{0}/{1}", url, "picture");

            return new FacebookUser { UserName = userName, FullName = fullName, PictureUrl = imageUrl };
        }
    }
}
