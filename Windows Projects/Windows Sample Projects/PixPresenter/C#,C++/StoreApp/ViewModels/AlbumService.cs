/* 
    Copyright (c) 2012 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Store apps, visit http://code.msdn.microsoft.com/windowsapps.
  
*/
using PixPresenterPortableLib;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;

namespace PixPresenter.ViewModels
{
    public class AlbumService : IAlbumService
    {
        public async Task<List<PictureViewModel>> GetPicturesAsync(string albumName, string path)
        {
            List<PictureViewModel> result = new List<PictureViewModel>();
            StorageFolder folder;
            if (String.IsNullOrEmpty(path))
                folder = KnownFolders.PicturesLibrary;
            else
                folder = await StorageFolder.GetFolderFromPathAsync(path);
            var files = await folder.GetFilesAsync();

            foreach (var file in files)
            {
                var properties = await file.Properties.GetImagePropertiesAsync();

                PictureViewModel pvm = new PictureViewModel(App.PictureService,albumName, file.DisplayName, file.Path, (int)properties.Height, (int)properties.Width);
                result.Add(pvm);
            }

            return result;
        }
    }
}
