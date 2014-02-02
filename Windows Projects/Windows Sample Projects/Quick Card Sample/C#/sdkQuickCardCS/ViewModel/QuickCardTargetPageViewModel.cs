/* 
    Copyright (c) 2011 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Net;

// Reference the data model.
using AppConnectExample.Model;

namespace AppConnectExample.ViewModel
{
    public class QuickCardTargetPageViewModel : INotifyPropertyChanged
    {
        // Observeable collection for the App Connect deep link URI parameters.
        private ObservableCollection<AppConnectUriParameter> _AppConnectUriParameters;
        public ObservableCollection<AppConnectUriParameter> AppConnectUriParameters
        {
            get { return _AppConnectUriParameters; }
            set
            {
                if (_AppConnectUriParameters != value)
                {
                    _AppConnectUriParameters = value;
                    NotifyPropertyChanged("AppConnectUriParameters");
                }
            }
        }

        // Class constructor.
        public QuickCardTargetPageViewModel()
        {
            // Create observeable collection object.
            AppConnectUriParameters = new ObservableCollection<AppConnectUriParameter>();
        }

        // Load parameters from quick page; extract from the NavigationContext.QueryString
        public void LoadUriParameters(IDictionary<string, string> QueryString)
        {
            // Clear parameters in the ViewModel.
            AppConnectUriParameters.Clear();

            // Loop through the quick card parameters in the App Connect deep link URI.
            foreach (string strKey in QueryString.Keys)
            {
                // Set default value for parameter if no value is present.
                string strKeyValue = "<no value present in URI>";

                // Try to extract parameter value from URI.
                QueryString.TryGetValue(strKey, out strKeyValue);

                // Add parameter object to ViewModel collection.
                AppConnectUriParameters.Add(new AppConnectUriParameter(strKey, strKeyValue));
            }

        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify that a property has changed.
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
