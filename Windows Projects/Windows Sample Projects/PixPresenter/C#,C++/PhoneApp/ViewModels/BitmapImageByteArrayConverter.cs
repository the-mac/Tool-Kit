/* 
    Copyright (c) 2012 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Media.Imaging;
using System.Windows.Data;

namespace PixPresenter
{
    /// <summary>
    /// Each picture is stored as a byte array in the PictureViewModel object.
    /// When we bind to that property we must convert to an image source that can be used by the Image control.
    /// </summary>
      public class BitmapImageByteArrayConverter : IValueConverter
      {

        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
          return this.Convert(value, targetType, parameter, culture.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }


        private object Convert(object value, Type targetType, object parameter, string language)
        {
            return value == null ? null : GetBitmap((byte[])value);
        }

        private BitmapImage GetBitmap(byte[] bytes)
        {
            var bmp = new BitmapImage();

            using (MemoryStream ms = new MemoryStream(bytes))
            {
                bmp.CreateOptions = BitmapCreateOptions.BackgroundCreation;
                ms.Seek(0, SeekOrigin.Begin);
                bmp.SetSource(ms);
            };

            return bmp;
        }
    }
}
