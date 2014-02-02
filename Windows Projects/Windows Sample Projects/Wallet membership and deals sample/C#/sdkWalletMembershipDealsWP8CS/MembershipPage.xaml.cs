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
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using Microsoft.Phone.Wallet;

namespace sdkWalletMembershipDealsWP8CS
{
    public partial class MembershipPage : PhoneApplicationPage
    {
        // Add wallet item task
        static AddWalletItemTask addWalletItemTask = new AddWalletItemTask();

        // Membership from web service
        Membership webServiceMembership;

        /// <summary>
        /// Initialize Membership and set event.
        /// </summary>
        public MembershipPage()
        {
            InitializeComponent();

            // Load membership and refresh UI
            LoadMembership();

            // Set completion event after wallet item task
            addWalletItemTask.Completed += new EventHandler<Microsoft.Phone.Tasks.AddWalletItemResult>(awic_Completed);
        }

        /// <summary>
        /// Add wallet item task completion function.
        /// </summary>
        void awic_Completed(object sender, Microsoft.Phone.Tasks.AddWalletItemResult e)
        {
            if (e.TaskResult == Microsoft.Phone.Tasks.TaskResult.OK)
            {
                MessageBox.Show(e.Item.DisplayName + " was added to your wallet!");

                //Reload membership with wallet item added to refresh UI
                LoadMembership();
            }
            else if (e.TaskResult == Microsoft.Phone.Tasks.TaskResult.Cancel)
            {
                MessageBox.Show("Cancelled");
            }
            else if (e.TaskResult == Microsoft.Phone.Tasks.TaskResult.None)
            {
                MessageBox.Show("None");
            }

        }

        /// <summary>
        /// This function determines if a membership exists in the webservice, if there exists a webservice membership,
        /// it checks if the associated wallettransactionitem is in the wallet. It then displays the correct UI.
        /// </summary>
        public void LoadMembership()
        {
            // Check webservice for membership       
            webServiceMembership = MockWebService.WebService.GetMembership();

            if (webServiceMembership != null)
            {
                // If we found a membership from web service

                // Show view membership panel.
                ViewMembershipPanel.Visibility = System.Windows.Visibility.Visible;
                GetMembershipPanel.Visibility = System.Windows.Visibility.Collapsed;

                // Make Data context the web service
                DataContext = webServiceMembership;

                // Check wallet for membership card
                WalletTransactionItem walletMembershipCard = LoadMembershipFromWallet();

                if (walletMembershipCard != null)
                {
                    // If we found a membership card in wallet

                    // Show remove membership from wallet button
                    RemoveFromWallet.Visibility = System.Windows.Visibility.Visible;
                    SaveToWallet.Visibility = System.Windows.Visibility.Collapsed;
                }
                else
                {
                    // If we didn't find membership card in wallet

                    // Show save membership to wallet button
                    RemoveFromWallet.Visibility = System.Windows.Visibility.Collapsed;
                    SaveToWallet.Visibility = System.Windows.Visibility.Visible;
                }
            }
            else
            {
                // If we didn't find a membership in the web service.
                
                // Remove any existing wallet membership cards as we do not have an associated web service item
                RemoveMembershipFromWallet(true);

                // Show get a membership panel.
                ViewMembershipPanel.Visibility = System.Windows.Visibility.Collapsed;
                GetMembershipPanel.Visibility = System.Windows.Visibility.Visible;
            }
        }

        /// <summary>
        /// Load Wallet Membership card
        /// </summary>
        /// <returns></returns>
        private WalletTransactionItem LoadMembershipFromWallet()
        {
            try
            {
                // Retrieve membership from the wallet.
                return Wallet.FindItem("membership") as WalletTransactionItem;
            }
            catch (Exception ex)
            {
                MessageBox.Show("There were errors loading membership from wallet: " + ex.Message);
            }
            return null;
        }

        /// <summary>
        /// Checks if all fields are filled in
        /// </summary>
        /// <returns>Whether all fields are filled in</returns>
        private bool CheckFilledFields()
        {
            bool filledIn = true;
            StringBuilder sb = new StringBuilder();
            sb.Append("Please fill in the following fields:");

            if (String.IsNullOrEmpty(FirstNameInput.Text.Trim()))
            {
                sb.Append(" First Name,");
                filledIn = false;
            }
            if (String.IsNullOrEmpty(LastNameInput.Text.Trim()))
            {
                sb.Append(" Last Name,");
                filledIn = false;
            }
            if (String.IsNullOrEmpty(PhoneNumberInput.Text.Trim()))
            {
                sb.Append(" Phone Number,");
                filledIn = false;
            }
            if (String.IsNullOrEmpty(EmailInput.Text.Trim()))
            {
                sb.Append(" Email.");
                filledIn = false;
            }

            if (!filledIn)
            {
                // Display message box with errors.
                MessageBox.Show(sb.ToString());
            }

            return filledIn;
        }

