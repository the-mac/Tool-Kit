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
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;

using System.Xml.Linq;
using Windows.Phone.PersonalInformation;
using System.Threading.Tasks;
using System.Diagnostics;

namespace sdkStoredContactsWP8CS
{
    /// <summary>
    /// Helper class for holding remote contact data. Because each piece of contact data used by the sample app
    /// is exposed as a property, it makes parsing the XML contact data easier. The values of these fields are
    /// then copied into the StoredContact class when the contact is saved to the phone.
    /// </summary>
    public class MyRemoteContact
    {
        // These properties represent the contact data used by the sample app.
        public string RemoteId { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string CodeName { get; set; }

        // A utility function used to format debug output.
        public override string ToString()
        {
            return String.Format(" {0}\n {1}\n {2}\n {3}\n {4}\n {5}", RemoteId, GivenName, FamilyName, DisplayName, Email, CodeName);
        }


    }
    public partial class MainPage : PhoneApplicationPage
    {

        ContactStore contactStore;
        RemoteIdHelper remoteIdHelper;

        // Constructor
        public MainPage()
        {
            InitializeComponent();


            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();           
        }
        /// <summary>
        /// When the MainPage OnNavigatedTo handler is called, the CreateOrOpenAsync is called. The first time
        /// the app is run, this call creates the contact store. Subsequent calls instantiate the existing store.
        /// The RemoteIdHelper is a helper class defined by the sample app. It helps to ensure that the remote
        /// IDs for all contacts are unique for this app.
        /// </summary>
        /// <param name="e"></param>
        async protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            contactStore = await ContactStore.CreateOrOpenAsync(ContactStoreSystemAccessMode.ReadWrite,
                ContactStoreApplicationAccessMode.ReadOnly);

