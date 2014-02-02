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
    /// Defines a debit card in our mock web service. Contains logic specifc to debit cards.
    /// </summary>
    public class BankDebitCard : BankCard
    {
        /// <summary>
        /// Basic constructor.
        /// </summary>
        /// <param name="id">A card id, so we can easily and repeatedly identify this card.</param>
        /// <param name="accountString">The account string associated with this card.</param>
        /// <param name="cardName">The name of this card, eg. "Woodgrove Debit".</param>
        public BankDebitCard(string id, string accountString, string cardName)
        {
            this.AccountString = accountString;
            this.CardName = cardName;
            this.id = id;
        }

        /// <summary>
        /// Translates to Balance in PaymentInstrument.
        /// </summary>
        public int balance { get; set; }
    }
}
