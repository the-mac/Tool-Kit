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
using System.Collections.ObjectModel;
using Microsoft.Phone.Wallet;

namespace sdkWalletPaymentInstrumentsWP8CS
{
    /// <summary>
    /// This class represents a web service similar to that which an application creater might have on the back end.
    /// In this demo application, this class exists in order to better mimic real life code flow in other parts
    /// of the application.
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
            //Initialize deals collection
            this.Cards = new ObservableCollection<BankCard>();

            //Initialize with loaded deals
            LoadData();
        }

        /// <summary>
        /// Observable collection of BankCards for data binding.
        /// </summary>
        public Collection<BankCard> Cards { get; private set; }

        /// <summary>
        /// Creates and adds some mock cards to be pulled from the web service in future.
        /// </summary>
        public void LoadData()
        {
            // Clear out old data, if any exists.
            if (Cards.Count == 0)
            {
                // Add a few mock cards.
                this.Cards.Add(new BankCreditCard("1", "XXXX-XXXX-XXXX-1234", "Contoso Credit") { AccountHolder = "Joe Healy", creditLimit = 1000, availableCredit = 1000, ExpirationDate = new DateTime(2014, 1, 1), CultureName = "en-US" });
                this.Cards.Add(new BankDebitCard("2", "XXXX-XXXX-XXXX-9876", "Contoso Debit") { AccountHolder = "Joe Healy", balance = 1000, ExpirationDate = new DateTime(2014, 1, 1), CultureName = "en-US" });
            }
        }

        /// <summary>
        /// Creates and adds some mock cards to be pulled from the web service in future.
        /// </summary>
        public void RefreshData()
        {
            // Clear out any older cards, if they exist.
            if (Cards.Count != 0)
            {
                this.Cards.Clear();
            }

            // Add a few mock cards.
            this.Cards.Add(new BankCreditCard("1", "XXXX-XXXX-XXXX-1234", "Contoso Credit") { AccountHolder = "Joe Healy", creditLimit = 1000, availableCredit = 1000, ExpirationDate = new DateTime(2014, 1, 1), CultureName = "en-US" });
            this.Cards.Add(new BankDebitCard("2", "XXXX-XXXX-XXXX-9876", "Contoso Debit") { AccountHolder = "Joe Healy", balance = 1000, ExpirationDate = new DateTime(2014, 1, 1), CultureName = "en-US" });
        }

        /// <summary>
        /// Updates card with a fake transaction. For this sample application, if the balance of the card is less than 5, we won't add a
        /// transaction to avoid going into negative balance.
        /// </summary>
        /// <param name="card">The card to perform the transaction on. The display balance is currently formatted as "$1000"</param>
        public void UpdateWithLatestTransactions(PaymentInstrument card)
        {
            decimal newBalance;

            CultureInfo savedCultureInfo = CultureInfo.CurrentCulture;
            CustomWalletProperty cultureCodeProperty = new CustomWalletProperty();
            if (card.CustomProperties.TryGetValue("CultureName", out cultureCodeProperty))
            {
                CultureInfo.DefaultThreadCurrentCulture = new CultureInfo(cultureCodeProperty.Value);
            }

            WalletTransaction transaction = new WalletTransaction();
            transaction.Description = "Fake transaction";
            transaction.DisplayAmount = "-" + 5.ToString("C");
            transaction.TransactionDate = DateTime.Now;
            
            // Preform specific logic based on type of card (Credit or Debit) we are preforming
            // the transaction on.
            if (card.PaymentInstrumentKinds.Equals(PaymentInstrumentKinds.Credit))
            {
                try
                {
                    newBalance = Decimal.Parse(card.DisplayAvailableCredit, NumberStyles.Currency);
                    if (newBalance > 5)
                    {
                        newBalance -= 5;
                        card.DisplayAvailableCredit = newBalance.ToString("C");

                        card.TransactionHistory.Add("MockTransaction_" + DateTime.Now, transaction);
                    }
                }
                catch (FormatException)
                {
                    // Do nothing. If we can't parse correctly, we will just leave the balance as is.
                }
            }
            else
            {
                try
                {
                    newBalance = Decimal.Parse(card.DisplayBalance, NumberStyles.Currency);
                    if (newBalance > 5)
                    {
                        newBalance -= 5;
                        // In order to display the currency in the right format, we swap the current culture info the the culture info indicated
                        // by the card. Once we are done setting currency related fields, we switch back.
                        card.DisplayBalance = newBalance.ToString("C");

                        card.TransactionHistory.Add("MockTransaction_" + DateTime.Now, transaction);
                    }
                }
                catch (FormatException)
                {
                    // Do nothing. If we can't parse correctly, we will just leave the balance as is.
                }
            }
            System.Globalization.CultureInfo.DefaultThreadCurrentCulture = savedCultureInfo;
        }

        /// <summary>
        /// Perform a mock payment, which will reset the available credit to the credit limit.
        /// Only applicable to credit cards.
        /// </summary>
        /// <param name="card">The credit card to perform the payment on.</param>
        /// <param name="paymentAmount">A string representing how much the payment was for, for addition to the transaction history.</param>
        public async void PerformMockPaymentAsync(BankCreditCard card, string paymentAmount)
        {
            // In order to display the currency in the right format, we swap the current culture info the the culture info indicated
            // by the card. Once we are done setting currency related fields, we switch back.
            System.Globalization.CultureInfo savedCultureInfo = System.Globalization.CultureInfo.CurrentCulture;
            System.Globalization.CultureInfo.DefaultThreadCurrentCulture = new System.Globalization.CultureInfo(card.CultureName);

            card.WalletCard.DisplayAvailableCredit = card.creditLimit.ToString("C");

            WalletTransaction transaction = new WalletTransaction();
            transaction.Description = "Payment";
            transaction.DisplayAmount = "+" + paymentAmount;
            transaction.TransactionDate = DateTime.Now;
            card.WalletCard.TransactionHistory.Add("MockTransaction_" + DateTime.Now, transaction);

            System.Globalization.CultureInfo.DefaultThreadCurrentCulture = savedCultureInfo;

            card.WalletCard.DisplayAvailableCredit = card.creditLimit.ToString("C");

            await card.WalletCard.SaveAsync();
        }

    }
}
