// ---------------------------------------------------------- 
// Copyright (c) Microsoft Corporation.  All rights reserved. 
// ---------------------------------------------------------- 
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Data.Services.Client;
using ODataStreamingPhoneClient.Model;

namespace ODataStreamingPhoneClient
{
    public class MainViewModel : INotifyPropertyChanged
    {
        // Set the string of your streaming service implementation.
        //*****update this to your own streaming service implementation*****
        static string serviceUriString =
            "http://myserver/PhotoService/PhotoData.svc/";

        // Declare the service root URI.
        private Uri svcRootUri =
            new Uri(serviceUriString, UriKind.Absolute);

        // Declare our private binding collection.
        private DataServiceCollection<PhotoInfo> _photos;

        // Declare the DataServiceContext as private.
        private PhotoDataContainer _context;

        public bool IsDataLoaded { get; private set; }

        // Loads data from the data service for the first time.
        public void LoadData()
        {
            // Instantiate the context and binding collection.
            _context = new PhotoDataContainer(svcRootUri);
            Photos = new DataServiceCollection<PhotoInfo>(_context);

            // Load the data from the PhotoInfo feed.
            Photos.LoadAsync(new Uri("/PhotoInfo", UriKind.Relative));
        }

        // Provides data to the ViewModel with the provided objects, 
        // which enables us to restore the ViewModel after reactivation.
        public void LoadData(PhotoDataContainer context,
           DataServiceCollection<PhotoInfo> photos)
        {
            this._context = context;
            this.Photos = photos;

            IsDataLoaded = true;
        }

        // Public collection used for data binding.
        public DataServiceCollection<PhotoInfo> Photos
        {
            get
            {
                return _photos;
            }
            set
            {
                _photos = value;

                // Report the change in the binding collection.
                NotifyPropertyChanged("Photos");

                // Register a handler for the LoadCompleted event.
                _photos.LoadCompleted +=
                    new EventHandler<LoadCompletedEventArgs>(Photos_LoadCompleted);
            }
        }

        // Handles the LoadCompleted event.
        private void Photos_LoadCompleted(object sender, LoadCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                // Make sure that we load all pages of the Customers feed.
                if (_photos.Continuation != null)
                {
                    // Request the next page from the data service.
                    _photos.LoadNextPartialSetAsync();
                }
                else
                {
                    // All pages are loaded.
                    IsDataLoaded = true;
                }
            }
            else
            {
                if (MessageBox.Show(e.Error.Message, "Retry request?",
                    MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    this.LoadData();
                }
            }
        }

        // Returns the URI of the media resource for the provided media link entry.
        public Uri GetReadStreamUri(object entity)
        {
            // Return the URI of the media resource.
            return this._context.GetReadStreamUri(entity);
        }

        // Change notification event that is handled by the binding.
        public event PropertyChangedEventHandler PropertyChanged;

        // Notifies the binding about a changed property value.
        private void NotifyPropertyChanged(string propertyName)
        {
            var propertyChanged = PropertyChanged;
            if (propertyChanged != null)
            {
                // Raise the PropertyChanged event.
                propertyChanged(this,
                    new PropertyChangedEventArgs(propertyName));
            }
        }

        // Expose a way to set the media resource stream for the specified entity.
        public void SetSaveStream(object entity, System.IO.Stream stream,
            bool closeStream, string contentType, string slug)
        {
            this._context.SetSaveStream(entity, stream,
                closeStream, contentType, slug);
        }

        public void SaveChanges()
        {
            // Send an insert or update request to the data service.            
            this._context.BeginSaveChanges(OnSaveChangesCompleted, null);
        }

        private void OnSaveChangesCompleted(IAsyncResult result)
        {
            EntityDescriptor entity = null;

            // Use the Dispatcher to ensure that the response is
            // marshaled back to the UI thread.
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                try
                {
                    // Complete the save changes operation and display the response.
                    DataServiceResponse response = _context.EndSaveChanges(result);

                    // When we issue a POST request, the photo ID and 
                    // edit-media link are not updated on the client (a bug), 
                    // so we need to get the server values. 
                    if (response != null && response.Count() > 0)
                    {
                        var operation = response.FirstOrDefault()
                            as ChangeOperationResponse;
                        entity = operation.Descriptor as EntityDescriptor;

                        var changedPhoto = entity.Entity as PhotoInfo;

                        if (changedPhoto != null && changedPhoto.PhotoId == 0)
                        {
                            // Verify that the entity was created correctly. 
                            if (entity != null && entity.EditLink != null)
                            {
                                // Detach the new entity from the context.
                                _context.Detach(entity.Entity);

                                // Get the updated entity from the service. 
                                _context.BeginExecute<PhotoInfo>(entity.EditLink,
                                    OnExecuteCompleted, null);
                            }
                        }
                        else
                        {
                            // Raise the SaveChangesCompleted event.
                            if (SaveChangesCompleted != null)
                            {
                                SaveChangesCompleted(this, new AsyncCompletedEventArgs());
                            }
                        }
                    }
                }
                catch (DataServiceRequestException ex)
                {
                    // Display the error from the response.
                    MessageBox.Show(ex.Message);
                }
                catch (InvalidOperationException ex)
                {
                    MessageBox.Show(ex.GetBaseException().Message);
                }
            });
        }

        private void OnExecuteCompleted(IAsyncResult result)
        {
            // Use the Dispatcher to ensure that the response is
            // marshaled back to the UI thread.
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                try
                {
                    // Complete the query by assigning the returned
                    // entity, which materializes the new instance 
                    // and attaches it to the context. We also need to assign the 
                    // new entity in the collection to the returned instance.
                    PhotoInfo entity = _photos[_photos.Count - 1] =
                        _context.EndExecute<PhotoInfo>(result).FirstOrDefault();

                    // Report that that media resource URI is updated.
                    entity.ReportStreamUriUpdated();
                }
                catch (DataServiceQueryException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    // Raise the event by using the () operator.
                    if (SaveChangesCompleted != null)
                    {
                        SaveChangesCompleted(this, new AsyncCompletedEventArgs());
                    }
                }
            });
        }

        // Declare a delegate for the SaveChangesCompleted event.
        public delegate void SaveChangesCompletedEventHandler(object sender, AsyncCompletedEventArgs e);

        // Declare the event.
        public event SaveChangesCompletedEventHandler SaveChangesCompleted;

        // Saves the current state to persisted storage.
        public DataServiceState SaveState()
        {
            // Create a new dictionary to store binding collections. 
            var collections = new Dictionary<string, object>();

            // Add the current Titles binding collection.
            collections.Add("Photos", App.ViewModel.Photos);

            // Add the current context and binding collections to the list.
            return DataServiceState.Save(App.ViewModel._context, collections);
        }

        // Restores the ViewModel state from the supplied state dictionary.
        public void RestoreState(IDictionary<string, object> dictionary)
        {
            // Create a dictionary to hold any stored binding collections.
            Dictionary<string, object> collections;

            // Get the stored DataServiceState object from the dictionary.
            var state = dictionary["DataServiceState"] as DataServiceState;

            if (state != null)
            {
                // Restore the context and binding collections.
                PhotoDataContainer context
                    = state.Restore(out collections) as PhotoDataContainer;

                // Get the binding collection of Title objects.
                DataServiceCollection<PhotoInfo> photos
                    = collections["Photos"] as DataServiceCollection<PhotoInfo>;

                // Initialize the application with stored data. 
                App.ViewModel.LoadData(context, photos);

                dictionary.Remove("DataServiceState");
            }
        }
    }
}
