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
using System.Reflection;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Collections.Generic;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using Microsoft.Phone.Wallet;
using Microsoft.Phone.Shell;
using sdkWalletPaymentInstrumentsWP8CS.Resources;

namespace sdkWalletPaymentInstrumentsWP8CS
{
    public partial class CardPage : PhoneApplicationPage
    {
        BankCard currentBankCard;

        // Add wallet item task
        AddWalletItemTask addWalletItemTask = new AddWalletItemTask();

        /// <summary>
        /// Default constructor.
        /// </summary>
        public CardPage()
        {
            InitializeComponent();

            // Set completion event after wallet item task
            addWalletItemTask.Completed += new EventHandler<Microsoft.Phone.Tasks.AddWalletItemResult>(awic_Completed);
        }

        /// <summary>
        /// When navigated to set the data context to the current card.
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string passedID = "";
            if (NavigationContext.QueryString.TryGetValue("ID", out passedID))
            {
                // Set the current card and find the corresponding Wallet item.
                currentBankCard = MockWebService.WebService.Cards.First<BankCard>(item => item.id == passedID);
                if (currentBankCard != null)
                {
                    currentBankCard.WalletCard = Wallet.FindItem(currentBankCard.id) as PaymentInstrument;

                    // Set the data context to current card.
                    DataContext = currentBankCard;

                    // Set the expiration text
                    this.ExpirationDate.Text = currentBankCard.ExpirationDate.ToShortDateString();

                    // Check to see if current deal is in the Wallet and update UI accordingly.
                    if (currentBankCard.WalletCard != null)
                    {
                        ShowRemoveFromWallet();
                    }
                    else
                    {
                        ShowSaveToWallet();
                    }

                    // This app doesn't support payments for debit cards.
                    if (currentBankCard.GetType().Equals(typeof(BankDebitCard)))
                    {
                        this.ViewBill.Visibility = System.Windows.Visibility.Collapsed;
                    }
                }
            }
            else
            {
                MessageBox.Show(AppResources.CardLoadError);
            }
        }

        // Show remove current card UI
        private void ShowRemoveFromWallet()
        {
            this.RemoveFromWallet.Visibility = System.Windows.Visibility.Visible;
            this.SaveToWallet.Visibility = System.Windows.Visibility.Collapsed;
        }

        // Show save current card UI
        private void ShowSaveToWallet()
        {
            this.RemoveFromWallet.Visibility = System.Windows.Visibility.Collapsed;
            this.SaveToWallet.Visibility = System.Windows.Visibility.Visible;
        }

        // Click event for saving card to wallet.
        private void SaveToWallet_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Create a payment instrument object and set any mandatory fields, and any other fields the card should display.
                PaymentInstrument paymentInstrument = new PaymentInstrument(currentBankCard.id);
                paymentInstrument.AccountNumber = currentBankCard.AccountString;
                paymentInstrument.CustomerName = currentBankCard.AccountHolder;
                paymentInstrument.ExpirationDate = currentBankCard.ExpirationDate;
                paymentInstrument.DisplayName = currentBankCard.CardName;

                // Add the Culture Name as a custom property. Since we don't set a label for the custom property, it will
                // not show up in the UI, but we can use it later in this application when needed.
                CustomWalletProperty cultureCodeProperty = new CustomWalletProperty();
                cultureCodeProperty.Value = currentBankCard.CultureName;
                paymentInstrument.CustomProperties.Add("CultureName", cultureCodeProperty);

