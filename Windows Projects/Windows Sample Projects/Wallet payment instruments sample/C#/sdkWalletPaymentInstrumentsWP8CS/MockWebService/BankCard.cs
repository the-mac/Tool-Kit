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
using System.ComponentModel;
using Microsoft.Phone.Wallet;


namespace sdkWalletPaymentInstrumentsWP8CS
{
    /// <summary>
    /// Base class defining bank cards in our mock Web Service. Contains code shared between
    /// credit and debit cards.
    /// </summary>
    public abstract class BankCard : INotifyPropertyChanged
    {
        private string walletStatus;
        private PaymentInstrument walletCard;

        /// <summary>
        /// Account number, translates to AccountNumber in a PaymentInstrument.
        /// </summary>
        public string AccountString { get; set; }

        /// <summary>
        /// Card Name, translates to DisplayName in a PaymentInstrument.
        /// </summary>
        public string CardName { get; set; }

        /// <summary>
        /// Account Holder, translates to Customer Name in PaymentInstrument.
        /// </summary>
        public string AccountHolder { get; set; }

        /// <summary>
        /// Expiration Date, translates to ExpiraionDate in PaymentInstrument.
        /// </summary>
        public DateTime ExpirationDate { get; set; }

        /// <summary>
        /// The culture name string for the card, used for localization.
        /// </summary>
        public string CultureName { get; set; }

        /// <summary>
        /// For ease of implementation in creating our demo application,
        /// our Mock Web Service domain objexts will be able to wrap
        /// a Wallet PaymentInstrument.
        /// </summary>
        public PaymentInstrument WalletCard
        {
            get
            {
                return walletCard;
            }
            set
            {
                if (value != walletCard)
                {
                    walletCard = value;
                    NotifyPropertyChanged("WalletCard");
                }
            }
        }

        /// <summary>
        /// Wallet status string
        /// </summary>
        public string WalletStatus
        {
            get
            {
                return walletStatus;
            }
            set
            {
                if (value != walletStatus)
                {
                    walletStatus = value;
                    NotifyPropertyChanged("WalletStatus");
                }
            }
        }

        /// <summary>
        /// Customer Name, translates to CustomerName in a wallettransactionitem.
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// Event handler fired when there is a property change
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }  
    }
}
