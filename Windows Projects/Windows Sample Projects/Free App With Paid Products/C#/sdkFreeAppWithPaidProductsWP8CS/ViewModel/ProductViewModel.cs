/* 
    Copyright (c) 2012 - 2013 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://code.msdn.microsoft.com/wpapps
  
*/
namespace sdkFreeAppWithPaidProductsWP8CS
{
	using sdkFreeAppWithPaidProductsWP8CS.Resources;
	using System;
	using System.Threading.Tasks;
	using System.Windows.Media;
	using System.Windows.Media.Imaging;
	using Windows.ApplicationModel.Store;

	public class ProductViewModel : SdkHelper.PropertyChangedNotifier
	{
		#region fields
#if DEBUG
		// This field is used in debug configuration to help simulate purchase.
		private bool debugIsPurchased;
#endif // DEBUG
		#endregion fields

		#region properties

		// The ImageUri property contains the url pointing to the Tile image for the product in the Windows Phone Store.
		// The value will never change after instantiation, so there's no need to raise a property changed notification.
		internal Uri ImageUri { get; set; }

		/// <summary>
		/// The Image property contains the actual bitmap of the Tile image for the product in the Windows Phone Store. It
		/// is displayed by means of an Image element (in a data template in the UI) bound to it. Its value will never change
		/// after instantiation, so there's no need to raise a property changed notification.
		/// </summary>
		public ImageSource Image
		{
			get
			{
				BitmapImage bitmapImage = new BitmapImage(this.ImageUri);

				// Use the delay creation option so that initialization of the image is delayed until the
				// user has scrolled down to view it in the list.
				bitmapImage.CreateOptions = BitmapCreateOptions.DelayCreation;

				// The Image bound to this property is 125 logical pixels square.
				bitmapImage.DecodePixelType = DecodePixelType.Logical;
				bitmapImage.DecodePixelHeight = 125;
				return bitmapImage;
			}
		}

#if DEBUG
		// The IsPurchased property determines whether the product is purchased or not. In debug configuration, the debugIsPurchased
		// field is used to simulate purchase. Nothing is bound to this property, since it is internal, so there's no need to raise a
		// property changed notification on this property. But we do raise a changed notification on the IsPurchasedAsString property,
		// whose value is dependent on this one.
		internal bool IsPurchased
		{
			get
			{
				return this.debugIsPurchased;
			}
			// We only need a setter in debug configuration, to simulate purchase.
			set
			{
				this.debugIsPurchased = value;

		// The value of the IsPurchased property has changed (because we have changed the value of the debugIsPurchased field),
				// so raise a property changed notification on the IsPurchasedAsString property.
				this.RaisePropertyChanged("IsPurchasedAsString");
			}
		}
#else // DEBUG
		// The IsPurchased property determines whether the product is purchased or not. In release configuration, the Windows Phone Store
		// is queried.
		internal bool IsPurchased
		{
			get
			{
				// Be sure that the product id is present in the Windows Phone Store's dictionary of licenses,
				// and therefore that the ProductLicense instance is valid, before using the ProductLicense instance.
				ProductLicense productLicense;
				if (CurrentApp.LicenseInformation.ProductLicenses.TryGetValue(this.ProductId, out productLicense))
				{
					return productLicense.IsActive;
				}
				else
				{
					return false;
				}
			}
		}
#endif // DEBUG

		/// <summary>
		/// The IsPurchasedAsString property is displayed by means of a TextBlock (in a data template in the UI) bound to it.
		/// This property is implemented in terms of the IsPurchased property so whenever IsPurchased might have changed
		/// (after a purchase operation), a property changed notification will be raised on IsPurchasedAsString.
		/// </summary>
		public string IsPurchasedAsString
		{
			get
			{
				return (this.IsPurchased) ? AppResources.YouOwnThisItem : AppResources.TapToPurchase;
			}
		}

		/// <summary>
		/// The ProductId property stores the unique identifier for the in-app product.
		/// The value will never change after instantiation, so there's no need to raise a property changed notification.
		/// </summary>
		public string ProductId { get; internal set; }

		/// <summary>
		/// The Title property stores the title of the in-app product that was defined when the product was submitted to the Windows Phone Store.
		/// The value will never change after instantiation, so there's no need to raise a property changed notification.
		/// </summary>
		public string Title { get; internal set; }
		#endregion properties

		#region methods
		internal async void BuyAsync()
		{
			try
			{
				// Kick off purchase; don't ask for a receipt when it returns.
				await CurrentApp.RequestProductPurchaseAsync(this.ProductId, false);
			}
			catch
			{
				// An exception is expected to be raised by CurrentApp.RequestProductPurchaseAsync() when it is not called from an app installed from the Windows Phone Store.
				// When the user does not complete the purchase (e.g. cancels or navigates back from the Purchase Page), an exception with an HRESULT of E_FAIL is expected.
			}

			// The value of the IsPurchased property might have changed (after the purchase operation), so raise a property
			// changed notification on the IsPurchasedAsString property.
			this.RaisePropertyChanged("IsPurchasedAsString");
		}
		#endregion methods
	}
}
