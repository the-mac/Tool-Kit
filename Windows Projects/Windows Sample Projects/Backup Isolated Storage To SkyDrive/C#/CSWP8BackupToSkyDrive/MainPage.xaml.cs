/****************************** Module Header ******************************\
* Module Name:    MainPage.xaml.cs
* Project:        CSWP8BackupToSkyDrive
* Copyright (c) Microsoft Corporation
*
* This demo shows how you can save Isolated Storage's files to SkyDrive.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\*****************************************************************************/

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using System.Collections.Generic;
using Microsoft.Live;
using Microsoft.Live.Controls;
using System.IO;
using System.IO.IsolatedStorage;

namespace CSWP8BackupToSkyDrive
{
    public partial class MainPage : PhoneApplicationPage
    {
        CameraCaptureTask cameraCaputreTask = null;                     //CameraCaptureTask instance.
        private LiveConnectClient client = null;
        private LiveConnectSession session = null;
        private string strSkyDriveFolderName = "IsolatedStorageFolder"; // The folder name for backups
        private string strSkyDriveFolderID = string.Empty;              // The id of the folder name for backups
        private string fileID = null;                                   // The file id of your backup file
        private IsolatedStorageFileStream readStream = null;            // The stream for restoring data 
        private string fileName = "MyAppBackup.jpg";                    // Backup name for the capture image.     

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // SignIn button
            btnSignIn.ClientId = "00000000480E7666";
            btnSignIn.Scopes = "wl.basic wl.signin wl.offline_access wl.skydrive_update";
            btnSignIn.Branding = BrandingType.Windows;
            btnSignIn.TextType = ButtonTextType.SignIn;
            btnSignIn.SessionChanged += btnSignIn_SessionChanged;

            // CameraCaptureTask
            cameraCaputreTask = new CameraCaptureTask();
            cameraCaputreTask.Completed += cameraCaputreTask_Completed;
        }

        /// <summary>
        /// Show the camera application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnShowCamera_Click(object sender, RoutedEventArgs e)
        {
            cameraCaputreTask.Show();
        }

