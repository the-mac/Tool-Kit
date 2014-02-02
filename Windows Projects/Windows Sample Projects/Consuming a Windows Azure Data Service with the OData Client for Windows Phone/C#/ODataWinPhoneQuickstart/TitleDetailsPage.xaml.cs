// ----------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// ----------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using ODataWinPhoneQuickstart.Netflix;

namespace ODataWinPhoneQuickstart
{
    public partial class TitleDetailsPage : PhoneApplicationPage
    {
        Title currentTitle;

        public TitleDetailsPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string indexAsString = this.NavigationContext.QueryString["selectedIndex"];
            int index = int.Parse(indexAsString);
            this.DataContext = this.currentTitle 
                = (Title)App.ViewModel.Titles[index];
        }
    }
}