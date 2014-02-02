/****************************** Module Header ******************************\
* Module Name:    StateUtilities.cs
* Project:        CSWP8CollectionViewSource
* Copyright (c) Microsoft Corporation
*
* This is a utility class.
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

namespace CSWP8CollectionViewSource
{
    public static class StateUtilities
    {
        // This property is used to track if the user has started a new instance of the app.
        private static Boolean isLaunching;

        public static Boolean IsLaunching
        {
            get { return isLaunching; }
            set { isLaunching = value; }
        }
    }

}
