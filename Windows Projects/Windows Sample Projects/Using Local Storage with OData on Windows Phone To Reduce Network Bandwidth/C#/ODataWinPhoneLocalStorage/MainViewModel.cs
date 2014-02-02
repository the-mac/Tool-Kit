// ----------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// ----------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Services.Client;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;
//using NetflixCatalog;
using ODataWinPhoneQuickstart.DataServiceContext.Netflix;

namespace ODataWinPhoneQuickstart
{
    public class MainViewModel : INotifyPropertyChanged
    {       
        // Define the client paging parameters.
        private int _pageSize = 20;
        private int _currentPage = 0;
        private int _totalCount = 0;

        // Define the isolate storage path.
        private static string isoPathName = "ImageCache";

        // Define the application settings.
        private IsolatedStorageSettings appSettings;

        // Define the message string.
        private string _message = string.Empty;

        // Defines the root URI of the data service.
        // We are using the default service in the proxy.
        private static readonly Uri _rootUri = new Uri("http://odata.netflix.com/Catalog/");

        // Define the local database.
        private NetflixCatalogLocalDB localDb;

        // Define the typed DataServiceContext.
        private NetflixCatalog _context;

        // Define the binding collection for Titles.
        private ObservableCollection<Title> _titles;

        public MainViewModel()
        {
            // Instantate the DataContext.
            localDb = new NetflixCatalogLocalDB();
            
            // Instantiate the context and binding collection using the stored URI.
            this._context = new NetflixCatalog(_rootUri);

            appSettings = IsolatedStorageSettings.ApplicationSettings;

            _currentPage = 0;
        }
       
