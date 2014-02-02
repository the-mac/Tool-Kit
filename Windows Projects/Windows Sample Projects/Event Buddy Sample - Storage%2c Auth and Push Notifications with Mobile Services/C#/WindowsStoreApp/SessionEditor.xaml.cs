// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

namespace EventBuddy
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using EventBuddy.DataModel;
    using Microsoft.Live;
    using Windows.Storage;
    using Windows.Storage.Pickers;
    using Windows.UI.Popups;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    public sealed partial class SessionEditor : UserControl
    {
        private CancellationTokenSource cancellationToken;
        private bool retrySelected;

        public SessionEditor()
        {
            this.InitializeComponent();
            this.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            VisualStateManager.GoToState(this, "hidden", false);
        }

        public delegate void CancelledEditor(object sender, CancelledEditorEventArgs args);

        public delegate void SaveEditor(object sender, SaveEditorEventArgs args);

        public event CancelledEditor Cancelled;

        public event SaveEditor Save;

        public static LiveConnectSession LiveSession { get; set; }

        public void InvokeCancelled()
        {
            var cncl = this.Cancelled;
            if (cncl != null)
            {
                cncl.Invoke(this, new CancelledEditorEventArgs());
            }
        }

        public void InvokeSave()
        {
            var accpt = this.Save;
            if (accpt != null)
            {
                accpt.Invoke(this, new SaveEditorEventArgs());
            }
        }

        public void Show()
        {
            this.Visibility = Windows.UI.Xaml.Visibility.Visible;
            this.FileNameTextBlock.Text = string.Empty;

            var session = this.DataContext as Session;
            if (session != null)
            {
                lblDialogTitle.Text = (session.Id != null) ? "Edit Session" : "Add Session";
            }

            VisualStateManager.GoToState(this, "shown", false);

            txtName.Focus(Windows.UI.Xaml.FocusState.Programmatic);
        }

        public void Hide()
        {
            VisualStateManager.GoToState(this, "hidden", false);
            this.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private static FileOpenPicker CreateFilePicker()
        {
            var filePicker = new FileOpenPicker()
            {
                ViewMode = PickerViewMode.List,
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };

            filePicker.FileTypeFilter.Add(".txt");
            filePicker.FileTypeFilter.Add(".ppt");
            filePicker.FileTypeFilter.Add(".pptx");
            filePicker.FileTypeFilter.Add(".doc");
            filePicker.FileTypeFilter.Add(".docx");

            return filePicker;
        }

        private static bool CheckIfFolderExists(ref string eventBuddyFolderId, LiveOperationResult foldersResult)
        {
            var folders = (List<object>)foldersResult.Result["data"];
            var isFound = false;
            var iterator = 0;

            while (!isFound && iterator < folders.Count())
            {
                dynamic folder = folders[iterator];

                if (folder.name == "EventBuddy")
                {
                    isFound = true;

                    eventBuddyFolderId = folder.id;
                }

                iterator++;
            }

            return isFound;
        }

        private static async Task<string> CreateFolderInSkydrive(LiveConnectClient client)
        {
            var folderData = new Dictionary<string, object>();

            folderData.Add("name", "EventBuddy");
            var createFolderResult = await client.PostAsync("me/skydrive", folderData);
            dynamic creationResult = createFolderResult.Result;

            return creationResult.id;
        }

        private void OnCancelClick(object sender, RoutedEventArgs e)
        {
            this.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            this.InvokeCancelled();
        }

        private void OnSaveClick(object sender, RoutedEventArgs e)
        {
            if (txtName.IsValid() && startDate.IsValid() && endDate.IsValid())
            {
                this.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                this.InvokeSave();
            }
        }

        private async void OnUploadClick(object sender, RoutedEventArgs e)
        {
            if (await this.LogInToSkydrive())
            {
                this.SetButtonsState(false);
                var filePicker = CreateFilePicker();
                var file = await filePicker.PickSingleFileAsync();

                if (file != null)
                {
                    var eventBuddyFolderId = string.Empty;
                    var client = new LiveConnectClient(LiveSession);
                    eventBuddyFolderId = await this.CreateFolderIfNotExists(client);

                    if (eventBuddyFolderId != string.Empty)
                    {
                        await this.UploadFileToSkydrive(file, eventBuddyFolderId, client);
                    }
                }

                this.SetButtonsState(true);
            }
        }

        private void SetButtonsState(bool state)
        {
            this.CancelButton.IsEnabled = state;
            this.SaveButton.IsEnabled = state;
        }

        private async Task UploadFileToSkydrive(StorageFile file, string eventBuddyFolderId, LiveConnectClient client)
        {
            using (var stream = await file.OpenStreamForReadAsync())
            {
                var progressHandler = this.InitializeProgressBar();
                var hasErrors = false;

                do
                {
                    try
                    {
                        hasErrors = false;
                        this.retrySelected = false;
                        LiveOperationResult result = await client.BackgroundUploadAsync(eventBuddyFolderId, file.Name, stream.AsInputStream(), OverwriteOption.Overwrite, this.cancellationToken.Token, progressHandler);
                        this.FileNameTextBlock.Text = file.Name;
                        await this.UpdateSession(client, result);
                    }
                    catch (LiveConnectException)
                    {
                        hasErrors = true;
                    }

                    if (hasErrors)
                    {
                        var errorTitle = "Upload file error";
                        await this.ShowMessage(errorTitle, "Exception occured while trying to upload the " + file.Name + " file to Skydrive.");
                    }

                    this.uploadProgress.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                }
                while (this.retrySelected);
            }
        }

        private async Task ShowMessage(string title, string message)
        {
            var messageDialog = new MessageDialog(message, title);

            messageDialog.Commands.Add(new UICommand("Retry", new UICommandInvokedHandler(this.RetryCommandInvokedHandler)));

            messageDialog.Commands.Add(new UICommand("Cancel"));

            messageDialog.DefaultCommandIndex = 0;

            messageDialog.CancelCommandIndex = 1;

            await messageDialog.ShowAsync();

            uploadProgress.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private void RetryCommandInvokedHandler(IUICommand command)
        {
            this.retrySelected = true;
        }

        private async Task<string> CreateFolderIfNotExists(LiveConnectClient client)
        {
            var eventBuddyFolderId = string.Empty;
            var hasErrors = false;

            do
            {
                try
                {
                    hasErrors = false;
                    this.retrySelected = false;

                    // Check if EventBuddy folder exists in SkyDrive, otherwise create it
                    var foldersResult = await client.GetAsync("/me/skydrive/files?filter=folders");
                    var isFound = CheckIfFolderExists(ref eventBuddyFolderId, foldersResult);

                    // If the folder was not found in Skydrive, then create it
                    if (!isFound)
                    {
                        eventBuddyFolderId = await CreateFolderInSkydrive(client);
                    }
                }
                catch (LiveConnectException)
                {
                    hasErrors = true;
                }

                if (hasErrors)
                {
                    var errorTitle = "Upload file error";
                    await this.ShowMessage(errorTitle, "Exception occured while checking if the shared folder exists in Skydrive.");
                }
            }
            while (this.retrySelected);

            return eventBuddyFolderId;
        }

        private async Task UpdateSession(LiveConnectClient client, LiveOperationResult result)
        {
            var session = this.DataContext as Session;

            LiveOperationResult operationResult = await client.GetAsync(result.Result["id"] + "/content?download=true");
            dynamic value = operationResult.Result;

            session.DeckSource = value.location.ToString();
        }

        private Progress<LiveOperationProgress> InitializeProgressBar()
        {
            uploadProgress.Value = 0;
            uploadProgress.Visibility = Windows.UI.Xaml.Visibility.Visible;
            this.cancellationToken = new CancellationTokenSource();

            var progressHandler = new Progress<LiveOperationProgress>((progress) =>
            {
                uploadProgress.Value = progress.ProgressPercentage;
            });

            return progressHandler;
        }

        private async Task<bool> LogInToSkydrive()
        {
            var hasErrors = false;

            do
            {
                try
                {
                    hasErrors = false;
                    this.retrySelected = false;

                    LiveAuthClient auth = new LiveAuthClient();
                    LiveLoginResult loginResult = await auth.LoginAsync(new string[] { "wl.signin", "wl.skydrive", "wl.skydrive_update" });

                    if (loginResult.Status == LiveConnectSessionStatus.Connected)
                    {
                        LiveSession = loginResult.Session;
                        return true;
                    }
                }
                catch (LiveAuthException)
                {
                    hasErrors = true;
                }

                if (hasErrors)
                {
                    var errorTitle = "Live Connect error";
                    await this.ShowMessage(errorTitle, "Exception occured while logging in to Skydrive.");
                }
            }
            while (this.retrySelected);

            return false;
        }

        public class CancelledEditorEventArgs : EventArgs
        {
        }

        public class SaveEditorEventArgs : EventArgs
        {
        }
    }
}
