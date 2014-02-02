/* 
    Copyright (c) 2012 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
using Microsoft.Phone;
using Microsoft.Xna.Framework.Media;
using PixPresenterPortableLib;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using FastFilters;

namespace PixPresenter.ViewModels
{
    /// <summary>
    /// Asynchronously get a picture using the specified path
    /// </summary>
    /// <param name="path">The path to the picture</param>
    /// <returns>The picture as a byte array on success; Otherwise, null</returns>
    /// <remarks>The path parameter in the Windows Phone implementation is not a file path. Instead, it is 
    /// a combination of the album name and the picture name.</remarks>
    public class PictureService : IPictureService
    {
        public Task<byte[]> GetImageBytesAsync(string path)
        {
            return Task.Run(delegate
            {
                Debug.WriteLine("PicturePath '{0}'",path);

                // Extract the album name and picture name from the path
                var pathParts = path.Split("|".ToCharArray());
                string albumName = pathParts[0];
                string pictureName = pathParts[1];

                byte[] result = null;

                MediaLibrary mediaLib = new MediaLibrary();
                foreach (var album in mediaLib.RootPictureAlbum.Albums)
                {
                    // Find the album
                    if (album.Name == albumName)
                    {
                        foreach (var pic in album.Pictures)
                        {
                            if (pic.Name == pictureName)
                            {
                                using (var picStream = pic.GetImage())
                                {
                                    using (BinaryReader br = new BinaryReader(picStream))
                                    {
                                        Debug.WriteLine("picStream.Length {0}", picStream.Length);
                                        result = br.ReadBytes((int)picStream.Length);
                                    }
                                }
                            }
                        }
                    }
                }
                Debug.WriteLine(result.Length);

                return result;
            });
        }

        ImageFilter _filter = null;

        /// <summary>
        /// Convert an image to grayscale
        /// </summary>
        /// <param name="imageBytes">The image to convert</param>
        /// <param name="height">The height of the image</param>
        /// <param name="width">The width of the image</param>
        /// <returns>The image, converted to grayscale</returns>
        public async Task<byte[]> ConvertToGrayScale(byte[] imageBytes, int height, int width)
        {
            using (MemoryStream stream = new MemoryStream(imageBytes))
            {
                WriteableBitmap bmp = PictureDecoder.DecodeJpeg(stream, height, width);
                var pixels = bmp.Pixels;
                var bytes = new byte[pixels.Length * 4];
                Buffer.BlockCopy(pixels, 0, bytes, 0, bytes.Length);

                if (_filter == null)
                    _filter = new ImageFilter();
                await _filter.ToGrayScale(bytes.AsBuffer());

                Buffer.BlockCopy(bytes, 0, pixels, 0, bytes.Length);
                MemoryStream outStream = new MemoryStream();
                bmp.SaveJpeg(outStream, height, width, 0, 100);
                return outStream.ToArray();
            }
        }
    }
}
