/* 
    Copyright (c) 2012 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ScheduledTaskAgent1
{
  // This class prepares the picture for uploading.
  class ServiceUploadHelper
  {
    // The IsolatedStorageSettings contains the access key to use when uploading
    // to the service.
    private IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;

    // Used to check for already submitted photos.
    PhotoDataModel photoDataModel = new PhotoDataModel();

    // Tracks current number of photos uploaded.
    private static int counter;

    // Stores total number of pictures in the album.
    private static int totalNumberOfPictures;
    private static string strAccessToken;

    // Stores the callback for returning to ScheduledAgent.
    internal delegate void NotifyComplete();
    NotifyComplete notifyCompleteCallBack;

    // Loops over the albums stored on the phone until a match is found.
    private static PictureAlbum FindAlbum(MediaLibrary lib)
    {
      foreach (PictureAlbum album in lib.RootPictureAlbum.Albums)
      {
        if (album.Name == AsyncHttpPostHelper.ConstantStrings.albumName)
          return album;
        album.Dispose();
      }
      return null;
    }

    // Stores the callback for the ScheduledAgent and iterates over the photos
    // until the end of the album is reached.
    public void InitializeServiceUpload(NotifyComplete notifyCompleteDelegate)
    {
      notifyCompleteCallBack = notifyCompleteDelegate;

      // Make sure that the required key exists in the IsolatedStorageSettings
      // (i.e., the user is logged in).
      if (settings.Contains(AsyncHttpPostHelper.ConstantStrings.settingsKey))
      {
        // Find the album to upload.
        using (MediaLibrary m = new MediaLibrary())
        using (PictureAlbum cr = FindAlbum(m))
        {
          if (cr.Pictures.Count > 0)
          {
            // Save the total number of photos available in the album.
            totalNumberOfPictures = cr.Pictures.Count;

            // Decrypt the user's private settings key.
            byte[] byteAccessToken = ProtectedData.Unprotect((byte[])settings[AsyncHttpPostHelper.ConstantStrings.settingsKey], null);
            strAccessToken = Encoding.UTF8.GetString(byteAccessToken, 0, byteAccessToken.Length);

            // Begin the cycle by starting the upload with the first image.
            counter = 0;
            UploadPicture(cr, counter);
          }
          else
          {
            // Album is empty. No pictures to upload.
          }
        }
      }
      else
      {
        // User has not logged in. Abort.
        notifyCompleteCallBack();
      }
    }

    // Callback from ServiceUploadHelper
    private void OnHttpPostCompleteDelegate(Picture picture)
    {
      // At this state in the cycle, the given picture has been uploaded. Mark
      // it as so.
      photoDataModel.MarkUploaded(picture);
      if (counter + 1 == totalNumberOfPictures) // array based indexing vs natural numbers (starting at 1)
      {
        notifyCompleteCallBack(); // The upload process has completed.
      }
      else
      {
        UploadPicture(picture.Album, ++counter); // upload next picture
      }
    }

    // Checks to see if the picture is uploaded, if not, creates a
    // AsyncHttpPostHelper instance to handle the upload. The callback method is
    // passed to this instance to notify the ServiceUploadHelper when to upload
    // the next picture.
    private void UploadPicture(PictureAlbum album, int index)
    {
      Picture picture = album.Pictures[index];
      if (photoDataModel.IsAlreadyUploaded(picture))
      {
        // Do nothing
      }
      else
      {
	// Initialise a AsyncHttpPostHelper to upload the picture.
        Uri requestUri = new Uri(AsyncHttpPostHelper.ConstantStrings.apiUri + HttpUtility.UrlEncode(strAccessToken));
        AsyncHttpPostHelper asyncHttpPostHelper = new AsyncHttpPostHelper(picture, requestUri);
        asyncHttpPostHelper.BeginSend(OnHttpPostCompleteDelegate);
      }
    }
  }
}
