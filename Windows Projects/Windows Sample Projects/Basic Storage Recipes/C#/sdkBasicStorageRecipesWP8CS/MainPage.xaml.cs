/* 
    Copyright (c) 2012 - 2013 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://code.msdn.microsoft.com/wpapps
  
*/
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using sdkBasicStorageRecipesWP8CS.Resources;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using Windows.ApplicationModel;
using Windows.Storage;

namespace sdkBasicStorageRecipesWP8CS
{
    public partial class MainPage : PhoneApplicationPage
    {
        #region Variables and constants

        App thisApp;
        StorageFolder localFolder;
        CameraCaptureTask cameraCaptureTask;
        PhotoChooserTask photoChooserTask;
        string output;

        const string OUTPUT_PAGE_URI = "/OutputPage.xaml";
        const string NEW_FOLDER_NAME = "TestFolder";
        const string TEXT_FILE_NAME = "SampleTextFile.txt";
        const string NEW_PHOTO_NAME = "CapturedPhoto.jpg";

        #endregion

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Get a reference to the running instance of this application.
            thisApp = Application.Current as App;

            // Get the local folder.
            localFolder = ApplicationData.Current.LocalFolder;
        }

        #region Button event handlers

        private async void btnGetInstallationFolder_Click(object sender, RoutedEventArgs e)
        {
            // Get the installation folder.
            StorageFolder installationFolder = Package.Current.InstalledLocation;
            
            // Enumerate the files and folders in the installation folder.
            output = await FileListHelper.EnumerateFilesAndFolders(installationFolder);
            NavigationService.Navigate(new Uri(OUTPUT_PAGE_URI, UriKind.Relative));
        }

        private async void btnGetLocalFolder_Click(object sender, RoutedEventArgs e)
        {
            // The code for this event handler has been refactored into a method
            // because it is called from multiple event handlers.
            await ListFilesInLocalFolderAsync();
        }

        private async void btnCreateFile_Click(object sender, RoutedEventArgs e)
        {
            // Create a new file with a random filename.
            string newFilePath = Path.GetTempFileName();
            string newFileName = Path.GetFileName(newFilePath);
            StorageFile newFile = await localFolder.CreateFileAsync(newFileName, CreationCollisionOption.ReplaceExisting);
            
            // Save the new file in the list of files created by this app.
            thisApp.filesToDelete.Add(newFile.Path);

            // Enumerate the files and folders in the local folder,
            // including the new file.
            await ListFilesInLocalFolderAsync();
        }

        private async void btnCreateFolder_Click(object sender, RoutedEventArgs e)
        {
            // Create a new folder with a default folder name.
            const string folderName = NEW_FOLDER_NAME;
            StorageFolder newFolder = await localFolder.CreateFolderAsync(folderName, CreationCollisionOption.ReplaceExisting);
            
            // Save the new folder in the list of folders created by this app.
            thisApp.foldersToDelete.Add(newFolder.Path);

            // Enumerate the files and folders in the local folder,
            // including the new folder.
            await ListFilesInLocalFolderAsync();
        }

        private async void btnWriteTextFile_Click(object sender, RoutedEventArgs e)
        {
            string textFilePath = await FileHelper.WriteTextFile(TEXT_FILE_NAME, AppResources.TextFileContent);

            // Save the new file in the list of files created by this app.
            thisApp.filesToDelete.Add(textFilePath);

            // Enumerate the files and folders in the local folder,
            // including the new file.
            await ListFilesInLocalFolderAsync();
        }

        private async void btnReadTextFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                output = await FileHelper.ReadTextFile(TEXT_FILE_NAME);

                // Display the contents of the text file.
                NavigationService.Navigate(new Uri(OUTPUT_PAGE_URI, UriKind.Relative));
            }
            catch (FileNotFoundException exc)
            {
                MessageBox.Show(AppResources.TextFileNotFoundExceptionMessage + "\n\n" + exc.Message, AppResources.TextFileNotFoundExceptionCaption, MessageBoxButton.OK);
            }
        }
        private void btnCopyExistingPhoto_Click(object sender, RoutedEventArgs e)
        {
            // Start the photo chooser task.
            photoChooserTask = new PhotoChooserTask();
            photoChooserTask.Completed += new EventHandler<PhotoResult>(photoChooserTask_Completed);
            photoChooserTask.Show();
        }

        async void photoChooserTask_Completed(object sender, PhotoResult e)
        {
            // If the user chose an existing photo,
            // save the photo to the local folder with its original filename.
            if (e.TaskResult == TaskResult.OK)
            {
                string fileName = Path.GetFileName(e.OriginalFileName);
                string photoFilePath = await FileHelper.SavePhoto(e.ChosenPhoto, fileName);

                // Save the new file in the list of files created by this app.
                thisApp.filesToDelete.Add(photoFilePath);
                
                await ListFilesInLocalFolderAsync();
            }
        }

        private void btnSaveNewPhoto_Click(object sender, RoutedEventArgs e)
        {
            // Start the camera capture task.
            cameraCaptureTask = new CameraCaptureTask();
            cameraCaptureTask.Completed += new EventHandler<PhotoResult>(cameraCaptureTask_Completed);
            cameraCaptureTask.Show();
        }

        async void cameraCaptureTask_Completed(object sender, PhotoResult e)
        {
            // If the user took a new photo and accepted it,
            // save the photo to the local folder with a default filename.
            if (e.TaskResult == TaskResult.OK)
            {
                const string fileName = NEW_PHOTO_NAME;
                string photoFilePath = await FileHelper.SavePhoto(e.ChosenPhoto, fileName);

                // Save the new file in the list of files created by this app.
                thisApp.filesToDelete.Add(photoFilePath);

                await ListFilesInLocalFolderAsync();
            }
        }

        private async void btnCleanup_Click(object sender, RoutedEventArgs e)
        {
            await FileListHelper.Cleanup();

            // Enumerate the files and folders in the local folder.
            await ListFilesInLocalFolderAsync();
        }

        #endregion

        private async Task ListFilesInLocalFolderAsync()
        {
            // Enumerate the files and folders in the local folder.
            output = await FileListHelper.EnumerateFilesAndFolders(localFolder);
            NavigationService.Navigate(new Uri(OUTPUT_PAGE_URI, UriKind.Relative));
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            if (e.Content is OutputPage)
            {
                // Pass the string output to the OutputPage
                // to be displayed in a TextBlock.
                (e.Content as OutputPage).Output = output;
            }
        }
    }
}
