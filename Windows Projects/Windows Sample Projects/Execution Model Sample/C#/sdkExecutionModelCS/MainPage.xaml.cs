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
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

namespace sdkExecutionModelCS
{
    public partial class MainPage : PhoneApplicationPage
    {
        ViewModel _viewModel;
        bool _isNewPageInstance = false;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            _isNewPageInstance = true;

            // Set the event handler for when the application data object changes
            (Application.Current as sdkExecutionModelCS.App).ApplicationDataObjectChanged +=
                new EventHandler(MainPage_ApplicationDataObjectChanged);

        }


        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            // If this is a back navigation, the page will be discarded, so there
            // is no need to save state.
            if (e.NavigationMode != System.Windows.Navigation.NavigationMode.Back)
            {
                // Save the ViewModel variable in the page's State dictionary.
                State["ViewModel"] = _viewModel;
            }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            // If _isNewPageInstance is true, the page constuctor has been called, so
            // state may need to be restored
            if (_isNewPageInstance)
            {


                #region RestorePageState

                if (_viewModel == null)
                {
                    if (State.Count > 0)
                    {
                        _viewModel = (ViewModel)State["ViewModel"];
                    }
                    else
                    {
                        _viewModel = new ViewModel();
                    }
                }
                DataContext = _viewModel;

                #endregion


                #region RestoreApplicationState

                // if the application member variable is not empty,
                // set the page's data object from the application member variable.
                if ((Application.Current as sdkExecutionModelCS.App).ApplicationDataObject != null)
                {
                    UpdateApplicationDataUI();
                }
                else
                {

                    // Otherwise, call the method that loads data.
                    statusTextBlock.Text = "getting data...";
                    (Application.Current as sdkExecutionModelCS.App).GetDataAsync();
                }

                #endregion
            }

            // Set _isNewPageInstance to false. If the user navigates back to this page
            // and it has remained in memory, this value will continue to be false.
            _isNewPageInstance = false;
        }

        // The event handler called when the ApplicationDataObject changes.
        void MainPage_ApplicationDataObjectChanged(object sender, EventArgs e)
        {
            // Call UpdateApplicationData on the UI thread.
            Dispatcher.BeginInvoke(() => UpdateApplicationDataUI());
        }

        void UpdateApplicationDataUI()
        {
            // Set the ApplicationData and ApplicationDataStatus members of the ViewModel
            // class to update the UI.
            dataTextBlock.Text = (Application.Current as sdkExecutionModelCS.App).ApplicationDataObject;
            statusTextBlock.Text = (Application.Current as sdkExecutionModelCS.App).ApplicationDataStatus;
        }
    }
}
