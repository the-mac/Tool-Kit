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
using sdkWalletPaymentInstrumentsWP8CS.Resources;

namespace sdkWalletPaymentInstrumentsWP8CS
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Set data context to webservice for data binding to list.
            DataContext = MockWebService.WebService;

            // Load mock data.
            MockWebService.WebService.LoadData();
        }

        /// <summary>
        /// When we navigate main page and show the cards, we must reinitialize the UI to properly
        /// display the wallet status changes.
        /// </summary>
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            // Checking if deals found on web service are currently in wallet.
            CheckWalletStatus();

            base.OnNavigatedTo(e);
        }

        /// <summary>
        /// Checks if the cards found on web service are currently in wallet.
        /// </summary>
        public void CheckWalletStatus()
        {
            foreach (BankCard bankCard in MockWebService.WebService.Cards)
            {
                // Find a specific item in the Wallet.
                bankCard.WalletCard = Wallet.FindItem(bankCard.id) as PaymentInstrument;

                if (bankCard.WalletCard != null)
                {
                    bankCard.WalletStatus = AppResources.InWalletStatus;
                }
                else
                {
                    bankCard.WalletStatus = AppResources.NotInWalletStatus;
                }
            }
        }

        // Event fired when selection is changed in cards list.
        private void CardsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // If selected item is null (no selection) do nothing.
            if (CardsList.SelectedItem == null)
                return;

            // Navigate to the new page.
            NavigationService.Navigate(new Uri("/CardPage.xaml?ID=" + (CardsList.SelectedItem as BankCard).id, UriKind.Relative));

            // Deselect the item so that user can click it again when they revisit this screen.
            CardsList.SelectedItem = null;
        }
    }
}
