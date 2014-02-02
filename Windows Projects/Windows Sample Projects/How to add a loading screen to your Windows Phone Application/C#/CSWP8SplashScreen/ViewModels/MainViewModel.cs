/****************************** Module Header ******************************\
 * Module Name:  MainViewModel.cs
 * Project:      CSWP8SplashScreen
 * Copyright (c) Microsoft Corporation.
 * 
 * MainViewModel. 
 *
 * This source is subject to the Microsoft Public License.
 * See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
 * All other rights reserved.
 * 
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
 * EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/
using System;
using System.ComponentModel;
using System.Net.Http;

namespace SplashScreen.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        HttpClient _httpClient;
        public MainViewModel()
        {
            _dataLoaded = false;
            _status = "";
            _httpClient = new HttpClient();
            LoadViewModelData();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        private bool _dataLoaded;

        public bool DataLoaded
        {
            get { return _dataLoaded; }
            set { _dataLoaded = value; NotifyPropertyChanged("DataLoaded"); }
        }

        private string _status;

        public string Status
        {
            get { return _status; }
            set { _status = value; NotifyPropertyChanged("Status"); }
        }
        

        public async void LoadViewModelData()
        {
            DataLoaded = false;
            HttpResponseMessage response = await _httpClient.GetAsync("http://blogs.msdn.com/b/onecode/");
            Status = String.Format("Status: {0}", response.StatusCode);
            DataLoaded = true;
        }
        
    }
}
