using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;

namespace NTLMSampleCode
{
    public partial class CustomLoginPage : PhoneApplicationPage
    {
        public CustomLoginPage()
        {
            InitializeComponent();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            bool? isChecked = rememberme.IsChecked;

            if (isChecked != null && (bool)isChecked)
            {
                if(!string.IsNullOrEmpty(UserName.Text))
                    IsolatedStorageSettings.ApplicationSettings["UserName"] = UserName.Text;

                //Should encrypt password before storage in a real world application using DPAPIs
                if(!string.IsNullOrEmpty(Password.Password))
                    IsolatedStorageSettings.ApplicationSettings["Password"] = Password.Password;
            }

            NavigationService.GoBack();
        }
    }
}