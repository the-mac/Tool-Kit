// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

namespace EventBuddy.Helpers
{
    using System;
    using Windows.UI.Popups;

    public class Utils
    {
        public static async void ShowNotImplementedMessage()
        {
            var messageDialog = new MessageDialog(string.Empty, "This feature is not implemented.");

            messageDialog.Commands.Add(new UICommand("Ok"));

            messageDialog.DefaultCommandIndex = 0;

            messageDialog.CancelCommandIndex = 1;

            await messageDialog.ShowAsync();
        }
    }
}
