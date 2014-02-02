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
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Windows.Storage;

namespace sdkBasicStorageRecipesWP8CS
{
    public static class FileListHelper
    {
        private static StringBuilder folderContents;

        private const string FOLDER_PREFIX = "\\";
        private const int PADDING_FACTOR = 3;
        private const char SPACE = ' ';

        // Begin recursive enumeration of files and folders.
        public static async Task<string> EnumerateFilesAndFolders(StorageFolder rootFolder)
        {
            // Initialize StringBuilder to contain output.
            folderContents = new StringBuilder();
            folderContents.AppendLine(FOLDER_PREFIX + rootFolder.Name);

            await ListFilesInFolder(rootFolder, 1);

            return folderContents.ToString();
        }

        // Continue recursive enumeration of files and folders.
        private static async Task ListFilesInFolder(StorageFolder folder, int indentationLevel)
        {
            string indentationPadding = String.Empty.PadRight(indentationLevel * PADDING_FACTOR, SPACE);

            // Get the subfolders in the current folder.
            // Increase the indentation level of the output.
            // For each subfolder, call this method again recursively.
            var foldersInFolder = await folder.GetFoldersAsync();
            int childIndentationLevel = indentationLevel + 1;
            foreach (StorageFolder currentChildFolder in foldersInFolder)
            {
                folderContents.AppendLine(indentationPadding + FOLDER_PREFIX + currentChildFolder.Name);
                await ListFilesInFolder(currentChildFolder, childIndentationLevel);
            }

            // Get the files in the current folder.
            var filesInFolder = await folder.GetFilesAsync();
            foreach (StorageFile currentFile in filesInFolder)
            {
                folderContents.AppendLine(indentationPadding + currentFile.Name);
            }
        }

        public static async Task Cleanup()
        {
            List<string> folders = (Application.Current as App).foldersToDelete;

            // Clean up folders created by this app.
            if (folders != null && folders.Count > 0)
            {
                foreach (string testFolderPath in folders)
                {
                    if (Directory.Exists(testFolderPath))
                    {
                        StorageFolder testFolder = await StorageFolder.GetFolderFromPathAsync(testFolderPath);
                        await testFolder.DeleteAsync();
                    }
                }
            }

            List<string> files = (Application.Current as App).filesToDelete;

            // Clean up files created by this app.
            if (files != null && files.Count > 0)
            {
                foreach (string testFilePath in (Application.Current as App).filesToDelete)
                {
                    if (File.Exists(testFilePath))
                    {
                        StorageFile testFile = await StorageFile.GetFileFromPathAsync(testFilePath);
                        await testFile.DeleteAsync();
                    }
                }
            }
        }

    }
}
