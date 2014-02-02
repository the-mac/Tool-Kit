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

namespace ContosoHandlerApp
{
    class CustomUriMapper : UriMapperBase
    {
        private string tempUri;

        public override Uri MapUri(Uri uri)
        {
            // The original URI is encoded prior to URI association. Decode before using it.
            tempUri = System.Net.HttpUtility.UrlDecode(uri.ToString());

            // URI association launch for contoso handler page.
            if (tempUri.Contains("contoso:ShowHandlerPage?CategoryID="))
            {
                // Get the category ID (after "CategoryID=").
                int categoryIdIndex = tempUri.IndexOf("CategoryID=") + 11;
                string categoryId = tempUri.Substring(categoryIdIndex);

                // Original URI that triggered the URI association.
                int origUriIndex = tempUri.IndexOf("encodedLaunchUri=") + 17;
                string origUri = tempUri.Substring(origUriIndex);

                // Construct new URI
                string NewURI = String.Format("/ContosoHandlerPage.xaml?CategoryID={0}&OrigURI={1}",
                                            categoryId, origUri);

                // Map the show products request to ShowProducts.xaml
                return new Uri(NewURI, UriKind.Relative);
            }

            // Show other uses of the contoso scheme.
            if (tempUri.Contains("contoso:"))
            {
                // Get the category ID (after "CategoryID=").
                string categoryId = " (not available)";

                // Original URI that triggered the URI association.
                int origUriIndex = tempUri.IndexOf("encodedLaunchUri=") + 17;
                string origUri = tempUri.Substring(origUriIndex);

                // Construct new URI
                string NewURI = String.Format("/ContosoHandlerPage.xaml?CategoryID={0}&OrigURI={1}",
                                            categoryId, origUri);

                // Map the show products request to ShowProducts.xaml
                return new Uri(NewURI, UriKind.Relative);
            }


            // Otherwise perform normal launch.
            return uri;
        }
    }
}
