// ----------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// ----------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Services.Client;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using MVVMODataTestApp.Northwind;

namespace MVVMODataTestApp
{
    public partial class MainViewModel : INotifyPropertyChanged
    {               
        // Defines the root URI of the data service.
        // To make updates and save changes, use this URI for the
        // read/write Northwind service implementation included in the solution.
        private static readonly Uri _rootUri =
            new Uri("http://localhost:12345/Northwind.svc");

        //// This is the URI of the public, read-only Northwind data service.  
        //// Changes cannot be saved to the read-only service.
        //private static readonly Uri _rootUri =
        //    new Uri("http://services.odata.org/Northwind/Northwind.svc/");

        // Define the typed DataServiceContext.
        private NorthwindEntities _context;

        // Define the binding collection for Customers.
        private DataServiceCollection<Customer> _customers;

        // Gets and sets the collection of Customer objects from the feed.
        // This collection is used to bind to the UI (View).
        public DataServiceCollection<Customer> Customers
        {
            get { return _customers; }

            private set
            {
                // Set the Titles collection.
                _customers = value;

                // Register a handler for the LoadCompleted callback.
                _customers.LoadCompleted += OnCustomersLoaded;

                // Raise the PropertyChanged events.
                NotifyPropertyChanged("Customers");
            }
        }

        // Used to determine whether the data is loaded.
        public bool IsDataLoaded { get; private set; }

        // Loads data when the application is initialized.
        public void LoadData()
        {
            // Instantiate the context and binding collection.
            _context = new NorthwindEntities(_rootUri);
            Customers = new DataServiceCollection<Customer>(_context);

            // Specify an OData query that returns all customers.
            var query = from cust in _context.Customers
                        select cust;

            // Load the customer data.
            Customers.LoadAsync(query);
        }

        // Displays data from the stored data context and binding collection.  
        public void LoadData(NorthwindEntities context,
            DataServiceCollection<Customer> _customers)
        {
            _context = context;
            Customers = _customers;

            IsDataLoaded = true;
        }

        // Handles the DataServiceCollection<T>.LoadCompleted event.
        private void OnCustomersLoaded(object sender, LoadCompletedEventArgs e)
        {
            // Make sure that we load all pages of the Customers feed.
            if (Customers.Continuation != null)
            {
                Customers.LoadNextPartialSetAsync();
            }
            IsDataLoaded = true;
        }

        // Declare a PropertyChanged for the UI to register 
        // to get updates from the ViewModel.
        public event PropertyChangedEventHandler PropertyChanged;

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
        
        // Return a string serialization of the application state.
        public string SaveState()
        {
            if (App.ViewModel.IsDataLoaded)
            {
                // Create a new dictionary to store binding collections. 
                var collections = new Dictionary<string, object>();

                // Add the current Customers binding collection.
                collections["Customers"] = Customers;

                // Return the serialized context and binding collections.
                return DataServiceState.Serialize(_context, collections);
            }
            else
            {
                return string.Empty;
            }
        }

        // Restores the view model state from the supplied state serialization.
        public void RestoreState(string appState)
        {
            // Create a dictionary to hold any stored binding collections.
            Dictionary<string, object> collections;

            if (!string.IsNullOrEmpty(appState))
            {
                // Deserialize the DataServiceState object.
                DataServiceState state
                    = DataServiceState.Deserialize(appState);

                // Restore the context and binding collections.
                var context = state.Context as NorthwindEntities;
                collections = state.RootCollections;

                // Get the binding collection of Customer objects.
                DataServiceCollection<Customer> customers
                    = collections["Customers"] as DataServiceCollection<Customer>;

                // Initialize the application with stored data. 
                App.ViewModel.LoadData(context, customers);
            }
        }
        public void CancelCustomersAsyncLoad()
        {
            // Call the CancelAsyncLoad method on the binding collection.
            this.Customers.CancelAsyncLoad();
        }
        public void SaveChanges()
        {
            // Start the save changes operation. 
            this._context.BeginSaveChanges(OnChangesSaved, this._context);
        }

        private void OnChangesSaved(IAsyncResult result)
        {
            // Use the Dispatcher to ensure that the 
            // asynchronous call returns in the correct thread.
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                this._context = result.AsyncState as NorthwindEntities;

                try
                {
                    // Complete the save changes operation.
                    this._context.EndSaveChanges(result);
                }
                catch (DataServiceRequestException ex)
                {
                    // Ideally, we should not create a UI element 
                    // from the ViewModel. A better way is to use a 
                    // service or event to report exceptions to the view.
                    MessageBox.Show(string.Format(
                            "{0} The target Northwind data service ('{1}') is read-only.",
                            ex.Message, this._context.BaseUri));
                }
            }
            );
        }
        public void Refresh()
        {
            // Cache the current merge option and change 
            // it to MergeOption.OverwriteChanges.
            MergeOption cachedOption = _context.MergeOption;
            _context.MergeOption = MergeOption.OverwriteChanges;

            // Reload data from the data service.
            this.LoadData();

            // Reset the merge option.
            _context.MergeOption = cachedOption;
        }

    }
}