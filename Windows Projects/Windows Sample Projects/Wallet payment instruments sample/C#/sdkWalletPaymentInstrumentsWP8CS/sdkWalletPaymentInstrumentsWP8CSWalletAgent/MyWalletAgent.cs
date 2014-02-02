/* 
    Copyright (c) 2012 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
using System.Windows;
using Microsoft.Phone.Wallet;
using sdkWalletPaymentInstrumentsWP8CS;

namespace sdkWalletPaymentInstrumentsWP8CSWalletAgent
{
    /// <summary>
    /// This class defines the behavior of the background agent that is run to keep Wallet items fresh with up to date data.
    /// 
    /// In particular, the OnRefreshData method will be called whenever the Wallet is opened or the user looks at a card in
    /// the Wallet (as long as the card hasn't been updated within the last 30 minutes), or whever a user hits refresh (no
    /// matter how long it's been since the last card was last updated).
    /// </summary>
    public class MyWalletAgent : WalletAgent
    {
        private static volatile bool _classInitialized;

        /// <remarks>
        /// ScheduledAgent constructor, initializes the UnhandledException handler
        /// </remarks>
        public MyWalletAgent()
        {
            if (!_classInitialized)
            {
                _classInitialized = true;

                // Subscribe to the managed exception handler
                Deployment.Current.Dispatcher.BeginInvoke(delegate
                {
                    Application.Current.UnhandledException += ScheduledAgent_UnhandledException;
                });
            }
        }

        /// Code to execute on Unhandled Exceptions
        private void ScheduledAgent_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        /// <summary>
        /// The refresh agent is run whenever the user looks at an item in the wallet UI. The sorts of things you might want to do here include:
        /// 1) Update the transaction history for the item
        /// 2) Update the logo, contact information, and other metadata for the item
        /// 3) Update the status message to inform the user of required actions or present them with useful information.
        /// </summary>
        /// <param name="args">The args contain the list of wallet items that the wallet wants you to update. This way, if there are multiple services/items in the 
        /// wallet, you'll know which one the user is looking at, and hence which one needs to be updated.</param>
        protected override async void OnRefreshData(RefreshDataEventArgs args)
        {
            // Iterate through each wallet item that we're supposed to update. This will be the number of WalletTransactionItemBase derived objects in the Wallet linked to our app.
            foreach (WalletItem item in args.Items)
            {
                PaymentInstrument card = item as PaymentInstrument;
                if (card != null)
                {
                    // In this example, we're performing a fake transaction worth 5 dollars each time the Wallet Agent runs.
                    // In the wallet UI, you can force the referesh agent to run by opening an item, expanding the app menu (the "..." at the bottom right), and
                    // choosing "refresh".
                    MockWebService.WebService.UpdateWithLatestTransactions(card);

                    await card.SaveAsync();
                }
            }
            NotifyComplete();
        }
    }
}
