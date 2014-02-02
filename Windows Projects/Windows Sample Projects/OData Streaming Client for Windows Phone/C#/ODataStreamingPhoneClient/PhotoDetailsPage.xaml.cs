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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using System.Data.Services.Client;
using ODataStreamingPhoneClient.Model;

namespace ODataStreamingPhoneClient
{
    public partial class PhotoDetailsPage : PhoneApplicationPage
    {
        PhotoChooserTask photoChooser;

        // Define local variables.
        private PhotoInfo currentPhoto;
        private bool chooserCancelled;

        public PhotoDetailsPage()
        {
            InitializeComponent();

            // Initialize the PhotoChooserTask and assign the Completed handler.
            photoChooser = new PhotoChooserTask();
            photoChooser.Completed +=
                new EventHandler<PhotoResult>(photoChooserTask_Completed);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (chooserCancelled == true)
            {
                // The user did not choose a photo so return to the main page;
                // the added PhotoInfo is already removed.
                NavigationService.GoBack();

                // Void out the binding so that we don't try and bind
                // to an empty PhotoInfo object.
                this.DataContext = null;

                return;
            }

            // Get the selected PhotoInfo object.
            string indexAsString = this.NavigationContext.QueryString["selectedIndex"];
            int index = int.Parse(indexAsString);
            this.DataContext = currentPhoto
                = (PhotoInfo)App.ViewModel.Photos[index];

            // If this is a new photo, we need to get the image file.
            if (currentPhoto.PhotoId == 0
                && currentPhoto.FileName == string.Empty)
            {
                // Call the OnSelectPhoto method to open the chooser.
                this.OnSelectPhoto(this, new EventArgs());
            }
        }

        private void OnDeletePhoto(object sender, EventArgs e)
        {
            // Delete the selected photo.
            if (MessageBox.Show("Delete this photo?", "Confirm delete",
                MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                // Delete the current photo and save changes.
                App.ViewModel.Photos.Remove(currentPhoto);

                // Send the delete to the data service.
                this.OnSave(sender, e);
            }
        }

        private void OnSave(object sender, EventArgs e)
        {
            // Register for the SaveChangesCompleted event on the ViewModel.
            App.ViewModel.SaveChangesCompleted +=
                new MainViewModel.SaveChangesCompletedEventHandler(ViewModel_SaveChangesCompleted);

            // Call SaveChanges on the ViewModel.
            App.ViewModel.SaveChanges();

            // Show the progress bar during the request.
            this.requestProgress.Visibility = Visibility.Visible;
            this.requestProgress.IsIndeterminate = true;
        }

        void ViewModel_SaveChangesCompleted(object sender, EventArgs e)
        {
            // Hide the progress bar now that save changes operation is complete.
            this.requestProgress.Visibility = Visibility.Collapsed;
            this.requestProgress.IsIndeterminate = false;

            // Unregister for the SaveChangedCompleted event now that we are done.
            App.ViewModel.SaveChangesCompleted -=
                new MainViewModel.SaveChangesCompletedEventHandler(ViewModel_SaveChangesCompleted);

            // Return to the previous page.
            NavigationService.GoBack();
        }

        private void OnSelectPhoto(object sender, EventArgs e)
        {
            try
            {
                // Display the PhotoChooser.
                photoChooser.Show();
            }
            catch (System.InvalidOperationException ex)
            {
                MessageBox.Show("The image could not be selected. \n" + ex.Message);
            }
        }

        private void photoChooserTask_Completed(object sender, PhotoResult e)
        {
            // Get back the last PhotoInfo in the collection, which we just added.
            currentPhoto = App.ViewModel.Photos[App.ViewModel.Photos.Count - 1];

            if (e.TaskResult == TaskResult.OK)
            {
                // Set the file properties for the returned image.                
                string[] splitFileName = e.OriginalFileName.Split(new Char[] { '\\' });
                currentPhoto.FileName = splitFileName[splitFileName.Length - 1];
                currentPhoto.ContentType =
                    GetContentTypeFromFileName(currentPhoto.FileName);

                // Read remaining entity properties from the stream itself.
                currentPhoto.FileSize = (int)e.ChosenPhoto.Length;

                // Create a new image using the returned memory stream.
                BitmapImage imageFromStream =
                    new System.Windows.Media.Imaging.BitmapImage();
                imageFromStream.SetSource(e.ChosenPhoto);

                // Set the height and width of the image.
                currentPhoto.Dimensions.Height = (short?)imageFromStream.PixelHeight;
                currentPhoto.Dimensions.Width = (short?)imageFromStream.PixelWidth;

                this.PhotoImage.Source = imageFromStream;

                // Reset the stream before we pass it to the service. 
                e.ChosenPhoto.Position = 0;

                // Set the save stream for the selected photo stream.
                App.ViewModel.SetSaveStream(currentPhoto, e.ChosenPhoto, true,
                    currentPhoto.ContentType, currentPhoto.FileName);
            }
            else
            {
                // Assume that the select photo operation was cancelled,
                // remove the added PhotoInfo and navigate back to the main page.
                App.ViewModel.Photos.Remove(currentPhoto);
                chooserCancelled = true;
            }
        }
        private static string GetContentTypeFromFileName(string fileName)
        {
            // Get the file extension from the FileName property.
            string[] splitFileName = fileName.Split(new Char[] { '.' });

            // Return the Content-Type value based on the file extension.
            switch (splitFileName[splitFileName.Length - 1])
            {
                case "jpeg":
                    return "image/jpeg";
                case "jpg":
                    return "image/jpeg";
                case "gif":
                    return "image/gif";
                case "png":
                    return "image/png";
                case "tiff":
                    return "image/tiff";
                case "bmp":
                    return "image/bmp";
                default:
                    return "application/octet-stream";
            }
        }
    }
}