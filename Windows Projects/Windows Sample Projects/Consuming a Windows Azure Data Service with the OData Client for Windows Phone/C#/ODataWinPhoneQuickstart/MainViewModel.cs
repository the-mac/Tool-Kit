// ----------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// ----------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data.Services.Client;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using ODataWinPhoneQuickstart.Netflix;

namespace ODataWinPhoneQuickstart
{
    public class MainViewModel : INotifyPropertyChanged
    {
        // Define the client paging parameters.
        private int _pageSize = 20;
        private int _currentPage = 0;
        private int _totalCount;

        // Define the message string.
        private string _message = string.Empty;

        // Defines the root URI of the data service.
        private static readonly Uri _rootUri = new Uri("http://odata.netflix.com/v1/Catalog/");

        // Define the typed DataServiceContext.
        private NetflixCatalog _context;

        // Define the binding collection for Titles.
        private DataServiceCollection<Title> _titles;

        // Gets and sets the collection of Title objects from the feed.
        public DataServiceCollection<Title> Titles
        {
            get { return _titles; }

            private set
            {
                // Set the Titles collection.
                _titles = value;

                // Register a handler for the LoadCompleted callback.
                _titles.LoadCompleted += OnTitlesLoaded;

                // Raise the PropertyChanged events.
                NotifyPropertyChanged("Titles");
            }
        }

        // Used to determine whether the data is loaded.
        public bool IsDataLoaded { get; private set; }

        // Loads data when the application is initialized.
        public void LoadData()
        {
            // Instantiate the context and binding collection.
            _context = new NetflixCatalog(_rootUri);

            // Use the public property setter to generate a notification to the binding.
            Titles = new DataServiceCollection<Title>(_context);

            // Load the data.
            Titles.LoadAsync(GetQuery());
        }

        // Displays data from the stored data context and binding collection 
        public void LoadData(NetflixCatalog context, DataServiceCollection<Title> _titles)
        {
            _context = context;
            Titles = _titles;

            IsDataLoaded = true;
        }

        // Loads data for a specific page of Titles.
        public void LoadData(int page)
        {
            _currentPage = page;

            // Reset the binding collection.
            Titles = new DataServiceCollection<Title>(_context);

            // Load the data.
            Titles.LoadAsync(GetQuery());
        }

        // Gets the pages loaded text displayed in the UI.
        public string PagesLoadedText
        {
            get
            {
                return string.Format("Page {0} of {1}", _currentPage + 1,
                    _totalCount / _pageSize);
            }
        }

        // Gets and sets the current page, which is needed for tombstoning.
        public int CurrentPage
        {
            get
            {
                return _currentPage;
            }
            set
            {
                // No need to report this change since we are not binding to it.
                _currentPage = value;
            }
        }

        // Gets and sets the total item count, which is needed for tombstoning.
        public int TotalCount
        {
            get
            {
                return _totalCount;
            }
            set
            {
                // No need to report this change since we are not binding to it.
                _totalCount = value;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnTitlesLoaded(object sender, LoadCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                // Make sure that we load all pages of the Customers feed.
                if (Titles.Continuation != null)
                {
                    Titles.LoadNextPartialSetAsync();
                }

                // Set the total page count, if we requested one.
                if (e.QueryOperationResponse.Query
                    .RequestUri.Query.Contains("$inlinecount=allpages"))
                {
                    _totalCount = (int)e.QueryOperationResponse.TotalCount;
                }

                IsDataLoaded = true;

                // Update the pages loaded text binding.
                NotifyPropertyChanged("PagesLoadedText");
            }
            else
            {
                // Display the error message in the binding.
                this.Message = e.Error.Message;
            }
        }

        // Private method that returns the page-specific query.
        private DataServiceQuery<Title> GetQuery()
        {
            // Get a query for the Titles feed from the context.
            DataServiceQuery<Title> query = _context.Titles;

            if (_currentPage == 0)
            {
                // If this is the first page, then also include a count of all titles.
                query = query.IncludeTotalCount();
            }

            // Add paging to the query.
            query = query.Skip(_currentPage * _pageSize)
                .Take(_pageSize) as DataServiceQuery<Title>;

            return query;
        }

        // Calls into the DataServiceContext to get the URI of the media resource.
        public Uri GetReadStreamUri(object entity)
        {
            // Return the URI for the supplied media link entry.
            return _context.GetReadStreamUri(entity);
        }

        // Notifies the binding about a changed property value.
        private void NotifyPropertyChanged(string propertyName)
        {
            var propertyChanged = PropertyChanged;
            if (propertyChanged != null)
            {
                // Raise the PropertyChanged event.
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        // Return a collection of key-value pairs to store in the application state.
        public List<KeyValuePair<string, object>> SaveState()
        {
            if (App.ViewModel.IsDataLoaded)
            {
                List<KeyValuePair<string, object>> stateList
                    = new List<KeyValuePair<string, object>>();

                // Create a new dictionary to store binding collections.
                var collections = new Dictionary<string, object>();

                // Add the current Titles binding collection.
                collections["Titles"] = App.ViewModel.Titles;

                // Store the current context and binding collections in the view model state.
                stateList.Add(new KeyValuePair<string, object>(
                    "DataServiceState", DataServiceState.Serialize(_context, collections)));
                stateList.Add(new KeyValuePair<string, object>("CurrentPage", CurrentPage));
                stateList.Add(new KeyValuePair<string, object>("TotalCount", TotalCount));

                return stateList;
            }
            else
            {
                return null;
            }
        }

        // Restores the view model state from the supplied state dictionary.
        public void RestoreState(IDictionary<string, object> dictionary)
        {
            // Create a dictionary to hold any stored binding collections.
            object titles;
            object stateAsString;

            if (dictionary.TryGetValue("DataServiceState", out stateAsString))
            {
                // Rehydrate the DataServiceState object from the serialization.
                DataServiceState state =
                    DataServiceState.Deserialize((string)stateAsString);

                if (state.RootCollections.TryGetValue("Titles", out titles))
                {
                    // Initialize the application with data from the DataServiceState.
                    App.ViewModel.LoadData((NetflixCatalog)state.Context,
                        (DataServiceCollection<Title>)titles);

                    // Restore other view model data.
                    _currentPage = (int)dictionary["CurrentPage"];
                    _totalCount = (int)dictionary["TotalCount"];
                }
            }
        }

        public string Message
        {
            get
            {
                return _message;
            }
            private set
            {
                _message = value;

                // Raise the PropertyChanged event.
                NotifyPropertyChanged("Message");
            }

        }
    }
}