            remoteIdHelper = new RemoteIdHelper();
            await remoteIdHelper.SetRemoteIdGuid(contactStore);


        }
        /// <summary>
        /// A helper function that saves a key/value pairs to the ExtendedProperties dictionary of the contact
        /// store. The sample app uses this to track the last synced revision number.
        /// </summary>
        /// <param name="s">The key</param>
        /// <param name="o">The value</param>
        async private void SaveStoreExtendedProperty(string s, object o)
        {
            IDictionary<string, object> properties;
            properties = await contactStore.LoadExtendedPropertiesAsync().AsTask<IDictionary<string, object>>();
            properties[s] = o;
            System.Collections.ObjectModel.ReadOnlyDictionary<string, object> readonlyProperties = new System.Collections.ObjectModel.ReadOnlyDictionary<string, object>(properties);
            await contactStore.SaveExtendedPropertiesAsync(readonlyProperties);
        }
        /// <summary>
        /// Called when the Import Remote Contacts button is pressed. A typical app would retrieve the remote
        /// contact data from a web service. In order to keep the sample app self-contained, the web service
        /// result is simulated with a local XML file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            StatusTextBlock.Text = "Importing contacts...";
            var xmldoc = XDocument.Load("RemoteContactStore.xml");
            await ParseXmlResponse(xmldoc);
            StatusTextBlock.Text = "Done.";
        }
        /// <summary>
        /// Called when the Update Remote Contacts button is pressed. A typical app would retrieve updates 
        /// to the remote contact data from a web service. In order to keep the sample app self-contained, the web 
        /// service result is simulated with a local XML file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            StatusTextBlock.Text = "Syncing remote changes...";
            var xmldoc = XDocument.Load("RemoteContactUpdates.xml");
            await ParseXmlResponse(xmldoc);
            StatusTextBlock.Text = "Done.";
        }

        /// <summary>
        /// Takes the XML representing the contacts to be inserted, updated, or deleted and creates an
        /// instance of the MyRemoteContact helper class to hold the data. Then, the helper object is passed
        /// to the function for the appropriate operation. When all of the changes are complete, 
        /// the SaveExtendedProperty helper method is called to update the last synced revision number.</summary>
        /// <param name="xmldoc">The XML document containing the contact data.</param>
        /// <returns></returns>
        private async Task ParseXmlResponse(XDocument xmldoc)
        {
            var contactElements = xmldoc.Descendants("Contact");

            foreach (var el in contactElements)
            {
                var remoteContact = new MyRemoteContact
                {
                    RemoteId = el.Element("RemoteId") != null ? el.Element("RemoteId").Value : null,
                    GivenName = el.Element("GivenName") != null ? el.Element("GivenName").Value : null,
                    FamilyName = el.Element("FamilyName") != null ? el.Element("FamilyName").Value : null,
                    DisplayName = el.Element("DisplayName") != null ? el.Element("DisplayName").Value : null,
                    Email = el.Element("Email") != null ? el.Element("Email").Value : null,
                    CodeName = el.Element("CodeName") != null ? el.Element("CodeName").Value : null,
                };

                if (el.Attribute("ChangeType") != null)
                {
                    switch (el.Attribute("ChangeType").Value)
                    {
                        case "Create":
                            await AddContact(remoteContact);
                            break;
                        case "Update":
                            await UpdateContact(remoteContact);
                            break;
                        case "Delete":
                            await DeleteContact(remoteContact);
                            break;
                    }
                }
            }

            var remoteRevision = xmldoc.Root.Attribute("RemoteRevision").Value;
            SaveStoreExtendedProperty("LastRemoteRevision", remoteRevision);
            Debug.WriteLine(String.Format("Storing remote revision number: {0}", remoteRevision));

            SaveStoreExtendedProperty("LastLocalRevision", contactStore.RevisionNumber.ToString());
            Debug.WriteLine(String.Format("Storing local revision number: {0}", contactStore.RevisionNumber.ToString()));

        }
        /// <summary>
        /// Adds a contact to the contact store using data supplied in the MyRemoteContact helper object.
        /// </summary>
        /// <param name="remoteContact">The MyRemoteContact helper object representing the contact to be added.</param>
        /// <returns></returns>
        public async Task AddContact(MyRemoteContact remoteContact)
        {

            // Create a new StoredContact object
            StoredContact contact = new StoredContact(contactStore);

            // Make sure the remote contact has a RemoteId value
            if (remoteContact.RemoteId == null)
            {
                return;
            }

            // Use the RemoteIdHelper class to add a GUID to the remote ID to make sure
            // the value is unique to this app
            contact.RemoteId = await remoteIdHelper.GetTaggedRemoteId(contactStore, remoteContact.RemoteId);

            // Set the properties that are exposed directly by the StoredContact class
            if (remoteContact.GivenName != null) contact.GivenName = remoteContact.GivenName;
            if (remoteContact.FamilyName != null) contact.FamilyName = remoteContact.FamilyName;

            // If you don't supply a display name, "[GivenName] [FamilyName]" will be used, but it won't
            // automatically be updated when you update GivenName or FamilyName.
            if (remoteContact.DisplayName != null) contact.DisplayName = remoteContact.DisplayName;


            // Call GetPropertiesAsync to get the dictionary of properties that are understood by the phone. 
            // The keys for this dictionary must be values from the KnownContactProperies enumeration.
            IDictionary<string, object> props = await contact.GetPropertiesAsync();
            if (remoteContact.Email != null) props.Add(KnownContactProperties.Email, remoteContact.Email);

            // Call GetPropertiesAsync to get the dictionary of properties that are specific to your app.
            // In this case, the app will set a CodeName property.
            IDictionary<string, object> extprops = await contact.GetExtendedPropertiesAsync();
            if (remoteContact.CodeName != null) extprops.Add("CodeName", remoteContact.CodeName);

            try
            {
                // Call SaveAsync to save the contact into the store.
                await contact.SaveAsync();
                Debug.WriteLine(String.Format("Adding:\n{0}", remoteContact.ToString()));
            }
            catch (Exception)
            {
                Debug.WriteLine(String.Format("Unable to add contact: {0}", remoteContact.ToString()));
            }
        }
        /// <summary>
        /// Updates a contact in the contact store with data supplied in the MyRemoteContact helper object.
        /// </summary>
        /// <param name="remoteContact">The MyRemoteContact helper object representing the contact to be updated.</param>
        /// <returns></returns>
        public async Task UpdateContact(MyRemoteContact remoteContact)
        {
            // Use the RemoteIdHelper class to add a GUID to the remote ID to make sure
            // the value is unique to this app
            string taggedRemoteId = await remoteIdHelper.GetTaggedRemoteId(contactStore, remoteContact.RemoteId);

            // Call FindContactByRemoteIdAsync to obtain the StoredContact object to be updated
            StoredContact contact = await contactStore.FindContactByRemoteIdAsync(taggedRemoteId);

            if (contact != null)
            {

                // Set the properties that are exposed directly by the StoredContact class
                contact.GivenName = remoteContact.GivenName;
                contact.FamilyName = remoteContact.FamilyName;

                // If you let the system create automatically create a display name when creating a contact,
                // the display name is not automatically be updated when you update the contact's GivenName or FamilyName.
                if (remoteContact.DisplayName != null) contact.DisplayName = remoteContact.DisplayName;


                // Call GetPropertiesAsync to get the dictionary of properties that are understood by the phone. 
                // The keys for this dictionary must be values from the KnownContactProperies enumeration.
                IDictionary<string, object> props = await contact.GetPropertiesAsync();
                props[KnownContactProperties.Email] = remoteContact.Email;

                // Call GetPropertiesAsync to get the dictionary of properties that are specific to your app.
                // In this case, the app will set a CodeName property.
                IDictionary<string, object> extprops = await contact.GetExtendedPropertiesAsync();
                extprops["CodeName"] = remoteContact.CodeName;


                try
                {
                    // Call SaveAsync to save the changes to the contact into the store.
                    await contact.SaveAsync();
                    Debug.WriteLine(String.Format("Updating:\n{0}", remoteContact.ToString()));
                }
                catch (Exception)
                {
                    Debug.WriteLine(String.Format("Unable to update contact:\n{0}", remoteContact.ToString()));
                }
            }
            else
            {
                Debug.WriteLine(String.Format("Contact not found:\n{0}", remoteContact.ToString()));
            }
        }
        /// <summary>
        /// Updates a contact specified the MyRemoteContact helper object from the contact store.
        /// </summary>
        /// <param name="remoteContact"></param>
        /// <returns></returns>
        public async Task DeleteContact(MyRemoteContact remoteContact)
        {

            // Use the RemoteIdHelper class to add a GUID to the remote ID to make sure
            // the value is unique to this app
            string taggedRemoteId = await remoteIdHelper.GetTaggedRemoteId(contactStore, remoteContact.RemoteId);

            // Call FindContactByRemoteIdAsync to obtain the StoredContact object to be deleted
            StoredContact contact = await contactStore.FindContactByRemoteIdAsync(taggedRemoteId);

            if (contact != null)
            {
                try
                {
                    // Use the Id property of the stored contact to specify the contact to be deleted.
                    await contactStore.DeleteContactAsync(contact.Id);
                    Debug.WriteLine(String.Format("Deleting:\n{0}", remoteContact.ToString()));
                }
                catch (Exception)
                {
                    Debug.WriteLine(String.Format("Unable to delete contact:\n{0}", remoteContact.ToString()));
                }
            }
            else
            {
                Debug.WriteLine(String.Format("Contact not found:\n{0}", remoteContact.ToString()));
            }
        }
        /// <summary>
        /// Called when the Get Unsynced Local Changes button is pressed. Gets the last synced revision
        /// number out of the contact store's extended properties and then calls GetChanges.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void GetUnsyncedChangesButton_Click(object sender, RoutedEventArgs e)
        {

            IDictionary<string, object> properties;
            properties = await contactStore.LoadExtendedPropertiesAsync().AsTask<IDictionary<string, object>>();
            ulong lastSyncedLocalRevision = ulong.Parse(properties["LastLocalRevision"].ToString());


            await GetChanges(lastSyncedLocalRevision);

            SaveStoreExtendedProperty("LastLocalRevision", contactStore.RevisionNumber.ToString());
            Debug.WriteLine(String.Format("Storing local revision number: {0}", contactStore.RevisionNumber.ToString()));
        }
        /// <summary>
        /// Gets the list of changes in the local contact store since the specified revision number
        /// and generates an XML document that could be used to convey the changes to the app's web service
        /// </summary>
        /// <param name="revision">The revision number of the local store indicating which changes
        /// should be retrieved.</param>
        /// <returns></returns>
        public async Task GetChanges(ulong revision)
        {
            Debug.WriteLine(String.Format("Getting changes since revision: {0}", revision));

            // Create a new XML document and add a root node.
            var doc = new XDocument();
            doc.Add(new XElement("LocalChanges"));


            // Call GetChangesAsync to get all changes since the specified revision.
            var changeList = await contactStore.GetChangesAsync(revision);

            foreach (var change in changeList)
            {
                // Each change record returned contains the change type, remote and local ids, and revision number
                Debug.WriteLine(String.Format("Change Type: {0}\nLocal ID: {1}\nRemote ID: {2}\nRevision Number: {3}",
                    change.ChangeType.ToString(),
                    change.Id,
                    await remoteIdHelper.GetUntaggedRemoteId(contactStore, change.RemoteId),
                    change.RevisionNumber));

                // Get the contact associated with the change record using the Id property.
                var contact = await contactStore.FindContactByIdAsync(change.Id);

                if (contact != null)
                {
                    // Create an xml element to represent the local contact

                    var changeElement = new XElement("Contact", new XAttribute("ChangeType", change.ChangeType.ToString()));
                    changeElement.Add(new XElement("RemoteId", await remoteIdHelper.GetUntaggedRemoteId(contactStore, contact.RemoteId)));
                    changeElement.Add(new XElement("GivenName", await remoteIdHelper.GetUntaggedRemoteId(contactStore, contact.GivenName)));
                    changeElement.Add(new XElement("FamilyName", await remoteIdHelper.GetUntaggedRemoteId(contactStore, contact.FamilyName)));
                    var props = await contact.GetPropertiesAsync();
                    if (props.ContainsKey("Email"))
                    {
                        changeElement.Add(new XElement("Email", (string)props["Email"]));
                    }

                    var extprops = await contact.GetExtendedPropertiesAsync();
                    if (extprops.ContainsKey("CodeName"))
                    {
                        changeElement.Add(new XElement("CodeName", (string)extprops["CodeName"]));
                    }

                    // Append the contact element to the document
                    doc.Root.Add(changeElement);
                }


            }

            Debug.WriteLine(doc.ToString());
        }

        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}
