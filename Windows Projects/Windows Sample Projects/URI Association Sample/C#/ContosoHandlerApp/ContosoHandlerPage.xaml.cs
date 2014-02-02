/* 
    Copyright (c) 2012 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
using Microsoft.Phone.Controls;
using System.Collections.Generic;
using System.Windows.Navigation;

namespace ContosoHandlerApp
{
    public partial class ContosoHandlerPage : PhoneApplicationPage
    {
        public ContosoHandlerPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Get a dictionary of query string keys and values.
            IDictionary<string, string> queryStrings = this.NavigationContext.QueryString;

            // Ensure that the "CategoryID" key is present.
            if (queryStrings.ContainsKey("CategoryID"))
            {
                //Display Category ID
                textCategoryId.Text = queryStrings["CategoryID"].ToString();

                //Display Deep Link URI
                textDeepLinkUri.Text = queryStrings["OrigURI"].ToString();
            }


        }

    }
}
