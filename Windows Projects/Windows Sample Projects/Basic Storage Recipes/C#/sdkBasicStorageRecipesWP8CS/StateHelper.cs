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
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Storage;

namespace sdkBasicStorageRecipesWP8CS
{
    public static class StateHelper
    {
        public static async Task SaveList(List<string> itemsToDelete, string filename)
        {
            if (itemsToDelete.Count > 0)
            {
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                StorageFile listFile = await localFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);

                using (Stream listStream = await listFile.OpenStreamForWriteAsync())
                {
                    XmlSerializer listWriter = new XmlSerializer(typeof(List<string>));
                    listWriter.Serialize(listStream, itemsToDelete);
                }
            }
        }

        public static async Task<List<string>> LoadList(string filename)
        {
            List<string> itemsToDelete = null;

            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            string listFilePath = Path.Combine(localFolder.Path, filename);

            if (File.Exists(listFilePath))
            {
                StorageFile listFile = await localFolder.GetFileAsync(filename);

                using (Stream listStream = await listFile.OpenStreamForReadAsync())
                {
                    XmlSerializer listReader = new XmlSerializer(typeof(List<string>));
                    itemsToDelete = listReader.Deserialize(listStream) as List<string>;
                }

                await listFile.DeleteAsync();
            }

            return itemsToDelete;
        }
    }
}
