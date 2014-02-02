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
using System.Linq;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Wallet;

namespace sdkWalletMembershipDealsWP8CS
{
    public partial class CouponView : PhoneApplicationPage
    {

        // Object containing the current coupon
        Coupon currentCoupon;

        /// <summary>
        /// Constructor for the Deal View
        /// </summary>
        public CouponView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// When navigated to set the data context to the current deal
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string passedID = "";
            if (NavigationContext.QueryString.TryGetValue("ID", out passedID))
            {
                // Set the current deal.
                currentCoupon = MockWebService.WebService.Deals.First<Coupon>(item => item.ID == passedID);

                // Set the data context to current deal.
                DataContext = currentCoupon;

                // Set the expiration text
                this.DealExpiration.Text = "Expires " + currentCoupon.ExpirationDate.ToShortDateString();

                // Check to see if current deal is in wallet and update UI accordingly.
                if (currentCoupon.WalletDeal != null)
                {
                    ShowRemoveFromWallet();
                }
                else
                {
                    ShowSaveToWallet();
                }
            }
            else
            {
                MessageBox.Show("There was an error while loading the deal.");
            }
        }

        /// <summary>
        /// Shows remove from wallet UI
        /// </summary>
        private void ShowRemoveFromWallet()
        {
            this.RemoveFromWallet.Visibility = System.Windows.Visibility.Visible;
            this.SaveToWallet.Visibility = System.Windows.Visibility.Collapsed;
        }

        /// <summary>
        /// Shows save to wallet UI
        /// </summary>
        private void ShowSaveToWallet()
        {
            this.RemoveFromWallet.Visibility = System.Windows.Visibility.Collapsed;
            this.SaveToWallet.Visibility = System.Windows.Visibility.Visible;
        }

        // Click even for saving deal to wallet
        private async void SaveToWallet_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                // Create a new Deal.
                Deal deal = new Deal(currentCoupon.ID);
                deal.DisplayName = currentCoupon.Title;
                deal.Description = currentCoupon.Description;
                deal.IssuerName = MockWebService.WebService.IssuerName;
                deal.IssuerWebsite = new Uri(MockWebService.WebService.IssuerEmail);
                deal.IsUsed = false;
                deal.MerchantName = MockWebService.WebService.IssuerName;
                deal.TermsAndConditions = currentCoupon.Terms;
                deal.Code = currentCoupon.Code;
                deal.ExpirationDate = currentCoupon.ExpirationDate;
                deal.BarcodeImage = currentCoupon.Barcode;

                // Show progress bar
                SaveProgressBar.IsIndeterminate = true;
                SaveProgressBar.Visibility = System.Windows.Visibility.Visible;

                // Save deal to Wallet.
                await deal.SaveAsync();


                ShowRemoveFromWallet();

            }
            catch (Exception ex)
            {
                MessageBox.Show("There was an error while saving your deal: " + ex.Message);
            }

            // Hide progress bar and set IsIndeterminate to false so that it resets.
            SaveProgressBar.IsIndeterminate = false;
            SaveProgressBar.Visibility = System.Windows.Visibility.Collapsed;

        }

        // Click event for removing the current deal
        private void RemoveFromWallet_Click_1(object sender, RoutedEventArgs e)
        {
            // Show removing confirmation
            MessageBoxResult mResult = MessageBox.Show("Are you sure you want to remove this coupon from the wallet?", "Coupon Removal", MessageBoxButton.OKCancel);

            if (mResult == MessageBoxResult.OK)
            {
                try
                {
                    Wallet.Remove(currentCoupon.ID);

                    ShowSaveToWallet();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("There was an error removing the deal: " + ex.Message);
                }
            }
        }
    }
}
