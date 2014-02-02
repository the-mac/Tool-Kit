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
using System.Collections.ObjectModel;
using System.IO.IsolatedStorage;
using Microsoft.Phone.Wallet;

namespace sdkWalletMembershipDealsWP8CS
{
    /// <summary>
    /// This class mimics a web service that an ISV might call to perform a transaction, find deals relevant to a user, and manage memberships.
    /// </summary>
    public class MockWebService
    {
        private static MockWebService webService = null;

        /// <summary>
        /// A static singleton WebService.
        /// </summary>
        /// <returns>The MainWebService object.</returns>
        public static MockWebService WebService
        {
            get
            {
                if (webService == null)
                    webService = new MockWebService();

                return webService;
            }
        }

        /// <summary>
        /// Mock Web Service constructor
        /// </summary>
        public MockWebService()
        {
            // Initialize deals collection
            this.Deals = new ObservableCollection<Coupon>();

            // Initialize with loaded deals
            LoadData();
        }

        // Issuer Details
        public string IssuerName = "Adventure Works";
        public string IssuerPhone = "123456789";
        public string IssuerEmail = "http://www.adventure-works.com/";

        // Membership number to be populated by webservice.
        public string MembershipNumber = "01234567890";

        /// <summary>
        /// Deal observable collection for data binding.
        /// </summary>
        public ObservableCollection<Coupon> Deals { get; private set; }

        /// <summary>
        /// Creates and adds some mock deals, pull from web service in future.
        /// </summary>
        public void LoadData()
        {
            // Clear out old deals
            this.Deals.Clear();

            // Add a few mock deals
            this.Deals.Add(new Coupon("1", "10% Off") { Description = "10% off any purchase!", ExpirationDate = System.DateTime.Now + new TimeSpan(1,1,1,1), Terms = "Valid at participating locations only", Code = "12345678"});
            this.Deals.Add(new Coupon("2", "$50 off bike") { Description = "$50 off the purchase of a bike!", ExpirationDate = System.DateTime.Now + new TimeSpan(2, 2, 2, 2), Terms = "Only valid for selected bikes.", Code = "11112222" });
            this.Deals.Add(new Coupon("3", "Free Membership") { Description = "Free 1 year membership!", ExpirationDate = new DateTime(2012, 1, 1), Terms = "First time members only.", Code = "22223333"});
            this.Deals.Add(new Coupon("4", "20$ off") { Description = "$20 off your next purchase!", ExpirationDate = new DateTime(2012, 1, 1), Terms = "Valid at participating locations only", Code = "44445555"});
        }

        /// <summary>
        /// Performs a fake transaction. If the card would be put at a negative balance, restores the balance to 1000.
        /// </summary>
        /// <param name="card">The card to perform the transaction on. The display balance is currently formatted as "1000 points"</param>
        /// <returns>The new balance.</returns>
        public int PerformMockTransaction(WalletTransactionItem card)
        {
            int newBalance;

            if (Int32.TryParse(card.DisplayAvailableBalance.Split()[0], out newBalance))
            {
                newBalance -= 5;

                if (newBalance < 0)
                    newBalance = 1000;
            }
            return newBalance;
        }

        /// <summary>
        /// Returns a random deal that we pretend is relevant to a user. This function is used in the wallet agent.
        /// </summary>
        /// <returns>A random mock deal.</returns>
        public Coupon GetRelevantDealForUser()
        {
            Random rnd = new Random();
            int dealId = rnd.Next(Deals.Count);
            return Deals[dealId];
        }

        /// <summary>
        /// Mocks membership signup with isolated storage. May want to make async with a real web service.
        /// </summary>
        public void MembershipSignUp(string MemberFirstName, string MemberLastName, string MemberPhoneNumber, string MemberEmail)
        {
            // Save membership to application storage
            IsolatedStorageSettings isolatedStore = IsolatedStorageSettings.ApplicationSettings;
            isolatedStore.Add("FirstName", MemberFirstName);
            isolatedStore.Add("LastName", MemberLastName);
            isolatedStore.Add("Phone", MemberPhoneNumber);
            isolatedStore.Add("Email", MemberEmail);
            isolatedStore.Add("MembershipNumber", MembershipNumber);
            isolatedStore.Save();
        }

        /// <summary>
        /// Mocks deleting a membership from web service.
        /// </summary>
        public void MembershipDelete()
        {
            // Remove all fields of membership from isolated storage
            IsolatedStorageSettings isolatedStore = IsolatedStorageSettings.ApplicationSettings;
            isolatedStore.Clear();
        }

        /// <summary>
        /// Mocks retrieving membership from web service.
        /// </summary>
        /// <returns></returns>
        public Membership GetMembership()
        {
            Membership myMembership = null;

            IsolatedStorageSettings isolatedStore = IsolatedStorageSettings.ApplicationSettings;
            string outString;

            if (isolatedStore.TryGetValue<string>("FirstName", out outString))
            {
                //If isolated storage has a name field, then there is a saved membership.
                myMembership = new Membership();

                string firstName = outString;
                if (isolatedStore.TryGetValue<string>("LastName", out outString))
                {
                    myMembership.CustomerName = firstName + " " + outString;
                }

                if (isolatedStore.TryGetValue<string>("MembershipNumber", out outString))
                {
                    myMembership.MembershipNumber = outString;
                }

                if (isolatedStore.TryGetValue<string>("Phone", out outString))
                {
                    myMembership.PhoneNumber = outString;
                }

                if (isolatedStore.TryGetValue<string>("Email", out outString))
                {
                    myMembership.Email = outString;
                }
            }

            return myMembership;
        }
    }
}
