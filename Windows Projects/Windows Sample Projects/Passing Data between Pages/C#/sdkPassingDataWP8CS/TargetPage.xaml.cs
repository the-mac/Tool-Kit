/* 
    Copyright (c) 2012 - 2013 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://code.msdn.microsoft.com/wpapps
  
*/
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using Windows.Storage;
using Windows.Storage.Streams;

namespace sdkPassingDataWP8CS
{
    public partial class TargetPage : PhoneApplicationPage
    {
        PassingDataMethod method;

        public TargetPage()
        {
            InitializeComponent();
        }

        // For use in Recipe 2. Receive data in the OnNavigatedFrom event of the source Page.
        public string ReceivedData { get; set; }

        #region Recipe methods

        // Recipe 1. Receive data in the query string of the target Page Uri.
        private void ReceiveDataInPageUri()
        {
            // Read the source data from the query string of the target Page Uri.
            string receivedData;
            if (NavigationContext.QueryString.TryGetValue("targetData", out receivedData))
            {
                tbTargetData.Text = receivedData;
            }
        }

        // Recipe 2. Receive data in the OnNavigatedFrom event of the source Page.
        private void ReceiveDataInOnNavigatedFrom()
        {
            // Read the property value that was set
            // in the OnNavigatedFrom event of the source Page.
            tbTargetData.Text = this.ReceivedData;
        }

        // Recipe 3. Receive data in application-level state.
        private void ReceiveDataInApplicationState()
        {
            // Read the source data from application-level state.
            object receivedData;
            if (PhoneApplicationService.Current.State.TryGetValue("SourceData", out receivedData))
            {
                tbTargetData.Text = receivedData.ToString();
            }
        }

        // Recipe 4. Receive data in an application-level variable
        // defined in App.xaml.cs.
        private void ReceiveDataInApplicationVariable()
        {
            // Read the contents of the application-level variable.
            tbTargetData.Text = (Application.Current as App).sourceData;
        }

        // Recipe 5. Receive data in a file saved in the local folder.
        private async Task ReceiveDataInFileAsync()
        {
            // Read the contents of the file saved in the local folder.
            string filename;
            if (NavigationContext.QueryString.TryGetValue("filename", out filename))
            {
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                StorageFile dataFile = await localFolder.GetFileAsync(filename);
                using (IRandomAccessStream dataStream = await dataFile.OpenReadAsync())
                {
                    using (DataReader textReader = new DataReader(dataStream))
                    {
                        uint textLength = (uint)dataStream.Size;
                        await textReader.LoadAsync(textLength);
                        tbTargetData.Text = textReader.ReadString(textLength);
                    }
                }

            }

        }

        #endregion

        #region Page navigation event handlers

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Select the recipe for reading the data according to the method used to pass the data.
            string methodDescription;
            if (NavigationContext.QueryString.TryGetValue("method", out methodDescription))
            {
                method = (PassingDataMethod)Enum.Parse(typeof(PassingDataMethod), methodDescription);

                switch (method)
                {
                    case PassingDataMethod.QueryStringOfPageUri:
                        ReceiveDataInPageUri();
                        break;
                    case PassingDataMethod.OnNavigatedFromEvent:
                        ReceiveDataInOnNavigatedFrom();
                        break;
                    case PassingDataMethod.ApplicationState:
                        ReceiveDataInApplicationState();
                        break;
                    case PassingDataMethod.ApplicationVariable:
                        ReceiveDataInApplicationVariable();
                        break;
                    case PassingDataMethod.File:
                        await ReceiveDataInFileAsync();
                        break;
                    default:
                        break;
                }
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            // Reset values when the user leaves this target page.
            this.ReceivedData = string.Empty;
            tbTargetData.Text = string.Empty;
        }

        #endregion

    }
}
