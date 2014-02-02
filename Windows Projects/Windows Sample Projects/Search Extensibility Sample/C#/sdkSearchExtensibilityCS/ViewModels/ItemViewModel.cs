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
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace sdkSearchExtensibilityCS
{

    // ViewModel for the items page that is launched by Search Extras deep link URIs.
    public class ItemViewModel : INotifyPropertyChanged
    {

        // Parameters in the Search Extras deep link URI.
        private string _launchingParams;

        // Path to icon that designates if product is recalled.
        private ImageSource _iconSource;

        // Text that describe if product is recalled.
        private string _caption;

        // Property for Search Extras deep link URI parameters.
        public string LaunchingParams
        {
            get
            {
                return _launchingParams;
            }
            set
            {
                if (value != _launchingParams)
                {
                    _launchingParams = value;
                    NotifyPropertyChanged("LaunchingParams");
                }
            }
        }

        // Property for path to icon that designates if product is recalled.
        public ImageSource IconSource
        {
            get
            {
                return _iconSource;
            }
            set
            {
                if (value != _iconSource)
                {
                    _iconSource = value;
                    NotifyPropertyChanged("IconSource");
                }
            }
        }

        // Property of text that describes if product is recalled or not
        public string Caption
        {
            get
            {
                return _caption;
            }
            set
            {
                if (value != _caption)
                {
                    _caption = value;
                    NotifyPropertyChanged("Caption");
                }
            }
        }

        // Event handler for property changes
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
