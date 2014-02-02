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
	using System;
	using System.Collections.ObjectModel;
	using Windows.ApplicationModel.Store;

	public class ProductListViewModel : SdkHelper.PropertyChangedNotifier
	{
		#region fields
		// The selectedProduct field supports the SelectedProduct property.
		private ProductViewModel selectedProduct;
		#endregion fields

		#region properties
		/// <summary>
		/// The SelectedProduct property is two-way bound to the SelectedItem property of the ListBox in the UI.
		/// Whenever the user taps a product item in the ListBox, this property's setter is called, and the
		/// purchase process is initiated.
		/// </summary>
		public ProductViewModel SelectedProduct
		{
			get
			{
				return this.selectedProduct;
			}
			set
			{
				// Call a helper method on the PropertyChangedNotifier that updates the value
				// (if it has changed) and raises a property changed notification.
				this.SetPropertyAndRaisePropertyChanged(ref this.selectedProduct, value);
				if (this.SelectedProduct != null)
				{
#if DEBUG
					this.SelectedProduct.IsPurchased = true;
#else // DEBUG
					this.SelectedProduct.BuyAsync();
					// Set the ListBox's selection back to null. This is done so that the same product item
					// may be tapped again.
					this.SetPropertyAndRaisePropertyChanged(ref this.selectedProduct, null);
#endif // DEBUG
				}
			}
		}

		/// <summary>
		/// The Products property contains the listing of products in the Windows Phone Store.
		/// </summary>
		public ObservableCollection<ProductViewModel> Products { get; private set; }
		#endregion properties

		#region constructors
		public ProductListViewModel()
		{
			this.Products = new ObservableCollection<ProductViewModel>();
#if DEBUG
			// Simulate some product listings in debug configuration.
			this.Products.Add(new ProductViewModel() { Title = "Title1Debug", ProductId = "ProductId1Debug", ImageUri = new Uri("/Assets/one.png", UriKind.Relative) });
			this.Products.Add(new ProductViewModel() { Title = "Title2Debug", ProductId = "ProductId2Debug", ImageUri = new Uri("/Assets/two.png", UriKind.Relative) });
			this.Products.Add(new ProductViewModel() { Title = "Title3Debug", ProductId = "ProductId3Debug", ImageUri = new Uri("/Assets/three.png", UriKind.Relative) });
			this.Products.Add(new ProductViewModel() { Title = "Title4Debug", ProductId = "ProductId4Debug", ImageUri = new Uri("/Assets/four.png", UriKind.Relative) });
#else // DEBUG
			this.LoadListingInformationAsync();
#endif // DEBUG
		}
		#endregion constructors

		#region methods
		private async void LoadListingInformationAsync()
		{
			try
			{
				// Query the Windows Phone Store for the in-app products defined for the currently-running app.
				ListingInformation listingInformation = await CurrentApp.LoadListingInformationAsync();
				foreach (var v in listingInformation.ProductListings)
				{
					this.Products.Add(new ProductViewModel() { Title = v.Value.Name, ProductId = v.Value.ProductId, ImageUri = v.Value.ImageUri });
				}
			}
			catch
			{
				// An exception is expected to be raised by CurrentApp.LoadListingInformationAsync() when it is not called from an app installed from the Windows Phone Store.
			}
		}
		#endregion methods
	}
}
