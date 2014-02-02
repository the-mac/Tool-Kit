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
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace CatalogServiceMock
{
    public class Constants
    {
        public static readonly XNamespace ZuneAppNamespace = "http://schemas.zune.net/catalog/apps/2008/02";

        public static readonly XNamespace AtomNamespace = "http://www.w3.org/2005/Atom";

        public static XStreamingElement GetId(string idValue)
        {
            return new XStreamingElement(AtomNamespace + "id", idValue);
        }

        public static XStreamingElement GetUpdated()
        {
            return new XStreamingElement(AtomNamespace + "updated", DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss.ffffffZ", CultureInfo.InvariantCulture));
        }

        public static XStreamingElement GetEntryContent(string content)
        {
            return new XStreamingElement(AtomNamespace + "content", new XAttribute("type", "html"), content);
        }

        public static XStreamingElement GetFeed(
            XNamespace defaultNamespace,
            params object[] elementContents)
        {
            return new XStreamingElement(
                AtomNamespace + "feed",
                new XAttribute(XNamespace.Xmlns + "a", AtomNamespace),
                new XAttribute("xmlns", defaultNamespace),
                elementContents);
        }

        public static string FormatUUID(Guid? uuid)
        {
            return string.Format(CultureInfo.InvariantCulture, "urn:uuid:{0}", uuid);
        }

        public static XStreamingElement GetTitle(string title)
        {
            return new XStreamingElement(AtomNamespace + "title", new XAttribute("type", "text"), title);
        }

        public static XStreamingElement GetSelfLink(string hrefValue)
        {
            return GetEntryLink("self", hrefValue);
        }

        public static XStreamingElement GetEntryLink(string linkType, string hrefValue)
        {
            return new XStreamingElement(
                AtomNamespace + "link",
                new XAttribute("rel", linkType),
                new XAttribute("type", @"application/atom+xml"),
                new XAttribute("href", hrefValue));
        }

        public static XStreamingElement GetTagsElement(string[] tags)
        {
            XStreamingElement element = null;
            if (tags != null)
            {
                element = new XStreamingElement(ZuneAppNamespace + "tags",
                                                tags.Select(tag => new XStreamingElement(ZuneAppNamespace + "tag", tag)));
            }

            return element;
        }

        public static XStreamingElement GetFakeOffers()
        {
            var fakeOffer = new XStreamingElement(ZuneAppNamespace + "offer",
                                                  new XStreamingElement(ZuneAppNamespace + "offerId", FormatUUID(Guid.NewGuid())),
                                                  new XStreamingElement(ZuneAppNamespace + "mediaInstanceId", FormatUUID(Guid.NewGuid())),
                                                  new XStreamingElement(ZuneAppNamespace + "clientTypes", new XStreamingElement(ZuneAppNamespace + "clientType", "WindowsPhone80")),
                                                  new XStreamingElement(ZuneAppNamespace + "paymentTypes", new XStreamingElement(ZuneAppNamespace + "paymentType", "Credit Card")),
                                                  new XStreamingElement(ZuneAppNamespace + "store", "Zest"),
                                                  new XStreamingElement(ZuneAppNamespace + "price", "1.99"),
                                                  new XStreamingElement(ZuneAppNamespace + "displayPrice", "$1.99"),
                                                  new XStreamingElement(ZuneAppNamespace + "priceCurrencyCode", "USD"),
                                                  new XStreamingElement(ZuneAppNamespace + "licenseRight", "Purchase"),
                                                  new XStreamingElement(ZuneAppNamespace + "expiration", "2100-01-01T00:00:00Z"));

            return new XStreamingElement(ZuneAppNamespace + "offers", fakeOffer);
        }
    }
}
