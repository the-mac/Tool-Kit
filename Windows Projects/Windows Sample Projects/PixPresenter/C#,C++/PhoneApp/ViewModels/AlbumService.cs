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
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace PixPresenter.ViewModels
{
    /// <summary>
    /// Class to implement the platform specific abstraction, IAlbumService
    /// </summary>
    public class AlbumService : IAlbumService
    {
        /// <summary>
        /// Asynchronously returns a list of all pictures in the specified album
        /// </summary>
        /// <param name="albumName">The name of the album</param>
        /// <param name="path">The path of the album</param>
        /// <returns>A list of all pictures in the specified album</returns>
        /// <remarks>The path parameter is not actually used in this Windows Phone implementation
        /// since the MediaLibrary API does not support picture or album retrieval with a path. Instead, we must walk
        /// the library looking for the album with the given albumName and then grab all the pictures in that album.</remarks>
        public Task<List<PictureViewModel>> GetPicturesAsync(string albumName, string path)
        {
            return Task.Run(delegate
            {
              List<PictureViewModel> result = new List<PictureViewModel>();
              MediaLibrary mediaLib = new MediaLibrary();

              // Find the album
              foreach (var album in mediaLib.RootPictureAlbum.Albums)
              {
                // Because the photo library API on Windows Phone doesn't expose the concept of "path", 
                // we iterate and find the album based on the name.
                if (album.Name == albumName)
                {
                  foreach (var pic in album.Pictures)
                  {
                    PictureViewModel pvm = new PictureViewModel(App.PictureService, albumName, pic.Name,String.Format("{0}|{1}",albumName, pic.Name),pic.Width, pic.Height);
                    result.Add(pvm);
                  }
                  Debug.WriteLine("{0} pictures in {1}", result.Count(), albumName);
                  break;
                }
              }
              return result;
            });
        }
    }
}
