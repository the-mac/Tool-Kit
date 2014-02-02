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
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Wallet;

namespace sdkWalletMembershipDealsWP8CS
{
    public partial class CouponsPage : PhoneApplicationPage
    {
        /// <summary>
        /// Constructor for Deals
        /// </summary>
        public CouponsPage()
        {
            InitializeComponent();

            // Set data context to webservice for data binding to list
            DataContext = MockWebService.WebService;

            // Load mock data
            MockWebService.WebService.LoadData();
        }

        /// <summary>
        /// When we navigate to deals we must reinitialize the UI to properly display the wallet status changes
        /// </summary>
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            // Checking if deals found on web service are currently in wallet
            CheckWalletStatus();

            base.OnNavigatedTo(e);
        }

        /// <summary>
        /// Checks if the deals found on web service are currently in wallet
        /// </summary>
        public void CheckWalletStatus()
        {
            foreach (Coupon mockDeal in MockWebService.WebService.Deals)
            {
                mockDeal.WalletDeal = Wallet.FindItem(mockDeal.ID) as Deal;

                if (mockDeal.WalletDeal != null)
                {
                    mockDeal.WalletStatus = "In Wallet";
                }
                else
                {
                    mockDeal.WalletStatus = "Not In Wallet";
                }
            }
        }

        // Event fired when selection is changed in deals list
        private void DealsList_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            // If selected item is null (no selection) do nothing
            if (DealsList.SelectedItem == null)
                return;

            // Navigate to the new page
            NavigationService.Navigate(new Uri("/CouponView.xaml?ID=" + (DealsList.SelectedItem as Coupon).ID, UriKind.Relative));

            // Deselect the item so that user can click it again when they revisit this screen.
            DealsList.SelectedItem = null;
        }
    }
}
