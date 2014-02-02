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
using ODataWinPhoneQuickstart.DataServiceContext.Netflix;

namespace ODataWinPhoneQuickstart
{
    public partial class TitleDetailsPage : PhoneApplicationPage
    {
        public TitleDetailsPage()
        {
            InitializeComponent();

            this.DataContext = App.ViewModel.SelectedTitle;
        }
    }
}