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

namespace CustomMapping
{
    class CustomUriMapper : UriMapperBase
    {

        public override Uri MapUri(Uri uri)
        {
            string tempUri = uri.ToString();
            string mappedUri;


            // Launch from the photo apps picker.
            // This is for only Windows Phone OS 7.1 apps.
            // Incoming URI example: /MainPage.xaml?token=%7B273fea8d-134c-4764-870d-42224d13eb1a%7D
            if ((tempUri.Contains("token")) && !(tempUri.Contains("RichMediaEdit")))
            {
                // Redirect to PhotoPage.xaml.
                mappedUri = tempUri.Replace("MainPage", "PhotoPage");
                return new Uri(mappedUri, UriKind.Relative);
            }


            // Launch from the photo share picker.
            // Incoming URI example: /MainPage.xaml?Action=ShareContent&FileId=%7BA3D54E2D-7977-4E2B-B92D-3EB126E5D168%7D
            if ((tempUri.Contains("ShareContent")) && (tempUri.Contains("FileId")))
            {
                // Redirect to PhotoShare.xaml.
                mappedUri = tempUri.Replace("MainPage", "PhotoShare");
                return new Uri(mappedUri, UriKind.Relative);
            }



            // Launch from the photo edit picker.
            // This is only for Windows Phone 8 apps.
            // Incoming URI example: /MainPage.xaml?Action=EditPhotoContent&FileId=%7Bea74a960-3829-4007-8859-cd065654fbbc%7D
            if ((tempUri.Contains("EditPhotoContent")) && (tempUri.Contains("FileId")))
            {
                // Redirect to PhotoEdit.xaml.
                mappedUri = tempUri.Replace("MainPage", "PhotoEdit");
                return new Uri(mappedUri, UriKind.Relative);
            }


            // Launch from the rich media "Open in" link.
            // This is only for Windows Phone 8 apps.
            // Incoming URI example: /MainPage.xaml?Action=RichMediaEdit&token=%7Bed8b7de8-6cf9-454e-afe4-abb60ef75160%7D
            if ((tempUri.Contains("RichMediaEdit")) && (tempUri.Contains("token")))
            {
                // Redirect to RichMediaPage.xaml.
                mappedUri = tempUri.Replace("MainPage", "RichMediaPage");
                return new Uri(mappedUri, UriKind.Relative);
            }


            // Otherwise perform normal launch.
            return uri;
        }
    }
}
