/****************************** Module Header ******************************\
* Module Name:    NavigationExtensions.cs
* Project:        CSWP8PassNonstringpara
* Copyright (c) Microsoft Corporation
*
* This sample will demo how to pass no-string data between two pages.
* This is custom extension for NavigationService.
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
using System.Windows.Navigation;

namespace CSWP8PassNonstringpara
{
    public static class NavigationExtensions
    {
        // Store parameters to be passed.
        private static object _navigationData = null;

        /// <summary>
        /// Set data.
        /// </summary>
        /// <param name="service">NavigationService</param>
        /// <param name="page">Target page.</param>
        /// <param name="data">Parameter data.</param>
        public static void Navigate(this NavigationService service, string page, object data)
        {
            _navigationData = data;
            try
            {
                service.Navigate(new Uri(page, UriKind.Relative));
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get data.
        /// </summary>
        /// <param name="service">NavigationService</param>
        /// <returns></returns>
        public static object GetLastNavigationData(this NavigationService service)
        {
            object data = _navigationData;
            // Reset
            _navigationData = null;
            return data;
        }
    }

}
