// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Windows;
using Microsoft.Phone.Controls;
using PhotoHubSample.ViewModels;

namespace PhotoHubSample
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
        }
        
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            var viewModel = new PhotosViewModel();
            DataContext = viewModel;

            PhotoHubLLS.ScrollTo(PhotoHubLLS.ItemsSource[PhotoHubLLS.ItemsSource.Count - 1]);
        }
    }
}