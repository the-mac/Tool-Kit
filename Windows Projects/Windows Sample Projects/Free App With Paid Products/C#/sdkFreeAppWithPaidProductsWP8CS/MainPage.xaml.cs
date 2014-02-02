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
using sdkFreeAppWithPaidProductsWP8CS.Resources;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Windows.ApplicationModel.Store;

namespace sdkFreeAppWithPaidProductsWP8CS
{
	public partial class MainPage : PhoneApplicationPage
	{
		#region fields
#if DEBUG
		// This field is used in debug configuration to help simulate purchase.
		private bool debugIsProductPurchased;
#endif // DEBUG

		// Each in-app product is identified by a string that is unique for the owning app. This value
		// is an example identifier for a product that causes the display of ads to be removed from the app.
		private const string PRODUCTID = "AdRemovalProductIdentifier";
		#endregion fields

		#region constructors
		public MainPage()
		{
			InitializeComponent();

			// In debug configuration, hide the informative message that applies to release configuration, and vice-versa.
#if DEBUG
			this.tbReleaseConfigurationMessage.Visibility = Visibility.Collapsed;
#else // DEBUG
			this.tbDebugConfigurationMessage.Visibility = Visibility.Collapsed;
#endif // DEBUG

			// Initialize the UI to show the initial state of the product licence.
			this.UpdateUIWithStateOfProductLicense();
		}
		#endregion constructors

		#region properties
		private bool IsProductPurchased
		{
			get
			{
#if DEBUG
				return this.debugIsProductPurchased;
#else // DEBUG
				// Be sure that the product id is present in the Windows Phone Store's dictionary of licenses,
				// and therefore that the ProductLicense instance is valid, before using the ProductLicense instance.
				ProductLicense productLicense;
				if (CurrentApp.LicenseInformation.ProductLicenses.TryGetValue(MainPage.PRODUCTID, out productLicense))
				{
					return productLicense.IsActive;
				}
				else
				{
					return false;
				}
#endif // DEBUG
			}
		}
		#endregion properties

		#region methods

		// Update the UI so that it reflects the current state of the in-app product's license
		// and, consequently, the user's right to use the in-app product.
		private void UpdateUIWithStateOfProductLicense()
		{
			bool isProductPurchased = this.IsProductPurchased;

			this.tbNotPurchasedMessage.Opacity = isProductPurchased ? 0 : 1;
			this.tbPurchasedMessage.Opacity = isProductPurchased ? 1 : 0;

			this.btnBuyProduct.IsEnabled = !isProductPurchased;
			this.btnUseProduct.IsEnabled = isProductPurchased;
		}
		#endregion methods

		#region event handlers
#if DEBUG
		private void Buy_Product_Click(object sender, EventArgs e)
		{
			this.debugIsProductPurchased = true;
			this.UpdateUIWithStateOfProductLicense();
		}
#else // DEBUG
		private async void Buy_Product_Click(object sender, EventArgs e)
		{
			try
			{
				// Kick off purchase; don't ask for a receipt when it returns.
				await CurrentApp.RequestProductPurchaseAsync(MainPage.PRODUCTID, false);
			}
			catch
			{
				// An exception is expected to be raised by CurrentApp.RequestProductPurchaseAsync() when it is not called from an app installed from the Windows Phone Store.
				// When the user does not complete the purchase (e.g. cancels or navigates back from the Purchase Page), an exception with an HRESULT of E_FAIL is expected.
			}
			this.UpdateUIWithStateOfProductLicense();
		}
#endif // DEBUG

		private void Use_Product_Click(object sender, EventArgs e)
		{
			MessageBox.Show(string.Format(AppResources.UseProductMessageBoxTextFormat, MainPage.PRODUCTID), AppResources.UseProductMessageBoxCaption, MessageBoxButton.OK);
		}
		#endregion event handlers
	}
}
