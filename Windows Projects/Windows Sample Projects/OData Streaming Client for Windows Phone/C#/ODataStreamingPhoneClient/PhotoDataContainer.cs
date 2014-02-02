// ---------------------------------------------------------- 
// Copyright (c) Microsoft Corporation.  All rights reserved. 
// ---------------------------------------------------------- 
using System;
using System.Linq;
using System.Xml.Linq;
using System.Data.Services.Client;

namespace ODataStreamingPhoneClient.Model
{
    // We need this to remove the StreamUri extension property.
    public partial class PhotoDataContainer
    {
        // This method is called when the context is created.
        partial void OnContextCreated()
        {
            // Register a handle for the writing entity event.
            this.WritingEntity +=
                new EventHandler<ReadingWritingEntityEventArgs>(
                    PhotoDataContainer_WritingEntity);
        }

        // Method that handles the WritingEntity event.
        void PhotoDataContainer_WritingEntity(object sender,
            ReadingWritingEntityEventArgs e)
        {
            // Define an XName for the properties element in the response payload.
            XName properties = XName.Get("properties",
                e.Data.GetNamespaceOfPrefix("m").NamespaceName);

            // Get the payload element that contains the properties of the entity to be sent.
            XElement payload = e.Data.DescendantsAndSelf()
                .Where<XElement>(xe => xe.Name == properties).First<XElement>();

            // Define an XName for the property to remove from the payload.
            XName propertyName = XName.Get("StreamUri",
                e.Data.GetNamespaceOfPrefix("d").NamespaceName);

            //Get the element of the property to remove.
            XElement removeProperty = payload.Descendants()
                .Where<XElement>(xe => xe.Name == propertyName).First<XElement>();

            // Remove the property from the payload.
            removeProperty.Remove();
        }
    }
}