        // Loads data when the application is initialized.
        public void LoadData()
        {           
            // Try to get any stored count information.
            if (_totalCount == 0)
            {
                if (appSettings.TryGetValue("TotalCount", out _totalCount))
                {
                    // Update the pages loaded text.
                    NotifyPropertyChanged("PagesLoadedText");
                }
            }

            // Make sure we have a local DB to use, we can't place this in the
            // constructor because there might have been a previous DeleteDatabase call.
            if (!localDb.DatabaseExists())
            {
                // Create the database if it doesn't exist.
                localDb.CreateDatabase();
            }

            try
            {
                // Try to get entities from local database.
                var storedTitles = BuildCommonTitlesQuery(localDb.Titles);
                                   
                if (storedTitles != null && storedTitles.Count() < _pageSize )
                {
                    // Load the page from the data service.
                    var titlesFromService = new DataServiceCollection<Title>(this._context);

                    titlesFromService.LoadCompleted += this.OnTitlesLoaded;

                    var query = _context.Titles;

                    if (_totalCount == 0)
                    {
                        // If we don't yet have the total count, then request it now.
                        query = query.IncludeTotalCount();
                    }

                    // Load the data from the OData service.
                    titlesFromService.LoadAsync(BuildCommonTitlesQuery(query));
                }
                else
                {
                    // Bind to the data from the local database.
                    this.Titles = new ObservableCollection<Title>(storedTitles);

                    IsDataLoaded = true;
                    
                    if (LoadCompleted != null)
                    {
                        LoadCompleted(this, new SourcesLoadCompletedEventArgs(null));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to load stored titles. " + ex.Message);

                if (LoadCompleted != null)
                {
                    LoadCompleted(this, new SourcesLoadCompletedEventArgs(ex));
                }
            }           
        }

        // Gets and sets the collection of Title objects from the feed or local storage.
        public ObservableCollection<Title> Titles
        {
            get { return _titles; }

            private set
            {
                // Set the Titles collection.
                _titles = value;

                // Raise the PropertyChanged events.
                NotifyPropertyChanged("Titles");
                NotifyPropertyChanged("PagesLoadedText");
            }
        }

        // Used to determine whether the data is loaded.
        public bool IsDataLoaded { get; private set; }

        // Selected title for binding in the details page.
        private Title _selectedTitle;
        public Title SelectedTitle
        {
            get
            {
                return _selectedTitle;
            }
            set
            {
                _selectedTitle = value;
                NotifyPropertyChanged("SelectedTitle");
            }
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
                _currentPage = value;
                NotifyPropertyChanged("CurrentPage");
            }
        }       

        // Gets and sets the total item count, which is needed for tombstoning.
        public int TotalCount
        {
            get
            {
                return _totalCount;
            }
            private set
            {
                // No need to report this change since we are not binding to it.
                _totalCount = value;

                // Store this in case we reload from the local db.
                appSettings.Add("TotalCount", _totalCount);
                appSettings.Save();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnTitlesLoaded(object sender, LoadCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                // Get the binding collection, which is the sender.
                DataServiceCollection<Title> loadedTitles = sender as DataServiceCollection<Title>;

                if (loadedTitles != null)
                {
                    // Make sure that we load all pages of the Customers feed.
                    if (loadedTitles.Continuation != null)
                    {
                        loadedTitles.LoadNextPartialSetAsync();
                    }

                    // Set the total page count, if we requested one.
                    if (e.QueryOperationResponse.Query
                        .RequestUri.Query.Contains("$inlinecount=allpages"))
                    {
                        this.TotalCount = (int)e.QueryOperationResponse.TotalCount;
                    }

                    try
                    {
                        localDb.Titles.InsertAllOnSubmit<Title>(loadedTitles);

                        // We want to try and add all the entities even if they already exist.
                        localDb.SubmitChanges(ConflictMode.ContinueOnConflict);
                    }
                    catch (Exception)
                    {
                        // We don't care about general exceptions, which inculude SQL CE constraint 
                        // violations when we try to store the same entity twice, 
                        // so we just eat the error.                        
                    }                   

                    // Unregister the event handler.
                    loadedTitles.LoadCompleted -= OnTitlesLoaded;

                    // Set the binding collection to the loaded titles.
                    this.Titles = loadedTitles;

                    IsDataLoaded = true;
                }              
            }
            else
            {
                // Display the error message in the binding.
                this.Message = e.Error.Message;
            }
            if (LoadCompleted != null)
            {
                LoadCompleted(this, new SourcesLoadCompletedEventArgs(e.Error));
            }
        }

        // Private method that returns the page-specific query.
        private IQueryable<Title> BuildCommonTitlesQuery(IQueryable<Title> query)
        {           
            // Enforce a sort order.
            query = query.OrderByDescending(t => t.ReleaseYear).ThenBy(t => t.Name);

            // Add paging to the query.
            query = query.Skip(_currentPage * _pageSize)
                .Take(_pageSize);

            return query;
        }

        // Calls into the DataServiceContext to get the URI of the media resource.
        public BitmapImage GetImage(object entity)
        {
            // First check for the image stored locally.
            // Obtain the virtual store for the application.
            IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication();

            Title title = entity as Title;
            MergeOption cacheMergeOption;
            Uri entityUri;            

            // Construct the file name from the entity ID.
            string fileStorageName = string.Format("{0}\\{1}.png", isoPathName, title.Id);

            if (!isoStore.DirectoryExists(isoPathName))
            {
                // Create a new folder if it doesn't exist.
                isoStore.CreateDirectory(isoPathName);
            }

            // We need to handle the case where we have stored the entity but not the image.
            if (!isoStore.FileExists(fileStorageName))
            {
                // Try to get the key of the title entity; if it's not in the DataServiceContext, 
                // then the entity comes from  the local database and it is not in the 
                // DataServiceContext, which means that we need to request it again to get 
                // the URI of the media resource.
                if (!_context.TryGetUri(entity, out entityUri))
                {
                    // We need to attach the entity to request it from the data service.
                    _context.AttachTo("Titles", entity);

                    if (_context.TryGetUri(entity, out entityUri))
                    {
                        // Cache the current merge option and change it to overwrite changes.
                        cacheMergeOption = _context.MergeOption;
                        _context.MergeOption = MergeOption.OverwriteChanges;

                        // Request the Title entity again from the data service.
                        _context.BeginExecute<Title>(entityUri, OnExecuteComplete, entity);

                        // Reset the merge option.
                        _context.MergeOption = cacheMergeOption;
                    }
                }
                else
                {
                    DataServiceRequestArgs args = new DataServiceRequestArgs();

                    // If the file doesn't already exist, request it from the data service.
                    _context.BeginGetReadStream(title, args, OnGetReadStreamComplete, title);                    
                }

                // We don't have an image yet to set.
                return null;
            }
            else
            {
                using (var fs = new IsolatedStorageFileStream(fileStorageName, FileMode.Open, isoStore))
                {
                    // Return the image as a BitmapImage.
                    // Create a new bitmap image using the memory stream.
                    BitmapImage imageFromStream = new BitmapImage();
                    imageFromStream.SetSource(fs);

                    // Return the bitmap.
                    return imageFromStream;
                }
            }            
        }

        private void OnExecuteComplete(IAsyncResult result)
        {
            // Get the title entity. 
            Title storedTitle = result.AsyncState as Title;

            if (storedTitle != null)
            {
                // Use the Dispatcher to ensure that the 
                // asynchronous call returns in the correct thread.
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        try
                        {
                            // Get the entity returned by the data service.
                            Title titleFromService =
                                _context.EndExecute<Title>(result).FirstOrDefault();

                            if (titleFromService != null)
                            {
                                // If we got back the title, then load the media resource.
                                _context.BeginGetReadStream(titleFromService,
                                    new DataServiceRequestArgs(), OnGetReadStreamComplete, storedTitle);
                            }
                        }
                        catch (Exception)
                        {
                            // If we can't get the entity, then it may have been 
                            // deleted from the data service, so remove it.
                            Titles.Remove(storedTitle);
                        }
                    });
            }
        }
        private void OnGetReadStreamComplete(IAsyncResult result)
        {
            // Obtain the virtual store for the application.
            IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication();

            Title title = result.AsyncState as Title;

            if (title != null)
            {
                // Use the Dispatcher to ensure that the 
                // asynchronous call returns in the correct thread.
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        try
                        {
                            // Get the response.
                            DataServiceStreamResponse response =
                                _context.EndGetReadStream(result);

                            // Construct the file name from the entity ID.
                            string fileStorageName = string.Format("{0}\\{1}.png",
                                isoPathName, title.Id);

                            // Specify the file path and options.
                            using (var isoFileStream =
                                new IsolatedStorageFileStream(fileStorageName,
                                    FileMode.Create, isoStore))
                            {
                                //Write the data
                                using (var fileWriter = new BinaryWriter(isoFileStream))
                                {
                                    byte[] buffer = new byte[1000];
                                    int count = 0;

                                    // Read the returned stream into the new file stream.
                                    while (response.Stream.CanRead && (0 < (
                                        count = response.Stream.Read(buffer, 0, buffer.Length))))
                                    {
                                        fileWriter.Write(buffer, 0, count);
                                    }
                                }
                            }

                            using (var bitmapFileStream =
                                new IsolatedStorageFileStream(fileStorageName,
                                    FileMode.Open, isoStore))
                            {
                                // Return the image as a BitmapImage.
                                // Create a new bitmap image using the memory stream.
                                BitmapImage imageFromStream = new BitmapImage();
                                imageFromStream.SetSource(bitmapFileStream);

                                // Return the bitmap.                                    
                                title.DefaultImage = imageFromStream;
                            }
                        }

                        catch (DataServiceClientException)
                        {
                            // We need to eat this exception so that loading can continue.
                            // Plus there is a bug where the binary stream gets
                            /// written to the message.
                        }
                    });
            }
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
            // Since we are storing entities in local database,
            // we don't need to store the OData client objects.               
            List<KeyValuePair<string, object>> stateList
                = new List<KeyValuePair<string, object>>();

