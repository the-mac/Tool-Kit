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

namespace CatalogServiceMock
{
    public class IAPResponseEntry
    {
        public Guid? ParentProductId { get; set; }
        public Guid? IapProductId { get; set; }

        public string IapId { get; set; }
        public string IapTitle { get; set; }
        public string IapDescription { get; set; }
        public string IapType { get; set; }
        public string IapDevData { get; set; }

        public Guid? ImageId { get; set; }

        public string[] IapTags { get; set; }

        public Guid? OfferId { get; set; }
        public Guid? IapSkuId { get; set; }
        public string Price { get; set; }
        public string DisplayPrice { get; set; }
        public string PriceCurrencyCode { get; set; }
    }
}
