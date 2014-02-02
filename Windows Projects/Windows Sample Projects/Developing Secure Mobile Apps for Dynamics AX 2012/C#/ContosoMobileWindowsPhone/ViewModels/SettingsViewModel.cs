﻿using System.ComponentModel;

namespace ContosoMobile
{
    /// <summary>
    /// The view model for the Settings page
    /// </summary>
    public class SettingsViewModel : INotifyPropertyChanged
    {
        private SettingsModel settings;

        public SettingsViewModel()
        {
            settings = SettingsModel.GetSettings();
        }

        /// <summary>
        /// Property changed event handler declaration
        /// clients need to subscribe to this event
        /// to be notified of changes to property values
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Called by a property when the value is changed
        /// This will raise the Property Changed event
        /// </summary>
        /// <param name="propertyName">Property name</param>
        protected virtual void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public string UserName
        {
            get
            {
                return settings.UserName;
            }
            set
            {
                if (value != settings.UserName)
                {
                    settings.UserName = value.Replace('/', '\\'); // Fix any forward slashes
                    settings.SaveSettings();
                    RaisePropertyChanged("UserName");
                }
            }
        }

        public string Password
        {
            get
            {
                return settings.Password;
            }
            set
            {
                if (value != settings.Password)
                {
                    settings.Password = value;
                    settings.SaveSettings();
                    RaisePropertyChanged("Password");
                }
            }
        }

        /// <summary>
        /// This property is used to store the Windows Azure namespace
        /// </summary>
        public string AzureNamespace
        {
            get
            {
                return settings.AzureNamespace;
            }
            set
            {
                if (value != settings.AzureNamespace)
                {
                    settings.AzureNamespace = value;
                    settings.SaveSettings();
                    RaisePropertyChanged("AzureNamespace");
                }
            }
        }

        /// <summary>
        /// This read-only property is the actual endpoint exposed on ServiceBus that the final service call & security tokens will be sent to.
        /// </summary>
        public string ServiceEndpoint
        {
            get
            {
                return "https://" + AzureNamespace + ".servicebus.windows.net/Expense/";
            }
        }
      
        /// <summary>
        /// This is the actual ADFS endpoint we post our RST to in order to receive an RSTR with a SAML token in it
        /// </summary>
        public string StsEndpoint
        {
            get
            {
                return settings.StsEndpoint;
            }
            set
            {
                if (value != settings.StsEndpoint)
                {
                    settings.StsEndpoint = value;
                    settings.SaveSettings();
                    RaisePropertyChanged("StsEndpoint");
                }
            }
        }

        /// <summary>
        /// This read-only property returns whether all settings have been configured
        /// </summary>
        public bool IsFullyConfigured
        {
            get
            {
                return !string.IsNullOrWhiteSpace(UserName)
                    && !string.IsNullOrWhiteSpace(Password)
                    && !string.IsNullOrWhiteSpace(AzureNamespace)
                    && !string.IsNullOrWhiteSpace(StsEndpoint);
            }
        }
    }
}