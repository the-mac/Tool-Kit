/* 
    Copyright (c) 2012 - 2013 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://code.msdn.microsoft.com/wpapps
  
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Phone.PersonalInformation;

namespace sdkStoredContactsWP8CS
{
    /// <summary>
    /// When using the StoredContact APIs, you must ensure that the Remote IDs for your contacts are
    /// unique across all applications. If you can't guarantee that the IDs used by your remote contact
    /// store are unique across all application, you shoud use this RemoteIdHelper class to make sure that
    /// your remote IDs are unique. SetRemoteIdGuid creates a new GUID for the app the first
    /// time it is called. GetTaggedRemoteId preprends the GUID for the app to the supplied RemoteID value.
    /// GetUntaggedRemoteId gives you the original RemoteID back, without the prepended GUID.
    /// </summary>
    class RemoteIdHelper 
    {
        // Key used to store the app's GUID
        private const string ContactStoreLocalInstanceIdKey = "LocalInstanceId";

        // Create a GUID and store it in the ContactStore's extended properties collection. If the GUID
        // has already been set, it is not overwritten, so there is no danger in calling this method 
        // multiple times.
        public async Task SetRemoteIdGuid(ContactStore store)
        {
            IDictionary<string, object> properties;
            properties = await store.LoadExtendedPropertiesAsync().AsTask<IDictionary<string, object>>();
            if (!properties.ContainsKey(ContactStoreLocalInstanceIdKey))
            {
                // The given store does not have a local instance id so set one against store extended properties
                Guid guid = Guid.NewGuid();
                properties.Add(ContactStoreLocalInstanceIdKey, guid.ToString());
                System.Collections.ObjectModel.ReadOnlyDictionary<string, object> readonlyProperties = new System.Collections.ObjectModel.ReadOnlyDictionary<string, object>(properties);
                await store.SaveExtendedPropertiesAsync(readonlyProperties).AsTask();
            } 
        }

        /// <summary>
        /// Prepends the supplied remote ID with the GUID for the application in order to make sure
        /// that the remote ID is unique across all applications.
        /// </summary>
        /// <param name="store">The app's contact store. The remote ID Guid is stored in the store's
        /// extended properties collection.</param>
        /// <param name="remoteId">The remote ID to be prepended with the app's GUID.</param>
        /// <returns></returns>
        public async Task<string> GetTaggedRemoteId(ContactStore store, string remoteId)
        {
            string taggedRemoteId = string.Empty;

            System.Collections.Generic.IDictionary<string, object> properties;
            properties = await store.LoadExtendedPropertiesAsync().AsTask<System.Collections.Generic.IDictionary<string, object>>();
            if (properties.ContainsKey(ContactStoreLocalInstanceIdKey))
            {
                taggedRemoteId = string.Format("{0}_{1}", properties[ContactStoreLocalInstanceIdKey], remoteId);
            }
            else
            {
                // handle error condition
            }

            return taggedRemoteId;
        }

        /// <summary>
        /// Removes the prepended GUID from the supplied remote ID.
        /// </summary>
        /// <param name="store">The app's contact store. The remote ID Guid is stored in the store's
        /// extended properties collection.</param>
        /// <param name="taggedRemoteId">The remote ID from which the app's GUID should be removed.</param>
        /// <returns></returns>
        public async Task<string> GetUntaggedRemoteId(ContactStore store, string taggedRemoteId)
        {
            string remoteId = string.Empty;

            System.Collections.Generic.IDictionary<string, object> properties;
            properties = await store.LoadExtendedPropertiesAsync().AsTask<System.Collections.Generic.IDictionary<string, object>>();
            if (properties.ContainsKey(ContactStoreLocalInstanceIdKey))
            {
                string localInstanceId = properties[ContactStoreLocalInstanceIdKey] as string;
                if (taggedRemoteId.Length > localInstanceId.Length + 1)
                {
                    remoteId = taggedRemoteId.Substring(localInstanceId.Length + 1);
                }
            }
            else
            {
                // handle error condition
            }

            return remoteId;
        }

    }

}

