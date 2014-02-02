/* 
    Copyright (c) 2012 - 2013 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://code.msdn.microsoft.com/wpapps
  
*/
using Microsoft.Phone.Controls;

namespace sdkScheduledNotificationsCS
{
    public partial class ShowParams : PhoneApplicationPage
    {
        public ShowParams()
        {
            InitializeComponent();
        }

        // Implement the OnNavigatedTo method and use NavigationContext.QueryString
        // to get the parameter values passed by the reminder.
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string param1Value = "";
            string param2Value = "";

            NavigationContext.QueryString.TryGetValue("param1", out param1Value);
            NavigationContext.QueryString.TryGetValue("param2", out param2Value);

            param1TextBlock.Text = param1Value;
            param2TextBlock.Text = param2Value;
        }


    }
}
