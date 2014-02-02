/* 
    Copyright (c) 2012 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
namespace sdkWalletPaymentInstrumentsWP8CS
{
    /// <summary>
    /// Defines a credit card in out mock web service. Contains logic specific to credit cards.
    /// </summary>
    public class BankCreditCard : BankCard
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">A unique id identifying this instance</param>
        /// <param name="accountString">The account number for the card.</param>
        /// <param name="cardName">A descriptive name for the card, e.g. Contoso Credit.</param>
        public BankCreditCard(string id, string accountString, string cardName)
        {
            this.AccountString = accountString;
            this.CardName = cardName;
            this.id = id;
        }

        /// <summary>
        /// Translates to AvailableCredit in PaymentInstrument.
        /// </summary>
        public decimal availableCredit { get; set; }

        /// <summary>
        /// Translates to CreditLimit in PaymentInstrument.
        /// </summary>
        public int creditLimit { get; set; }

    }
}
