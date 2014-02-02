/* 
    Copyright (c) 2012 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Phone.Controls;

#if DEBUG
using MockIAPLib;
using Store = MockIAPLib;
#else
using Windows.ApplicationModel.Store;
using Store = Windows.ApplicationModel.Store;
#endif


namespace IAPMockLibSample
{
    public partial class StoreFront : PhoneApplicationPage
    {
        public ObservableCollection<PicItem> picItems = new ObservableCollection<PicItem>();

        public StoreFront()
        {
            InitializeComponent();
#if DEBUG
            clearStoreBtn.Visibility = System.Windows.Visibility.Visible;
#else
            clearStoreBtn.Visibility = System.Windows.Visibility.Collapsed;
#endif
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            RenderStoreItems();
            base.OnNavigatedTo(e);
        }

        private async void RenderStoreItems()
        {
            picItems.Clear();

            StoreManager mySM = new StoreManager();
            ListingInformation li = await Store.CurrentApp.LoadListingInformationAsync();

            foreach (string key in li.ProductListings.Keys)
            {
                ProductListing pListing = li.ProductListings[key];

                string status = Store.CurrentApp.LicenseInformation.ProductLicenses[key].IsActive ? "Purchased." : "Available to purchase.";

                string imageLink = string.Empty;

                if (mySM.StoreItems.TryGetValue(key, out imageLink))
                    picItems.Add(new PicItem { imgLink = imageLink, Status = status, key = key });

            }

            pics.ItemsSource = picItems;
        }

        private async void Image_Tap_1(object sender, GestureEventArgs e)
        {
            Image img = sender as Image;
            
            string key = img.Tag.ToString();

            if (!Store.CurrentApp.LicenseInformation.ProductLicenses[key].IsActive)
            {
                ListingInformation li = await Store.CurrentApp.LoadListingInformationAsync();
                string pID = li.ProductListings[key].ProductId;

                string receipt = await Store.CurrentApp.RequestProductPurchaseAsync(pID, false);

                RenderStoreItems();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
#if DEBUG
            if (MessageBoxResult.OK == MessageBox.Show("This will clear your store state, sure?", "Clear State", MessageBoxButton.OKCancel))
            {
                MockIAP.ClearCache();                

                MessageBox.Show("State cleared! Restart app please");
            }
#endif
        }

        private void returnToMain_Click_1(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/IAPMockLibSample;component/MainPage.xaml", UriKind.Relative));
        }
    }
}
