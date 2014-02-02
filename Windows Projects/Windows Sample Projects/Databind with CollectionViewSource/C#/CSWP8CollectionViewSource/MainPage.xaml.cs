/****************************** Module Header ******************************\
* Module Name:    MainPage.xaml.cs
* Project:        CSWP8CollectionViewSource
* Copyright (c) Microsoft Corporation
*
* This sample will demo how to databind with CollectionViewSource.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\*****************************************************************************/

using System;
using System.Linq;
using System.Windows;
using Microsoft.Phone.Controls;
using CSWP8CollectionViewSource.ViewModelNamespace;
using System.Windows.Data;
using CSWP8CollectionViewSource.Model;

namespace CSWP8CollectionViewSource
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

        /// <summary>
        /// Got data from state
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (!StateUtilities.IsLaunching && this.State.ContainsKey("ReadinessAndLevels"))
            {
                // Old instance of the application
                // The user started the application from the Back button.
                vm = (ViewModel)this.State["ReadinessAndLevels"];
                MessageBox.Show("Got data from state");
            }
            else
            {
                // New instance of the application
                // The user started the application from the application list,
                // or there is no saved state available.
                vm.GetReadinessAndLevels();
                //MessageBox.Show("Did not get data from state");
            }

            // There are two different views, but only one view model.
            // So, use LINQ queries to populate the views.

            // Set the data context for the Item view.
            ItemViewOnPage.DataContext = from ReadinessAndLevels in vm.readinessAndLevels
                                         where ReadinessAndLevels.Type == "Item"
                                         select ReadinessAndLevels;

            // [-or-] 

            //CollectionViewSource cvs = new CollectionViewSource();
            //cvs.Source = vm.ReadinessAndLevels;           
            //cvs.Filter += new FilterEventHandler(cvs_Filter);
            //ItemViewOnPage.DataContext = cvs;

            // Set the data context for the Level view.
            LevelViewOnPage.DataContext = from ReadinessAndLevels in vm.readinessAndLevels
                                          where ReadinessAndLevels.Type == "Level"
                                          select ReadinessAndLevels;       

            // If there is only one view, you could use the following code
            // to populate the view.
            ReadinessAndLevelsViewOnPage.DataContext = vm.readinessAndLevels;
        }

        /// <summary>
        /// Provides information and event data that is associated with the CollectionViewSource.Filter event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void cvs_Filter(object sender, FilterEventArgs e)
        {
            ReadinessAndLevel ral = e.Item as ReadinessAndLevel;
            if (ral != null)
            {
                e.Accepted = ral.Type == "Item";
            }
        }

        /// <summary>
        /// Save data from state. 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            if (this.State.ContainsKey("ReadinessAndLevels"))
            {
                this.State["ReadinessAndLevels"] = vm;
            }
            else
            {
                this.State.Add("ReadinessAndLevels", vm);
            }

            StateUtilities.IsLaunching = false;
        }

        /// <summary>
        /// Store the data. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AppBarSave_Click(object sender, EventArgs e)
        {
            vm.SaveReadinessAndLevels();
        }

        /// <summary>
        /// Add a new item to collection.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            vm.readinessAndLevels.Add(new Model.ReadinessAndLevel() { Name = "Windows 8", Type = "Item" });
        }

    }
}