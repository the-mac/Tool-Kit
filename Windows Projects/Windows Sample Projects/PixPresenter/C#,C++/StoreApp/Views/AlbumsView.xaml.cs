/* 
    Copyright (c) 2012 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Store apps, visit http://code.msdn.microsoft.com/windowsapps. 
  
*/
using PixPresenterPortableLib;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace PixPresenter.Views
{
  public sealed partial class AlbumsView : ViewBase
  {
    public AlbumsView()
    {
        this.InitializeComponent();
    }

    bool loaded = false;

    protected async override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        if (loaded)
        return;

        loaded = true;

        // Load albums into the view model
        await App.AlbumsViewModel.LoadAlbumsAsync();
        this.DataContext = App.AlbumsViewModel;
      
    }

    // View a specific album
    void AlbumsGridView_ItemClick(object sender, ItemClickEventArgs e)
    {
         var album = (AlbumViewModel)e.ClickedItem;
         this.Frame.Navigate(typeof(AlbumView), album.Name);
    }

  }
}