        /// <summary>
        /// Uses AddWalletItemTask to add membership to the wallet.
        /// </summary>
        public void AddMembershipToWallet(Membership membership)
        {
            try
            {
                WalletTransactionItem membershipItem;
                membershipItem = new WalletTransactionItem("membership");
                membershipItem.IssuerName = MockWebService.WebService.IssuerName;
                membershipItem.DisplayName = MockWebService.WebService.IssuerName + " Membership Card";
                membershipItem.IssuerPhone.Business = MockWebService.WebService.IssuerPhone;
                membershipItem.CustomerName = membership.CustomerName;
                membershipItem.AccountNumber = membership.MembershipNumber;
                membershipItem.BillingPhone = membership.PhoneNumber;
                membershipItem.IssuerWebsite = new Uri(MockWebService.WebService.IssuerEmail);
                membershipItem.CustomProperties.Add("email", new CustomWalletProperty(membership.PhoneNumber));
                membershipItem.DisplayAvailableBalance = "1000 points";
                BitmapImage bmp = new BitmapImage();
                using (System.IO.Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("sdkWalletMembershipDealsWP8CS.Assets.adventure.jpg"))
                    bmp.SetSource(stream);

                membershipItem.Logo99x99 = bmp;
                membershipItem.Logo159x159 = bmp;
                membershipItem.Logo336x336 = bmp;
                addWalletItemTask.Item = membershipItem;
                addWalletItemTask.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("There were the following errors when saving your membership to the wallet: " + ex.Message);
            }
        }

        /// <summary>
        /// Removes the membership card from wallet and refreshes UI.
        /// </summary>
        private void RemoveMembershipFromWallet(bool skipDialog)
        {
            MessageBoxResult mResult;
            if (skipDialog)
            {
                mResult = MessageBoxResult.OK;
            }
            else
            {
                mResult = MessageBox.Show("Are you sure you want to remove your membership card from the wallet?", "Membership Removal", MessageBoxButton.OKCancel);
            }

            if (mResult == MessageBoxResult.OK)
            {
                // Get the membership card in wallet
                WalletTransactionItem walletMembershipCard = LoadMembershipFromWallet();

                // If the wallet membership card is not null, remove it. 
                if (walletMembershipCard != null)
                {
                    Wallet.Remove(walletMembershipCard);
                    LoadMembership();
                }


            }
        }


        /// <summary>
        /// Button click event that signs the user up for a membership with mock web service.
        /// </summary>
        private void SignUpMembership_Click(object sender, RoutedEventArgs e)
        {
            if (CheckFilledFields())
            {
                // Signs up with web service synchronously.
                MockWebService.WebService.MembershipSignUp(FirstNameInput.Text, LastNameInput.Text, PhoneNumberInput.Text, EmailInput.Text);

                // Calls load membership after signed up to refresh UI.
                LoadMembership();
            }
        }

        /// <summary>
        /// Button listener that calls add membership
        /// </summary>
        private void SaveToWallet_Click_1(object sender, RoutedEventArgs e)
        {
            AddMembershipToWallet(webServiceMembership);
        }

        /// <summary>
        /// Button listener that deletes membership in both phone and wallet.
        /// </summary>
        private void DeleteMembership_Click_1(object sender, RoutedEventArgs e)
        {
            MessageBoxResult mResult = MessageBox.Show("Are you sure that you want to delete your membership, which in turn will remove any associated wallet membership cards?", "Membership Removal", MessageBoxButton.OKCancel);

            if (mResult == MessageBoxResult.OK)
            {
                // Removes membership from webservice
                MockWebService.WebService.MembershipDelete();

                // Refresh UI
                LoadMembership();
            }
        }

        /// <summary>
        /// Removes membership from wallet and reloads the Membership content.
        /// </summary>
        private void RemoveFromWallet_Click_1(object sender, RoutedEventArgs e)
        {
            RemoveMembershipFromWallet(false);
        }
    }
}
