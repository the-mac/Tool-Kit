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
using System.Windows.Navigation;
using Windows.Phone.Storage.SharedAccess;

namespace sdkRouteMapperWP8CS
{
    class CustomURIMapper : UriMapperBase
    {
        private string tempUri;

        public override Uri MapUri(Uri uri)
        {
            tempUri = uri.ToString();

            // File association launch
            // Example launch URI: /FileTypeAssociation?fileToken=89819279-4fe0-4531-9f57-d633f0949a19
            if (tempUri.Contains("/FileTypeAssociation"))
            {
                // Get the file ID (after "fileToken=").
                int fileIDIndex = tempUri.IndexOf("fileToken=") + 10;
                string fileID = tempUri.Substring(fileIDIndex);

                // Map the file association launch to route page.
                return new Uri("/RoutePage.xaml?fileToken=" + fileID, UriKind.Relative);
            }

            // Otherwise perform normal launch.
            return uri;
        }
    }
}
