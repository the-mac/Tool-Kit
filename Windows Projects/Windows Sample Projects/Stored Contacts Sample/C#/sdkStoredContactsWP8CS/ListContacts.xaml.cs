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
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

using Windows.Phone.PersonalInformation;

namespace sdkStoredContactsWP8CS
{
    public partial class ListContacts : PhoneApplicationPage
    {

        // A list of the MyRemoteContact struct, defined in MainPage.xaml.cs. Because this structure
        // stores all used fields as string properties, it's easier to use for data binding.
        public List<MyRemoteContact> contactsList;

        public ListContacts()
        {
            InitializeComponent();
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Clear the list of MyRemoteContact structures
            contactsList = new List<MyRemoteContact>();

            // Open the contact store
            var contactStore = await ContactStore.CreateOrOpenAsync(ContactStoreSystemAccessMode.ReadWrite,
                ContactStoreApplicationAccessMode.ReadOnly);

            // RemoteIdHelper class to help with managing remote IDs
            var remoteIdHelper = new RemoteIdHelper();

            // Get contacts from the store using the default query
            var queryResult = contactStore.CreateContactQuery();
            var storedContacts = await queryResult.GetContactsAsync();

            // Loop through each StoredContact object and create a parallel MyRemoteContact object
            foreach (var contact in storedContacts)
            {
                var props = await contact.GetPropertiesAsync();
                var extprops = await contact.GetExtendedPropertiesAsync();

                var remoteContact = new MyRemoteContact
                {
                    GivenName = contact.GivenName,
                    FamilyName = contact.FamilyName,
                    DisplayName = contact.DisplayName,
                    Email = props["Email"] != null ? (string)props["Email"] : null,
                    CodeName = extprops["CodeName"] != null ? (string)extprops["CodeName"] : null,
                };

                // Add to the list
                contactsList.Add(remoteContact);
            }

            // Bind the list box to the list of contacts
            ContactListBox.ItemsSource = contactsList;
        }
    }
}
