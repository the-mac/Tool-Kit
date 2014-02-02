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
using System.Threading.Tasks;

namespace PixPresenterPortableLib
{
    /// <summary>
    /// Interface for platform-specific picture operations. 
    /// </summary>
    /// <remarks>This interface is implemented in both the Windows Phone and Windows Store apps (projects).</remarks>
    public interface IPictureService
    {
        // Retrieve the image as a bytearray. This is stored in the PictureViewModel object and cleared
        // whenever the object goes out of scope. 
        Task<byte[]> GetImageBytesAsync(string path);

        // Convert the iamge to grayscale.
        Task<byte[]> ConvertToGrayScale(byte[] imageBytes, int height, int width);
    }
}
