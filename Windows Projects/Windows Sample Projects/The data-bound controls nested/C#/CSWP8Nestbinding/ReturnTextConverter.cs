/****************************** Module Header ******************************\
* Module Name:    ReturnTextConverter.cs
* Project:        CSWP8Nestbinding
* Copyright (c) Microsoft Corporation
*
* This sample will demo how to nested data-bound controls in WP.
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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CSWP8Nestbinding
{
    public class ReturnTextConverter : IValueConverter
    {
        //  Get link from each item. 
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return null;

            List<URLItem> listUri = GetList(value.ToString());
            return listUri;
        }


        /// <summary> 
        /// Get the URL and InnerHtml of the all a tag from the HTML. 
        /// </summary> 
        /// <param name="strInText">HTML code</param> 
        /// <returns>URL list of the all pictures</returns> 
        public static List<URLItem> GetList(string strInText)
        {
            //// The definition of a regular expression to match condition. 
             Regex regStr = new Regex(@"(?is)<a[^>]*?href=(['""]?)(?<url>[^'""\s>]+)\1[^>]*>(?<text>(?:(?!</?a\b).)*)</a>");

            // The search for a matching string.
            MatchCollection matches = regStr.Matches(strInText);
            List<URLItem> listUri = new List<URLItem>();

            // Get a list of matches
            foreach (Match match in matches)
            {
                if (match.Success)
                {
                   listUri.Add(new URLItem(match.Groups["text"].Value, match.Groups["url"].Value));   
                }
            }

            return listUri;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// URL entity
    /// </summary>
    public class URLItem
    {
        public URLItem(string stringInnerHtml, string striUrl)
        {
            this.StrInnerHtml = stringInnerHtml;
            this.URL = striUrl;
        }

        public string StrInnerHtml { get; set; }
        public string URL { get; set; }
    }

}
