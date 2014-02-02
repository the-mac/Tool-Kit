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
    /// Represents a picture album on the device
    /// </summary>
    public class AlbumViewModel : ViewModelBase
    {
        public ObservableCollection<PictureViewModel> Pictures { get; private set; }

        public string Name { get; set; }    // The name of the album
        public string Path { get; set; }    // The path of the album in the photo library. 

        private int _currentIndex = 0;
        private IAlbumService _albumService;

        public AlbumViewModel(IAlbumService albumService, string albumName, string path, byte[] thumb)
        {
            _albumService = albumService;
            this.Name = albumName;
            this.Path = path;
            this.Thumb = thumb;
            Pictures = new ObservableCollection<PictureViewModel>();
        }

        /// <summary>
        /// Loads all pictures into the pictures collection
        /// </summary>
        public Task LoadPicturesAsync()
        {
          return Task.Run(delegate
          {
            InitializeOnceOrWait(delegate
            {
              var pictureList = _albumService.GetPicturesAsync(this.Name,this.Path).Result;
              Pictures = new ObservableCollection<PictureViewModel>(pictureList);
            });
          });
        }

        /// <summary>
        /// Retrieve next picture in the forward direction.
        /// </summary>
        /// <remarks>This is circular. When the end of the list is reached, return the first item and start again.</remarks>
        public void GetNextPicture()
        {
          GetNextPictureAtOffset(1);
        }

        /// <summary>
        /// Retrieve the previous picture in the backwards direction.
        /// </summary>
        /// <remarks>This is circular. When the start of the list is reached, return the last item and start again.</remarks>
        public void GetPreviousPicture()
        {
          GetNextPictureAtOffset(-1);
        }

        void GetNextPictureAtOffset(int offset)
        {
          if (this.Pictures.Count == 0)
            return;

          int total = Pictures.Count;
          this.CurrentPictureIndex = (this.CurrentPictureIndex + offset + total) % total;

        }

      // Restore current index after deactivation / resume
        public void RestoreCurrentPictureToken(object token)
        {
          int index;
          _currentIndex = 0;

          if (token == null)
            return;

          if (token is int)
            index = (int)token;
          else
            int.TryParse(token.ToString(), out index);
          
          GetNextPictureAtOffset(index);
        }

        private PictureViewModel _currentPicture;

        /// <summary>
        /// The current picture in the navigation list.
        /// </summary>
        /// <remarks></remarks>
        public PictureViewModel CurrentPicture
        {
            get
            {
                if (_currentPicture == null && Pictures.Count > 0)
                {
                    _currentPicture = Pictures[0];
                }
                return _currentPicture;
            }
            set
            {
                SetProperty(ref _currentPicture, value);
            }
        }

        public int CurrentPictureIndex
        {
            get
            {
                return _currentIndex;
            }
            set
            {
                _currentIndex = value;
                
                this.CurrentPicture = this.Pictures[_currentIndex];
                ClearImages();
            }
        }

        // There is no need to keep each image in memory. The image
        // is stored in the PictureViewModel as a byte array. When a picture is 
        // not in scope, we clear this byte array. This method is called everytime 
        // we change the current picture.
        private void ClearImages()
        {
            for (int i = 0; i < this.Pictures.Count; i++)
            {
                if (i <= _currentIndex - 2 || i >= _currentIndex + 2)
                {
                    this.Pictures[i].ClearImage();
                }
            }
        }

        private byte[] _thumb;
        public byte[] Thumb
        {
            get
            {
                return _thumb;
            }
            set
            {
              SetProperty(ref _thumb, value);
            }
        }
    }
}
