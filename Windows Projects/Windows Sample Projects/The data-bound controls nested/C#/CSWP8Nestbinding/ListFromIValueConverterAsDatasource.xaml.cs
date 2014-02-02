/****************************** Module Header ******************************\
* Module Name:    ListFromIValueConverterAsDatasource.xaml.cs
* Project:        CSWP8Nestbinding
* Copyright (c) Microsoft Corporation
*
* This sample will demo how to nest data-bound controls in WP.
 * This page uses list that get from converter as data source.
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
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Xml;
using System.ServiceModel.Syndication;
using System.Xml.Linq;

namespace CSWP8Nestbinding
{
    public partial class ListFromIValueConverterAsDatasource : PhoneApplicationPage
    {
        public ListFromIValueConverterAsDatasource()
        {
            InitializeComponent();
            Loaded += ListFromIValueConverterAsDatasource_Loaded;
        }

        void ListFromIValueConverterAsDatasource_Loaded(object sender, RoutedEventArgs e)
        {         
            // Load Data.
            XmlReader xmlReader = XmlReader.Create("Data.xml");
            SyndicationFeed feed = SyndicationFeed.Load(xmlReader);
          
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                outListBox.ItemsSource = feed.Items;
            });
        }        
    }
}