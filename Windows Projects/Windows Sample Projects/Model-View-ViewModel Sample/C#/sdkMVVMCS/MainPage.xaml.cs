/* 
    Copyright (c) 2011 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
using System;
using System.Linq;
using System.Windows;
using Microsoft.Phone.Controls;
using sdkMVVMCS.ViewModelNS;

namespace sdkMVVMCS
{
    public partial class MainPage : PhoneApplicationPage
    {
        private ViewModel vm;

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            vm = new ViewModel();
        }


        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (!StateUtilities.IsLaunching && this.State.ContainsKey("Accomplishments"))
            {
                // Old instance of the application
                // The user started the application from the Back button.
    
                vm = (ViewModel)this.State["Accomplishments"];
                //MessageBox.Show("Got data from state");
            }
            else
            {
                // New instance of the application
                // The user started the application from the application list,
                // or there is no saved state available.
    
                vm.GetAccomplishments();
                //MessageBox.Show("Did not get data from state");
            }


            // There are two different views, but only one view model.
            // So, use LINQ queries to populate the views.

            // Set the data context for the Item view.
            ItemViewOnPage.DataContext = from Accomplishment in vm.Accomplishments where Accomplishment.Type == "Item" select Accomplishment;

            // Set the data context for the Level view.
            LevelViewOnPage.DataContext = from Accomplishment in vm.Accomplishments where Accomplishment.Type == "Level" select Accomplishment;

            // If there is only one view, you could use the following code
            // to populate the view.
            //AccomplishmentViewOnPage.DataContext = vm.Accomplishments;
        }


        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            if (this.State.ContainsKey("Accomplishments"))
            {
                this.State["Accomplishments"] = vm;
            }
            else
            {
                this.State.Add("Accomplishments", vm);
            }

            StateUtilities.IsLaunching = false;
        }


        private void AppBarSave_Click(object sender, EventArgs e)
        {
            vm.SaveAccomplishments();
        }
    }
}
