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
using System.Linq;
using System.Xml.Linq;

namespace CatalogServiceMock
{
    public class BatchGetIapDetailsResponse
    {
        public BatchGetIapDetailsResponse()
        {
            Entries = new List<IAPResponseEntry>();
        }

        public List<IAPResponseEntry> Entries { get; private set; }

        public XStreamingElement Generate()
        {
            IEnumerable<XStreamingElement> resourceElements = Entries.Select(CreateEntry);

            var response = new XStreamingElement(Constants.ZuneAppNamespace + "BatchGetIapDetailsResponse",
                                                 new XAttribute("xmlns", Constants.ZuneAppNamespace),
                                                 new XStreamingElement(Constants.ZuneAppNamespace + "Resources", resourceElements));

            return response;
        }

        public XStreamingElement CreateEntry(IAPResponseEntry entry)
        {
            XStreamingElement id = Constants.GetId(Constants.FormatUUID(entry.IapProductId));
            XStreamingElement title = Constants.GetTitle(entry.IapTitle);
            XStreamingElement entryContent = Constants.GetEntryContent(entry.IapDescription);

            return new XStreamingElement(Constants.ZuneAppNamespace + "Resource",
                                         new XStreamingElement(Constants.ZuneAppNamespace + "IapId", entry.IapId),
                                         new XStreamingElement(Constants.ZuneAppNamespace + "Result", "OK"),
                                         Constants.GetFeed(
                                             Constants.ZuneAppNamespace,
                                             Constants.GetSelfLink("/v8/catalog/iap?os=8.0"),
                                             Constants.GetUpdated(),
                                             title,
                                             id,
                                             entryContent,
                                             new XStreamingElement(Constants.ZuneAppNamespace + "iapId", entry.IapId),
                                             new XStreamingElement(Constants.ZuneAppNamespace + "iapParentProductId", Constants.FormatUUID(entry.ParentProductId)),
                                             new XStreamingElement(Constants.ZuneAppNamespace + "iapType", entry.IapType),
                                             new XStreamingElement(Constants.ZuneAppNamespace + "iapDevData", entry.IapDevData),
                                             new XStreamingElement(Constants.ZuneAppNamespace + "sortTitle", entry.IapTitle),
                                             new XStreamingElement(Constants.ZuneAppNamespace + "image", new XStreamingElement(Constants.ZuneAppNamespace + "id", Constants.FormatUUID(Guid.NewGuid()))),
                                             Constants.GetTagsElement(entry.IapTags),
                                             Constants.GetFakeOffers(),
                                             new XStreamingElement(Constants.ZuneAppNamespace + "taxString", "plus applicable 99% taxes")));
        }
    }
}
