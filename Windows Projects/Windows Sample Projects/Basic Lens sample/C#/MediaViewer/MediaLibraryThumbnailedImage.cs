/* 
    Copyright (c) 2012 - 2013 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://code.msdn.microsoft.com/wpapps
  
*/
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Media.PhoneExtensions;
using System;
using System.IO;

namespace Microsoft.Phone.Controls
{
    public class MediaLibraryThumbnailedImage : IThumbnailedImage
    {
        /// <summary>
        /// The Picture object that this instance represents.
        /// </summary>
        public Picture Picture { get; private set; }

        public MediaLibraryThumbnailedImage(Picture picture)
        {
            this.Picture = picture;
        }

        /// <summary>
        /// Returns a Stream representing the thumbnail image.
        /// </summary>
        /// <returns>Stream of the thumbnail image.</returns>
        public Stream GetThumbnailImage()
        {
            return this.Picture.GetPreviewImage();
        }

        /// <summary>
        /// Returns a Stream representing the full resolution image.
        /// </summary>
        /// <returns>Stream of the full resolution image.</returns>
        public Stream GetImage()
        {
            return this.Picture.GetImage();
        }

        /// <summary>
        /// Represents the time the photo was taken, useful for sorting photos.
        /// </summary>
        public DateTime DateTaken
        {
            get
            {
                return this.Picture.Date;
            }
        }
    }
}
