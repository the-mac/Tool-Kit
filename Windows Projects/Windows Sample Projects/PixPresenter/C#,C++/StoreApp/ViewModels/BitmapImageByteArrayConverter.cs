/* 
    Copyright (c) 2012 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Store apps, visit http://code.msdn.microsoft.com/windowsapps.
  
*/
using System;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace PixPresenter
{
  public class BitmapImageByteArrayConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, string language)
    {
      return value == null ? null : GetBitmap((byte[])value);
    }

    private BitmapImage GetBitmap(byte[] bytes)
    {
      var bmp = new BitmapImage();

      using (var stream = new Windows.Storage.Streams.InMemoryRandomAccessStream())
      {
        using (var writer = new Windows.Storage.Streams.DataWriter(stream.GetOutputStreamAt(0)))
        {
          writer.WriteBytes(bytes);
          writer.StoreAsync().GetResults(); 
          bmp.SetSource(stream);
        }
      }
      return bmp;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
      throw new NotImplementedException();
    }
  }
}
