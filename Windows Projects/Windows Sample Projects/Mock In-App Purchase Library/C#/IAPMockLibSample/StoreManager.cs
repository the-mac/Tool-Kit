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
using System.Collections.Generic;
using System.Threading.Tasks;

#if DEBUG
using MockIAPLib;
using Store = MockIAPLib;
#else
using Windows.ApplicationModel.Store;
#endif

namespace IAPMockLibSample
{
    public class StoreManager
    {
        public Dictionary<string, string> StoreItems = new Dictionary<string, string>();
//        private const string FreeImage = "/Res/Image/1.png";

        public StoreManager()
        {
            // Populate the store
            StoreItems.Add("img.1", "/Res/Image/1.png");
            StoreItems.Add("img.2", "/Res/Image/2.png");
            StoreItems.Add("img.3", "/Res/Image/3.png");
            StoreItems.Add("img.4", "/Res/Image/4.png");
            StoreItems.Add("img.5", "/Res/Image/5.png");
            StoreItems.Add("img.6", "/Res/Image/6.png");
        }

        public async Task<List<string>> GetOwnedItems()
        {
            List<string> items = new List<string>();

            ListingInformation li = await CurrentApp.LoadListingInformationAsync();
            
            foreach (string key in li.ProductListings.Keys)
            {
                if (CurrentApp.LicenseInformation.ProductLicenses[key].IsActive && StoreItems.ContainsKey(key))
                    items.Add(StoreItems[key]);
            }
            
            return items;
        }        
    }
}
