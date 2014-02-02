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

namespace PixPresenterPortableLib
{
    /// <summary>
    /// Represents a picture on the device
    /// </summary>
    public class PictureViewModel : ViewModelBase
    {
        private IPictureService _pictureService;

        /// <summary>
        /// PictureViewModel constructor
        /// </summary>
        /// <param name="pictureService">Injection of platform-specific operations, using an interface</param>
        /// <param name="albumName">The name of the album</param>
        /// <param name="pictureName">The name of the picture</param>
        /// <param name="picturepath">The path to the picture</param>
        /// <param name="height">The height of the picture</param>
        /// <param name="width">The width of the picture</param>
        /// <remarks>The picturePath property represents an actual path to the image file in Windows 8. In Windows Phone
        /// this represents a combination of album name and picture name.</remarks>
        public PictureViewModel(IPictureService pictureService, string albumName, string pictureName, string picturepath, int height, int width)
        {
            this.AlbumName = albumName;
            this.Name = pictureName;
            this.Path = picturepath;
            this.Height = height;
            this.Width = width;
            this._pictureService = pictureService;
        }

        public string AlbumName { get; private set; }
        public string Name { get; private set; }
        public string Path { get; private set; }
        public int Height { get; private set; }
        public int Width { get; private set; }

        byte[] _imageBytes = null;
        public byte[] ImageBytes
        {
          get
          {
            // Delay-load the image. A smarter implementation might also load
            // the next and previous images...
              if (_imageBytes == null)
              {
                  SetImageBytes();
              }

            return _imageBytes; 
          }
          set
          {
            SetProperty(ref _imageBytes, value);
          }
        }

        public void ClearImage()
        {
            ImageBytes = null;
        }

        public async void ToGrayScale()
        {
            var grayScaleBytes = await _pictureService.ConvertToGrayScale(_imageBytes, this.Height, this.Width);
            this.ImageBytes = grayScaleBytes;
        }

        private async void SetImageBytes()
        {
            var bytes = await _pictureService.GetImageBytesAsync(this.Path);
            this.ImageBytes = bytes;
        }
    }
}
