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

namespace AppConnectExample.Model
{
    // Represents a parameter from a quick card in an App Connect deep link URI
    public class AppConnectUriParameter : INotifyPropertyChanged
    {
        // The parameter name
        private string _paramName;
        public string ParamName
        {
            get { return _paramName; }
            set
            {
                if (_paramName != value)
                {
                    _paramName = value;
                    NotifyPropertyChanged("ParamName");
                }
            }
        }

        // The parameter value
        private string _paramValue;
        public string ParamValue
        {
            get { return _paramValue; }
            set
            {
                if (_paramValue != value)
                {
                    _paramValue = value;
                    NotifyPropertyChanged("ParamValue");
                }
            }
        }

        // Class constructor
        public AppConnectUriParameter(string pName, string pValue)
        {
            _paramName = pName.Trim();

            if (_paramName == "Category")
            {
                // Place multiple categories on new lines.
                _paramValue = pValue.Replace(",", ",\n");
            }
            else
            {
                _paramValue = pValue;
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify that a property changed
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
