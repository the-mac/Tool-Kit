// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using Microsoft.Devices;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using TwitterSample.Model;
using TwitterSample.ViewModels;

namespace TwitterSample
{
    public partial class MainPage : PhoneApplicationPage
    {
        int _pageNumber = 1;
        string _searchTerm = "";
        TwitterViewModel _viewModel;        
        int _offsetKnob = 7;

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            _viewModel = (TwitterViewModel)Resources["viewModel"];
            resultListBox.ItemRealized += resultListBox_ItemRealized;
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            var progressIndicator = SystemTray.ProgressIndicator;
            if (progressIndicator != null)
            {
                return;
            }

            progressIndicator = new ProgressIndicator();

            SystemTray.SetProgressIndicator(this, progressIndicator);

            Binding binding = new Binding("IsLoading") { Source = _viewModel };
            BindingOperations.SetBinding(
                progressIndicator, ProgressIndicator.IsVisibleProperty, binding);

            binding = new Binding("IsLoading") { Source = _viewModel };
            BindingOperations.SetBinding(
                progressIndicator, ProgressIndicator.IsIndeterminateProperty, binding);

            progressIndicator.Text = "Loading new tweets...";

        }

        void resultListBox_ItemRealized(object sender, ItemRealizationEventArgs e)
        {
            if (!_viewModel.IsLoading && resultListBox.ItemsSource != null && resultListBox.ItemsSource.Count >= _offsetKnob)
            {
                if (e.ItemKind == LongListSelectorItemKind.Item)
                {
                    if ((e.Container.Content as TwitterSearchResult).Equals(resultListBox.ItemsSource[resultListBox.ItemsSource.Count - _offsetKnob]))
                    {
                        Debug.WriteLine("Searching for {0}", _pageNumber);
                        _viewModel.LoadPage(_searchTerm, _pageNumber++);
                    }
                }
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                _searchTerm = SearchTextBox.Text.Trim();

                if (String.IsNullOrEmpty(_searchTerm))
                {
                    VibrateController.Default.Start(TimeSpan.FromMilliseconds(200));
                    return;
                }

                this.Focus();

                _pageNumber = 1;

                _viewModel.LoadPage(_searchTerm, _pageNumber++);
            }
        }

        private void OnGotFocus(object sender, RoutedEventArgs e)
        {
            SearchTextBox.SelectAll();
        }        
    }
}