/****************************** Module Header ******************************\
* Module Name:    ImageFromRssText.cs
* Project:        CSWP8RSSImage
* Copyright (c) Microsoft Corporation
*
* This demo shows how to display images from an RSS feed.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\*****************************************************************************/

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace CSWP8RSSImage
{
    public class ImageFromRssText : IValueConverter
    {
        //  Get images from each SyndicationItem. 
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return null;

            List<ImageItem> listUri = GetHtmlImageUrlList(value.ToString());
            return listUri;
        }

        /// <summary> 
        /// Get the URL of the all pictures from the HTML. 
        /// </summary> 
        /// <param name="sHtmlText">HTML code</param> 
        /// <returns>URL list of the all pictures</returns> 
        public static List<ImageItem> GetHtmlImageUrlList(string sHtmlText)
        {
            // The definition of a regular expression to match img tag. 
            Regex regImg = new Regex(@"<img\b[^<>]*?\bsrc[\s\t\r\n]*=[\s\t\r\n]*[""']?[\s\t\r\n]*(?<imgUrl>[^\s\t\r\n""'<>]*)[^<>]*?/?[\s\t\r\n]*>", RegexOptions.IgnoreCase);

            // The search for a matching string.
            MatchCollection matches = regImg.Matches(sHtmlText);
            int i = 0;
            List<ImageItem> imgUrlList = new List<ImageItem>();

            // Get a list of matches
            foreach (Match match in matches)
            {
                imgUrlList.Add(new ImageItem("img" + i, match.Groups["imgUrl"].Value));
                i++;
            }
            return imgUrlList;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Image entity
    /// </summary>
    public class ImageItem
    {
        public ImageItem(string title, string url)
        {
            this.Title = title;
            this.URL = url;
        }

        public string Title { get; set; }
        public string URL { get; set; }
    }
}
