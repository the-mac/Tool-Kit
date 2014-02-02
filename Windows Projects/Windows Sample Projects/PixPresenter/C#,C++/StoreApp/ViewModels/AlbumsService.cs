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
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace PixPresenter.ViewModels
{
    public class AlbumsService : IAlbumsService
    {
        public async Task<List<PixPresenterPortableLib.AlbumViewModel>> GetAlbumsAsync()
        {
            var result = new List<AlbumViewModel>();

            AlbumViewModel avm = null;

            IReadOnlyList<StorageFolder> folders = await KnownFolders.PicturesLibrary.GetFoldersAsync();
            foreach (var folder in folders)
            {
                avm = await CreateAlbumViewModel(folder);
                if (avm != null)
                    result.Add(avm);
            }

            return result;
        }

        private async Task<AlbumViewModel> CreateAlbumViewModel(StorageFolder folder)
        {
            var files = await folder.GetFilesAsync();
            if (files.Count == 0)
                return null;

            byte[] thumb = null;

            var thumbnail = await folder.GetThumbnailAsync(Windows.Storage.FileProperties.ThumbnailMode.PicturesView);
            if (thumbnail != null)
            {
              var stream = thumbnail.AsStreamForRead();
              using (BinaryReader br = new BinaryReader(stream))
              {
                thumb = br.ReadBytes((int)stream.Length);
              }
            }

            return new AlbumViewModel(App.AlbumService, folder.DisplayName, folder.Path, thumb);

        }
    }
}
