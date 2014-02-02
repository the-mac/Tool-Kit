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
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Windows.Storage;
using Windows.Storage.Streams;

namespace sdkPassingDataWP8CS
{
    public partial class MainPage : PhoneApplicationPage
    {
        PassingDataMethod method;

        const string METHOD_TEMPLATE = "Recipe: {0}\n\n{1}";
        const string TARGETPAGE_URI_TEMPLATE = "/TargetPage.xaml?method={0}";
        const string FILENAME = "saveddata.txt";

        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        // Select the recipe for passing the data according to the button tapped by the user.
        private async void RecipeButtonClick(object sender, RoutedEventArgs e)
        {
            string recipe = (sender as Button).Tag.ToString();
            switch (recipe)
            {
                case "Uri":
                    PassDataInPageUri();
                    break;
                case "Event":
                    PassDataInOnNavigatedFrom();
                    break;
                case "State":
                    PassDataInApplicationState();
                    break;
                case "Var":
                    PassDataInApplicationVariable();
                    break;
                case "File":
                    await PassDataInFileAsync();
                    break;
                default:
                    break;
            }
        }

        #region Recipe methods

        // Recipe 1. Pass data in the query string of the target Page Uri.
        private void PassDataInPageUri()
        {
            const string TARGET_DATA_TEMPLATE = "&targetData={0}";

            method = PassingDataMethod.QueryStringOfPageUri;

            // Prepare the data.
            string sourceData = string.Format(METHOD_TEMPLATE, method.ToString(), tbSourceData.Text);

            // Pass the data by adding it to the query string of the Uri.
            // Navigate to the target page.
            string targetPageUri =
                string.Format(TARGETPAGE_URI_TEMPLATE, method.ToString()) +
                string.Format(TARGET_DATA_TEMPLATE, sourceData);
            NavigationService.Navigate(new Uri(targetPageUri, UriKind.Relative));
        }

        // Recipe 2. Pass data in the OnNavigatedFrom event of the source Page.
        private void PassDataInOnNavigatedFrom()
        {
            method = PassingDataMethod.OnNavigatedFromEvent;

            // In this recipe, the data is not passed in this method,
            // but in the event handler for the OnNavigatedFrom method.

            // Navigate to the target page.
            string targetPageUri =
                string.Format(TARGETPAGE_URI_TEMPLATE, method.ToString());
            NavigationService.Navigate(new Uri(targetPageUri, UriKind.Relative));
        }

        // Recipe 3. Pass data in application-level state.
        private void PassDataInApplicationState()
        {
            method = PassingDataMethod.ApplicationState;

            // Prepare the data.
            string sourceData = string.Format(METHOD_TEMPLATE, method.ToString(), tbSourceData.Text);

            // Save the data to be passed in application-level state.
            PhoneApplicationService.Current.State["SourceData"] = sourceData;

            // Navigate to the target page.
            string targetPageUri =
                string.Format(TARGETPAGE_URI_TEMPLATE, method.ToString());
            NavigationService.Navigate(new Uri(targetPageUri, UriKind.Relative));
        }

        // Recipe 4. Pass data in an application-level variable
        // defined in App.xaml.cs.
        private void PassDataInApplicationVariable()
        {
            method = PassingDataMethod.ApplicationVariable;

            //Prepare the data.
            string sourceData = string.Format(METHOD_TEMPLATE, method.ToString(), tbSourceData.Text);

            // Set the value of the application-level variable
            // defined in App.xaml.cs.
            (Application.Current as App).sourceData = sourceData;

            // Navigate to the target page.
            string targetPageUri =
                string.Format(TARGETPAGE_URI_TEMPLATE, method.ToString());
            NavigationService.Navigate(new Uri(targetPageUri, UriKind.Relative));
        }

        // Recipe 5. Pass data in a file saved in the local folder.
        private async Task PassDataInFileAsync()
        {
            const string FILENAME_TEMPLATE = "&filename={0}";

            method = PassingDataMethod.File;

            //Prepare the data.
            string sourceData = string.Format(METHOD_TEMPLATE, method.ToString(), tbSourceData.Text);

            // Pass the data in a file saved in the local folder.
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFile dataFile =
                await localFolder.CreateFileAsync(FILENAME, CreationCollisionOption.ReplaceExisting);
            using (IRandomAccessStream dataStream = await dataFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                using (DataWriter textWriter = new DataWriter(dataStream))
                {
                    textWriter.WriteString(sourceData);
                    await textWriter.StoreAsync();
                }
            }

            // Pass the filename by adding it to the query string of the target Page Uri.
            // Navigate to the target page.
            string targetPageUri =
                string.Format(TARGETPAGE_URI_TEMPLATE, method.ToString()) +
                string.Format(FILENAME_TEMPLATE, FILENAME);
            NavigationService.Navigate(new Uri(targetPageUri, UriKind.Relative));
        }

        #endregion

        #region Page navigation event handlers

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Clean up and reset values when the user returns to the source page.

            // Reset the method that identifies which recipe was used.
            method = PassingDataMethod.Unspecified;

            // Reset the application-level state used in Recipe 3.
            if (PhoneApplicationService.Current.State.ContainsKey("SourceData"))
            {
                PhoneApplicationService.Current.State.Remove("SourceData");
            }

            // Reset the application-level variable used in Recipe 4.
            (Application.Current as App).sourceData = string.Empty;

            // Delete the file used in Recipe 5.
            if (File.Exists(FILENAME))
            {
                File.Delete(FILENAME);
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            //Prepare the data.
            string sourceData = string.Format(METHOD_TEMPLATE, method.ToString(), tbSourceData.Text);

            // Recipe 2. Pass data in the OnNavigatedFrom event of the source Page.
            if (method == PassingDataMethod.OnNavigatedFromEvent)
            {
                if (e.Content is TargetPage)
                {
                    (e.Content as TargetPage).ReceivedData = sourceData;
                }
            }
        }

        #endregion

    }
}
