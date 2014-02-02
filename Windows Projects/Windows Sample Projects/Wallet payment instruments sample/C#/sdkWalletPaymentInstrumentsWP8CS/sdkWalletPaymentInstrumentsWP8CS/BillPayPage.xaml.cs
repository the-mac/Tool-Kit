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
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Wallet;
using sdkWalletPaymentInstrumentsWP8CS.Resources;


namespace sdkWalletPaymentInstrumentsWP8CS
{
    public partial class BillPayPage : PhoneApplicationPage
    {
        BankCreditCard currentBankCard;

        public BillPayPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// When navigated to set the data context to the current card and find the corresponding Wallet item.
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string passedID = "";
            if (NavigationContext.QueryString.TryGetValue("ID", out passedID))
            {
                // Get the passed in card id and set the data context to that card.
                currentBankCard = (BankCreditCard) MockWebService.WebService.Cards.First<BankCard>(item => item.id == passedID);

                if (currentBankCard != null)
                {
                    // Set the data context to current card.
                    DataContext = currentBankCard;

                    System.Globalization.CultureInfo savedCultureInfo = System.Globalization.CultureInfo.CurrentCulture;
                    System.Globalization.CultureInfo.DefaultThreadCurrentCulture = new System.Globalization.CultureInfo(currentBankCard.CultureName);

                    // Find the corresponding Wallet item.
                    currentBankCard.WalletCard = Wallet.FindItem(currentBankCard.id) as PaymentInstrument;

                    if (currentBankCard.WalletCard != null)
                    {
                        // Parse the available credit from display string. Need to use Substring(1) to strip the currency code.
                        decimal availableCredit;
                        try
                        {
                            availableCredit = Decimal.Parse(currentBankCard.WalletCard.DisplayAvailableCredit, NumberStyles.Currency);
                            currentBankCard.availableCredit = availableCredit;
                        }
                        catch (FormatException)
                        {
                            availableCredit = 0.0M;
                        }

                        // Set the data context to current card.
                        DataContext = currentBankCard;

                        this.AmountDue.Text = (currentBankCard.creditLimit - currentBankCard.availableCredit).ToString("C");
                    }
                    else
                    {
                        this.AmountDue.Text = 0.ToString("C");
                    }

                    // If no payment is due, hide the button.
                    if (this.AmountDue.Text.Equals(0.ToString("C")))
                    {
                        this.PayBill.Visibility = System.Windows.Visibility.Collapsed;
                    }

                    System.Globalization.CultureInfo.DefaultThreadCurrentCulture = savedCultureInfo;
                }

            }
            else
            {
                MessageBox.Show(AppResources.CardLoadError);
                this.PayBill.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        /// <summary>
        /// When pay bill is click, we will fake a payment of the full amount due.
        /// </summary>
        private void PayBill_Click(object sender, RoutedEventArgs e)
        {
            if (currentBankCard.WalletCard != null)
            {
                MockWebService.WebService.PerformMockPaymentAsync(currentBankCard, this.AmountDue.Text);

                // In order to display the currency in the right format, we swap the current culture info the the culture info indicated
                // by the card. Once we are done setting currency related fields, we switch back.
                System.Globalization.CultureInfo savedCultureInfo = System.Globalization.CultureInfo.CurrentCulture;
                System.Globalization.CultureInfo.DefaultThreadCurrentCulture = new System.Globalization.CultureInfo(currentBankCard.CultureName);
                this.AmountDue.Text = 0.ToString("C");
                this.PayBill.Visibility = System.Windows.Visibility.Collapsed;
                System.Globalization.CultureInfo.DefaultThreadCurrentCulture = savedCultureInfo;
            }
        }
    }
}
