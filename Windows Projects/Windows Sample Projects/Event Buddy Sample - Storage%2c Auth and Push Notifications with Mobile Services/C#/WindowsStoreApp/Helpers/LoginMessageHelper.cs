// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

namespace EventBuddy.Helpers
{
    using System;
    using EventBuddy.DataModel;

    public static class LoginMessageHelper
    {
        public static string GetLoginMessage()
        {
            if (string.IsNullOrEmpty(User.Current.UserId))
            {
                return string.Empty;
            }
            else
            {
                var provider = User.Current.UserId.Split(':')[0];

                if (provider.Equals("Twitter", StringComparison.OrdinalIgnoreCase))
                {
                    return "Logged in with Twitter";
                }
                else if (provider.Equals("Facebook", StringComparison.OrdinalIgnoreCase))
                {
                    return "Logged in with Facebook";
                }
                else
                {
                    return "Logged as " + User.Current.UserId;
                }
            }
        }
    }
}
