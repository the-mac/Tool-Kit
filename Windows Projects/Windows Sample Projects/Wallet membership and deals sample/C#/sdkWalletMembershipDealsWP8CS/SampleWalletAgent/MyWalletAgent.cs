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
using System.Windows;
using Microsoft.Phone.Wallet;
using sdkWalletMembershipDealsWP8CS;

namespace sdkWalletMembershipDealsWP8CS
{
    public class MyWalletAgent : WalletAgent
    {
        private static volatile bool _classInitialized;

        /// <remarks>
        /// ScheduledAgent constructor that we are using for this application's wallet agent, initializes the UnhandledException handler
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

        // Code to execute on Unhandled Exceptions
        private void ScheduledAgent_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        /// <summary>
        /// The refresh agent is run when a user taps refresh on an item in the wallet, or when it has been viewed enough for the agent to deem it refreshable. 
        /// The sorts of things you might want to do here include:
        /// 1) Update the transaction history for the item
        /// 2) Update the logo, contact information, and other metadata for the item
        /// 3) Update the status message to inform the user of required actions or present them with useful information.
        /// </summary>
        /// <param name="args">The args contain the list of wallet items that are currently being refreshed. This way, if there are multiple services/items in the 
        /// wallet, you'll know which ones the user is looking at, and hence which ones need to be updated.</param>
        protected override async void OnRefreshData(RefreshDataEventArgs args)
        {
            // Iterate through each wallet item that requires a refresh.
            foreach (WalletItem item in args.Items)
            {
                WalletTransactionItem card = item as WalletTransactionItem;
                if (card != null)
                {
                    // In this example, we're performing a fake transaction worth 5 points each time the Wallet Agent runs. We are also displaying 
                    // 1 random deal from the mock web services deals as the status message with a deep link to the deal in the application itself.
                    // In the wallet UI, you can force the referesh agent to run by opening an item, expanding the application menu (the "..." at the bottom right), 
                    // and choosing "refresh".
                   
                    int newBalance = MockWebService.WebService.PerformMockTransaction(card);
                    card.DisplayAvailableBalance= newBalance + " points";


                    Coupon deal = MockWebService.WebService.GetRelevantDealForUser();
                    card.Message = "You might be interested in " + deal.Description + " Click here to check it out!";
                    card.MessageNavigationUri = new Uri("/CouponView.xaml?ID=" + deal.ID, UriKind.Relative);

                    await card.SaveAsync();
                }
            }
            NotifyComplete();
        }
    }
}