            stateList.Add(new KeyValuePair<string, object>("CurrentPage", CurrentPage));
            stateList.Add(new KeyValuePair<string, object>("SelectedTitle", SelectedTitle));

            return stateList;
        }

        // Restores the view model state from the supplied state dictionary.
        public void RestoreState(IDictionary<string, object> storedState)
        {           
            // Restore view model data.
            _currentPage = (int)storedState["CurrentPage"];
            this.SelectedTitle = storedState["SelectedTitle"] as Title;
        }

        public event EventHandler<SourcesLoadCompletedEventArgs> LoadCompleted;

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
       // public bool ClearCacheOnExit { get; set; }
        public void ClearLocalStorageOnNextStart()
        {
            if (!appSettings.Contains("ClearCache"))
            {
                appSettings.Add("ClearCache", true);
            }
            else
            {
                appSettings["ClearCache"] = true;
            }
            appSettings.Save();
        }
        public void ClearCache()
        {
            if (localDb != null)
            {
                // Delete the DB to clear local storage.
                localDb.DeleteDatabase();
            }

            // Obtain the virtual store for the application.
            IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication();

            foreach (string file in isoStore.GetFileNames(string.Format("{0}\\*.*", isoPathName)))
            {
                isoStore.DeleteFile(string.Format("{0}\\{1}", isoPathName, file));
            }
        }
    }

    // Event args used to notify the UI that the load is completed.
    public class SourcesLoadCompletedEventArgs : EventArgs
    {
        public SourcesLoadCompletedEventArgs(Exception error)
        {
            this.Error = error;
        }
        public Exception Error { get; set; }
    }
}