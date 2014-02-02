/* 
    Copyright (c) 2012 - 2013 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://code.msdn.microsoft.com/wpapps
  
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Xml.Linq;
using Microsoft.Phone.Controls;
using Windows.ApplicationModel;
using Windows.Phone.Management.Deployment;

namespace CompanyHubExample
{
    public partial class MainPage : PhoneApplicationPage
    {
        WebClient client = new WebClient(); 

        public MainPage()
        {
            InitializeComponent();

            // Download the LOB app data from Azure.
            client.DownloadStringCompleted += HttpsCompleted;

            // TODO: The following changes are necessary to make this sample work:
            // 1) Create an Applications.xml file that follows the structure of the Applications.xml file in this sample project.
            //    For each company app that is available, this file provides download URLs for the XAP and icon, the version number, 
            //    the product ID, and a description.
            // 2) Upload the Applications.xml file, the company app XAPs, and app icons to a secure site that is accessible from 
            //    this company hub app (such as Azure storage).
            // 3) Update the example URL in the following line of code to point to the Applications.xml file.
            // 4) Prepare the company hub app and company apps for distribution and enroll users for distribution by following
            //    the instructions in http://msdn.microsoft.com/en-us/library/windowsphone/develop/jj206943(v=vs.105).aspx.

            //TODO: Update the following line of code with the URL that points to your Applications.xml file.
			//client.DownloadStringAsync(new Uri("http://contosostorage.blob.core.windows.net/companyapps/Applications.xml")); 
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Set the data context.
            if (DataContext == null)
                DataContext = App.ViewModel;
        }

        // Converts the LOB app data to a stream.
        private void HttpsCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null)
            {

                XDocument xdoc = XDocument.Parse(e.Result, LoadOptions.None);
                byte[] byteArray = Encoding.Unicode.GetBytes(e.Result);
                MemoryStream stream = new MemoryStream(byteArray);
                LoadAvailablePackages(stream);
            }
            else
            {
                MessageBox.Show("Unable to load xml: " + e.Error.ToString());
            }
        }

        // Loads the LOB app data into the ViewModel.
        protected void LoadAvailablePackages(MemoryStream stream)
        {
            try
            {
                var element = XElement.Load(stream);

                var apps = from var in element.Descendants("App")
                           orderby var.Attribute("Name").Value
                           select new CompanyAppViewModel(var.Attribute("Name").Value, 
                               var.Attribute("Description").Value, var.Attribute("IconUrl").Value, 
                               new Uri(var.Attribute("XapUrl").Value), Status.NotInstalled, 
                               Guid.Parse(var.Attribute("ProductId").Value));

                foreach (CompanyAppViewModel item in apps)
                {
                    App.ViewModel.CompanyApps.Add(item);
                }

                // Make note of each LOB app that is already installed.
                IEnumerable<Package> installedPackages = InstallationManager.FindPackagesForCurrentPublisher();
                foreach (Package package in installedPackages)
                {
                    Guid productId = Guid.Parse(package.Id.ProductId);
                    int index = this.FindProductIdIndex(productId);
                    if (index != -1)
                    {
                        App.ViewModel.CompanyApps[index].Status = Status.Installed;
                    }
                }

            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        // Gets the index of the LOB app with the specified product ID.
        private int FindProductIdIndex(Guid productId)
        {
            for (int index = 0; index < App.ViewModel.CompanyApps.Count; index++)
            {
                if (App.ViewModel.CompanyApps[index].ProductId == productId)
                {
                    return index;
                }
            }

            return -1;
        }

        // Performs one of the following tasks when a LOB app is tapped: installs the app, starts the app, or stops 
        // the installation of the app.
        private void CompanyAppList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LongListSelector companyAppList = (LongListSelector)sender;
            try
            {
                // If selected item is null (no selection) do nothing.
                if (companyAppList.SelectedItem == null)
                    return;

                CompanyAppViewModel selectedApp = companyAppList.SelectedItem as CompanyAppViewModel;

                // Clear the selection in the list.
                companyAppList.SelectedItem = null;

                // App is not installed yet, so start installing it.
                if (selectedApp.Status == Status.NotInstalled || selectedApp.Status == Status.Canceled)
                {
                    Windows.Foundation.IAsyncOperationWithProgress<PackageInstallResult, uint> result;
                    selectedApp.Status = Status.Installing;
                    selectedApp.ProgressValue = 0;
                    result = InstallationManager.AddPackageAsync(selectedApp.Title, selectedApp.XapPath);

                    result.Completed = InstallCompleted;
                    result.Progress = InstallProgress;
                    selectedApp.Result = result;
                }

                // App is already installed, so start it.
                else if (selectedApp.Status == Status.Installed)
                {
                    this.LaunchApp(selectedApp.ProductId);
                }

                // App is in the process of installing; determine if the user is trying to stop the installation.
                else if (selectedApp.Status == Status.Installing || selectedApp.Status == Status.InstallFailed)
                {
                    if (selectedApp.Result != null)
                    {
                        MessageBoxResult result = MessageBox.Show("Are you sure you want to stop the company app installation?", 
                            "Stop installation?", 
                            MessageBoxButton.OKCancel);

                        if (result == MessageBoxResult.OK)
                        {
                            selectedApp.Result.Cancel();
                            selectedApp.Status = Status.Canceled;
                        }
                    }
                    else
                    {
                        foreach (var installingPackage in InstallationManager.GetPendingPackageInstalls())
                        {
                            installingPackage.Cancel();
                        }

                        selectedApp.Status = Status.Canceled;
                    }
                }

            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
                companyAppList.SelectedItem = null;
            }
        }

        // Starts the LOB app with the specified product ID.
        private void LaunchApp(Guid productId)
        {
            try
            {
                bool found = false; 
                IEnumerable<Package> packages = InstallationManager.FindPackagesForCurrentPublisher();

                foreach (Package package in packages)
                {
                    if (Guid.Parse(package.Id.ProductId) == productId)
                    {
                        found = true;
                        package.Launch(string.Empty);
                        break;
                    }
                }

                if (found == false)
                {
                    MessageBox.Show("Could not find the app to launch.");
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        // Updates the installation progress bar for the app.
        private void InstallProgress(Windows.Foundation.IAsyncOperationWithProgress<PackageInstallResult, uint> installResult, 
            uint progressInfo)
        {
            try
            {
                this.Dispatcher.BeginInvoke(
                    () =>
                    {
                        int index = FindAsyncAppIndex(installResult.Id);

                        if (index != -1)
                        {
                            App.ViewModel.CompanyApps[index].ProgressValue = progressInfo;
                        }
                    });
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void InstallCompleted(Windows.Foundation.IAsyncOperationWithProgress<PackageInstallResult, uint> installResult, 
            Windows.Foundation.AsyncStatus asyncStatus)
        {
            try
            {
                this.Dispatcher.BeginInvoke(
                    () =>
                    {
                        int index = FindAsyncAppIndex(installResult.Id);

                        if (index != -1)
                        {
                            App.ViewModel.CompanyApps[index].ProgressValue = 100;

                            if (asyncStatus == Windows.Foundation.AsyncStatus.Completed)
                            {
                                App.ViewModel.CompanyApps[index].Status = Status.Installed;
                            }
                            else if (asyncStatus == Windows.Foundation.AsyncStatus.Canceled)
                            {
                                App.ViewModel.CompanyApps[index].Status = Status.Canceled;
                            }
                            else
                            {
                                App.ViewModel.CompanyApps[index].Status = Status.InstallFailed;
                            }
                        }
                    });
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private int FindAsyncAppIndex(uint asyncId)
        {
            for (int index = 0; index < App.ViewModel.CompanyApps.Count; index++)
            {
                if (App.ViewModel.CompanyApps[index].Result != null && App.ViewModel.CompanyApps[index].Result.Id == asyncId)
                {
                    return index;
                }
            }

            return -1;
        }
    }
}