                // If we are creating a credit card, set credit card specific fields. If we are creating a debit card,
                // create debit card specific fields.
                if (currentBankCard.GetType().Equals(typeof(BankCreditCard)))
                {
                    // Adding some credit card related fields.
                    paymentInstrument.PaymentInstrumentKinds = PaymentInstrumentKinds.Credit;

                    BankCreditCard creditCard = currentBankCard as BankCreditCard;

                    // In order to display the currency in the right format, we swap the current culture info the the culture info indicated
                    // by the card. Once we are done setting currency related fields, we switch back.
                    System.Globalization.CultureInfo savedCultureInfo = System.Globalization.CultureInfo.CurrentCulture;
                    System.Globalization.CultureInfo.DefaultThreadCurrentCulture = new System.Globalization.CultureInfo(creditCard.CultureName);
                    paymentInstrument.DisplayCreditLimit = creditCard.creditLimit.ToString("C");
                    paymentInstrument.DisplayAvailableCredit = creditCard.availableCredit.ToString("C");
                    System.Globalization.CultureInfo.DefaultThreadCurrentCulture = savedCultureInfo;

                    // Set a status message and corresponding deep link.
                    paymentInstrument.Message = "Your bill is ready!";
                    paymentInstrument.MessageNavigationUri = new Uri("/BillPayPage.xaml?ID=" + currentBankCard.id, UriKind.Relative);
                }
                else if (currentBankCard.GetType().Equals(typeof(BankDebitCard)))
                {
                    paymentInstrument.PaymentInstrumentKinds = PaymentInstrumentKinds.Debit;

                    BankDebitCard debitCard = currentBankCard as BankDebitCard;

                    // In order to display the currency in the right format, we swap the current culture info the the culture info indicated
                    // by the card. Once we are done setting currency related fields, we switch back.
                    System.Globalization.CultureInfo savedCultureInfo = System.Globalization.CultureInfo.CurrentCulture;
                    System.Globalization.CultureInfo.DefaultThreadCurrentCulture = new System.Globalization.CultureInfo(debitCard.CultureName);
                    paymentInstrument.DisplayBalance = debitCard.balance.ToString("C");
                    System.Globalization.CultureInfo.DefaultThreadCurrentCulture = savedCultureInfo;
                    
                }

                // Add a logo to our card to make it look nice.
                BitmapImage bmp = new BitmapImage();
                Uri logoUri = new Uri("Assets/contoso_logo_large.png", UriKind.Relative);
                bmp.SetSource(Application.GetResourceStream(logoUri).Stream);
                paymentInstrument.Logo99x99 = bmp;
                paymentInstrument.Logo159x159 = bmp;
                paymentInstrument.Logo336x336 = bmp;

                // Add a deep link so that when our application is launch from the Wallet item, we show that card's page instead of just
                // opening the app to the main page.
                paymentInstrument.NavigationUri = new Uri("/CardPage.xaml?ID=" + currentBankCard.id, UriKind.Relative);

                // Add the new payment instrument to the taskl.
                addWalletItemTask.Item = paymentInstrument;

                SystemTray.ProgressIndicator.IsIndeterminate = true;

                // Run the task.
                addWalletItemTask.Show();

            }
            catch (Exception ex)
            {
                MessageBox.Show(AppResources.CardSaveError + " " + ex.Message);
            }
            finally
            {
                // UI Cleanup: reflect saved state in the UI.
                SystemTray.ProgressIndicator.IsIndeterminate = false;
            }
        }

        // Click event for removing the current card from the wallet.
        private void RemoveFromWallet_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (currentBankCard != null)
                {
                    Wallet.Remove(currentBankCard.id);
                    ShowSaveToWallet();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(AppResources.CardRemoveError + " " + ex.Message);
            }
        }

        /// <summary>
        /// Add wallet item task completion function.
        /// </summary>
        void awic_Completed(object sender, Microsoft.Phone.Tasks.AddWalletItemResult e)
        {
            if (e.TaskResult == Microsoft.Phone.Tasks.TaskResult.OK)
            {
                MessageBox.Show(e.Item.DisplayName + " was added!");
            }
            else if (e.TaskResult == Microsoft.Phone.Tasks.TaskResult.Cancel)
            {
                MessageBox.Show("Cancelled");
            }
            else if (e.TaskResult == Microsoft.Phone.Tasks.TaskResult.None)
            {
                MessageBox.Show("None");
            }

            ShowRemoveFromWallet();
        }

        // Navigate to the bill pay page for this card.
        private void ViewBill_Click(object sender, RoutedEventArgs e)
        {
            if (currentBankCard != null)
            {
                NavigationService.Navigate(new Uri("/BillPayPage.xaml?ID=" + currentBankCard.id, UriKind.Relative));
            }
        }
    }
}
