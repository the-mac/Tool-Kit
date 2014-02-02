/* 
    Copyright (c) 2012 - 2013 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://code.msdn.microsoft.com/wpapps
  
*/
using System.Collections.Generic;

namespace Microsoft.Phone.Controls
{
    /// <summary>
    /// Compares two IThumbnailedImage instances by DateTaken
    /// </summary>
    public class ThumbnailedImageDateComparer : IComparer<IThumbnailedImage>
    {
        public ThumbnailedImageDateComparer()
        {
        }

        /// <summary>
        /// Compare two IThumbnailedImage instances by DateTaken
        /// </summary>
        /// <param name="x">First IThumbnailedImage to examine</param>
        /// <param name="y">IThumbnailedImage to compare to the first IThumbnailedImage</param>
        /// <returns></returns>
        public int Compare(IThumbnailedImage x, IThumbnailedImage y)
        {
            return x.DateTaken.CompareTo(y.DateTaken);
        }
    }
}
