/* 
    Copyright (c) 2012 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
    To see all Code Samples for Windows Store apps, visit http://code.msdn.microsoft.com/windowsapps
  
*/
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace PixPresenterPortableLib
{
    /// <summary>
    /// Represents the collection of picture albums on the device
    /// </summary>
    public class AlbumsViewModel : ViewModelBase
    {
        public ObservableCollection<AlbumViewModel> Albums { get; private set; }
        IAlbumsService _albumsService;

        /// <summary>
        /// Initialize the ViewModel. Platform specific dependencies are injected into the constructor using
        /// an interface. 
        /// </summary>
        /// <param name="albumsService">The implementation of the platform specific abstraction</param>
        public AlbumsViewModel(IAlbumsService albumsService)
        {
            _albumsService = albumsService;
        }

        /// <summary>
        /// Loads all albums into the album collection.
        /// </summary>
        public Task LoadAlbumsAsync()
        {
          return Task.Run(delegate
          {
            InitializeOnceOrWait(delegate
            {
                var albumsList = _albumsService.GetAlbumsAsync().Result;
                Albums = new ObservableCollection<AlbumViewModel>(albumsList);
            });
          });
        }
    }
}