        /// <summary>
        /// Processing when the chooser task is completed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void cameraCaputreTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                try
                {
                    // Create a BitmapImage.
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.SetSource(e.ChosenPhoto);
                    // Display the image.
                    cameraImage.Source = bitmap;

                    // Write message to the UI thread.
                    UpdateUIThread(tbDebug, "Captured image available, saving picture.");

                    // Save picture as JPEG to isolated storage.
                    using (IsolatedStorageFile isStore = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        Uri uri = new Uri(fileName, UriKind.Relative);
                        string strTempFile = uri.ToString();

                        if (isStore.FileExists(strTempFile))
                        {
                            isStore.DeleteFile(strTempFile);
                        }

                        using (IsolatedStorageFileStream targetStream = isStore.OpenFile(strTempFile, FileMode.Create, FileAccess.Write))
                        {
                            WriteableBitmap wb = new WriteableBitmap(bitmap);
                            // Encode WriteableBitmap object to a JPEG stream.
                            Extensions.SaveJpeg(wb, targetStream, wb.PixelWidth, wb.PixelHeight, 0, 85);
                        }
                    }

                    // Write message to the UI thread.
                    UpdateUIThread(tbDebug, "Picture has been saved to isolated storage.");
                }
                finally
                {
                    // Close image stream
                    e.ChosenPhoto.Close();
                }
            }
        }

        /// <summary>
        /// Write message to the UI thread.
        /// </summary>
        /// <param name="textBlock">The control to update.</param>
        /// <param name="strTip">The message to show.</param>
        private void UpdateUIThread(TextBlock textBlock, string strTip)
        {
            Deployment.Current.Dispatcher.BeginInvoke(delegate()
            {
                textBlock.Text = strTip;
            });
        }

        /// <summary>
        /// Event triggered when Skydrive sign in status is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSignIn_SessionChanged(object sender, LiveConnectSessionChangedEventArgs e)
        {
            // If the user is signed in.
            if (e.Status == LiveConnectSessionStatus.Connected)
            {
                session = e.Session;
                client = new LiveConnectClient(e.Session);
                // Write message to the UI thread.
                UpdateUIThread(tbDebug, "Accessing SkyDrive...");

                // Get the folders in their skydrive.
                client.GetCompleted +=
                    new EventHandler<LiveOperationCompletedEventArgs>(btnSignin_GetCompleted);
                client.GetAsync("me/skydrive/files?filter=folders,albums");
            }
            else  // Otherwise the user isn't signed in.
            {
                // Write message to the UI thread.
                UpdateUIThread(tbDebug, "Not signed in.");
                client = null;
            }
        }

        /// <summary>
        /// Event for if the user just logged in.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnSignin_GetCompleted(object sender, LiveOperationCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                // Write message to the UI thread.
                UpdateUIThread(tbDebug, "Loading folder...");

                // Check all the folders in user's skydrive.
                Dictionary<string, object> folderData = (Dictionary<string, object>)e.Result;
                List<object> folders = (List<object>)folderData["data"];

                // Loop all folders to check if the isolatedstoragefolder exists.
                foreach (object item in folders)
                {
                    Dictionary<string, object> folder = (Dictionary<string, object>)item;
                    if (folder["name"].ToString() == strSkyDriveFolderName)
                        strSkyDriveFolderID = folder["id"].ToString();
                }

                // If the IsolatedStorageFolder does not exist, create it.
                if (strSkyDriveFolderID == string.Empty)
                {
                    Dictionary<string, object> skyDriveFolderData = new Dictionary<string, object>();
                    skyDriveFolderData.Add("name", strSkyDriveFolderName);
                    client.PostCompleted += new EventHandler<LiveOperationCompletedEventArgs>(CreateFolder_Completed);
                    // To create the IsolatedStorageFolder in Skydrive.
                    client.PostAsync("me/skydrive", skyDriveFolderData);

                    // Write message to the UI thread.
                    UpdateUIThread(tbDebug, "Creating folder...");
                }
                else  // Check if the backup file is in the IsolatedStorageFile
                {
                    // Write message to the UI thread.
                    UpdateUIThread(tbDebug, "Ready to backup.");
                    UpdateUIThread(tbDate, "Checking for previous backups...");
                    btnBackup.IsEnabled = true;

                    // Get the files' ID if they exists.
                    client = new LiveConnectClient(session);
                    client.GetCompleted += new EventHandler<LiveOperationCompletedEventArgs>(getFiles_GetCompleted);
                    // Get the file in the folder.
                    client.GetAsync(strSkyDriveFolderID + "/files");
                }
            }
            else
            {
                MessageBox.Show(e.Error.Message);
            }
        }

        /// <summary>
        /// Backup
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBackup_Click(object sender, RoutedEventArgs e)
        {
            if (client == null || client.Session == null)
            {
                MessageBox.Show("You must sign in first.");
            }
            else
            {
                if (MessageBox.Show("Are you sure you want to backup? This will overwrite your old backup file!", "Backup?", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    UploadFile();
                }
            }
        }

        /// <summary>
        /// The IsolatedStorageData folder have been created.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreateFolder_Completed(object sender, LiveOperationCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                // Write message to the UI thread.
                UpdateUIThread(tbDebug, "Ready to backup.");
                UpdateUIThread(tbDate, "No previous backup available.");

                Dictionary<string, object> folder = (Dictionary<string, object>)e.Result;
                // Get the folder ID.
                strSkyDriveFolderID = folder["id"].ToString();
                btnBackup.IsEnabled = true;
            }
            else
            {
                MessageBox.Show(e.Error.Message);
            }
        }

        /// <summary>
        /// Upload Files.
        /// </summary>
        public void UploadFile()
        {
            // The folder must exist, it should have already been created.
            if (strSkyDriveFolderID != string.Empty)
            {
                this.client.UploadCompleted
                    += new EventHandler<LiveOperationCompletedEventArgs>(IsFile_UploadCompleted);

                // Write message to the UI thread.
                UpdateUIThread(tbDebug, "Uploading backup...");
                UpdateUIThread(tbDate, "");

                try
                {
                    using (IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        // Upload many files.
                        foreach (string itemName in iso.GetFileNames())
                        {
                            fileName = itemName;
                            readStream = iso.OpenFile(fileName, FileMode.Open, FileAccess.Read);
                            client.UploadAsync(strSkyDriveFolderID, fileName, readStream, OverwriteOption.Overwrite, null);
                        }

                        // [-or-]

                        // Upload one file.
                        //readStream = iso.OpenFile(fileName, FileMode.Open, FileAccess.Read);
                        //client.UploadAsync(strSkyDriveFolderID, fileName, readStream, OverwriteOption.Overwrite, null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error accessing IsolatedStorage. Please close the app and re-open it, and then try backing up again!", "Backup Failed", MessageBoxButton.OK);

                    // Write message to the UI thread.
                    UpdateUIThread(tbDebug, ex.Message + ".Close the app and start again.");
                    UpdateUIThread(tbDate, "");
                }
            }
        }

        /// <summary>
        /// Check if the backup have finished.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void IsFile_UploadCompleted(object sender, LiveOperationCompletedEventArgs args)
        {
            if (args.Error == null)
            {
                // Write message to the UI thread.
                UpdateUIThread(tbDebug, "Backup complete.");
                // In order to prevent the crash when user click the backup button again, dispose the readStream.
                readStream.Dispose();
                // Write message to the UI thread.
                UpdateUIThread(tbDate, "Checking for new backup...");

                // Get the newly created fileID's (it will update the time too, and enable restoring).
                client = new LiveConnectClient(session);
                client.GetCompleted += new EventHandler<LiveOperationCompletedEventArgs>(getFiles_GetCompleted);
                client.GetAsync(strSkyDriveFolderID + "/files");
            }
            else
            {
                // Write message to the UI thread.
                UpdateUIThread(tbDebug, "Error uploading file: " + args.Error.ToString());
            }
        }

        /// <summary>
        /// Check whether the backup file exists in the folder.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void getFiles_GetCompleted(object sender, LiveOperationCompletedEventArgs e)
        {
            List<object> data = (List<object>)e.Result["data"];

            // Write message to the UI thread.
            UpdateUIThread(tbDate, " Getting previous backup...");
            DateTimeOffset date = DateTimeOffset.MinValue;

            foreach (IDictionary<string, object> content in data)
            {
                if (((string)content["name"]).Equals(fileName))
                {
                    fileID = (string)content["id"];
                    try
                    {
                        date = DateTimeOffset.Parse(((string)content["updated_time"]).Substring(0, 19));
                    }
                    catch (Exception ex)
                    {
                        // Write message to the UI thread.
                        UpdateUIThread(tbDebug, ex.Message);
                    }

                    break;
                }
            }

            if (fileID != null)
            {
                try
                {
                    string strTemp =
                        (date != DateTimeOffset.MinValue) ? "Last backup on " + date.Add(date.Offset).DateTime : "Last backup on: unknown";

                    UpdateUIThread(tbDate, strTemp);
                }
                catch (Exception ex)
                {
                    // Write message to the UI thread.
                    UpdateUIThread(tbDebug, ex.Message);
                    UpdateUIThread(tbDate, "Last backup on: unknown");
                }
            }
            else
            {
                UpdateUIThread(tbDate, "No previous backup available");
            }
        }
    }
}