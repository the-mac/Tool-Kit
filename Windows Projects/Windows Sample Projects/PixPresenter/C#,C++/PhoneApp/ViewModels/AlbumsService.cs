/* 
    Copyright (c) 2012 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
using Microsoft.Xna.Framework.Media;
using PixPresenterPortableLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixPresenter.ViewModels
{
    /// <summary>
    /// Class to implement the platform specific abstraction, IAlbumsService
    /// </summary>
    public class AlbumsService : IAlbumsService
    {
        /// <summary>
        /// Asynchronously returns a list of all albums in the picture library
        /// </summary>
        /// <returns>A list of all albums in the picture library</returns>
        public Task<List<PixPresenterPortableLib.AlbumViewModel>> GetAlbumsAsync()
        {
            return Task.Run(delegate
            {
                List<AlbumViewModel> result = new List<AlbumViewModel>();

                MediaLibrary mediaLib = new MediaLibrary();
                foreach (var album in mediaLib.RootPictureAlbum.Albums)
                {
                    byte[] thumb = null;

                    // Exclude empty picture albums
                    if (album.Pictures.Count > 0)
                    {
                        Stream stream = album.Pictures[0].GetThumbnail();
                        using (BinaryReader br = new BinaryReader(stream))
                        {
                            thumb =  br.ReadBytes((int)stream.Length);
                        }

                        AlbumViewModel avm = new AlbumViewModel(App.AlbumService, album.Name, album.Name, thumb);
                        avm.Name = album.Name;
                        result.Add(avm);
                    }
                }
                return result;
            });
        }

    }
}
