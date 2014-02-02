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
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using FastFiltersWin8;

namespace PixPresenter.ViewModels
{
    public class PictureService : IPictureService
    {
        public async Task<byte[]> GetImageBytesAsync(string path)
        {
            byte[] result = null;

            var file = await StorageFile.GetFileFromPathAsync(path);

            using (var picStream = await file.OpenStreamForReadAsync())
            {
                using (BinaryReader br = new BinaryReader(picStream))
                {
                    result = br.ReadBytes((int)picStream.Length);
                }
            }

            return result;
        }

        ImageFilter _filter = null;
        public async Task<byte[]> ConvertToGrayScale(byte[] imageBytes, int height, int width)
        {
            using (InMemoryRandomAccessStream rasStream = new InMemoryRandomAccessStream())
            {
                await rasStream.WriteAsync(imageBytes.AsBuffer());
                var decoder = await BitmapDecoder.CreateAsync(rasStream);
                var pixelData = await decoder.GetPixelDataAsync();
                var pixels = pixelData.DetachPixelData();

                if (_filter == null)
                    _filter = new ImageFilter();

                await _filter.ToGrayScale(pixels.AsBuffer());

                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, rasStream);
                encoder.SetPixelData(decoder.BitmapPixelFormat, BitmapAlphaMode.Ignore, (uint)width, (uint)height, decoder.DpiX, decoder.DpiY, pixels);
                await encoder.FlushAsync();

                using (BinaryReader br = new BinaryReader(rasStream.AsStreamForRead()))
                {
                    rasStream.Seek(0);
                    return br.ReadBytes((int)rasStream.AsStreamForRead().Length);
                }
            }
        }
    }
}
