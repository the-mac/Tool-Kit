/* 
    Copyright (c) 2012 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
using System;
using System.Windows;
using Microsoft.Phone.Controls;

using Microsoft.Phone.Tasks;
using Microsoft.Xna.Framework.Media;
using System.IO;

namespace sdkPhotoExtensibilityWP8CS
{
    public partial class PhotoSave : PhoneApplicationPage
    {
        // Camera capture task
        CameraCaptureTask cameraCaptureTask;
        string newPhotoName;

        public PhotoSave()
        {
            InitializeComponent();

            // Initialize the camera capture task object.
            cameraCaptureTask = new CameraCaptureTask();
            cameraCaptureTask.Completed += new EventHandler<PhotoResult>(cameraCaptureTask_Completed);
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Launch the built-in camera app.
                cameraCaptureTask.Show();
            }
            catch (Exception err)
            {
                MessageBox.Show("An error occurred: " + err.Message);
            }
        }

        void cameraCaptureTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                // Save the photo on a different thread.
                this.Dispatcher.BeginInvoke(delegate()
                {
                    // Create the file name.
                    newPhotoName = Path.GetFileNameWithoutExtension(e.OriginalFileName) + "_edited.jpg";

                    // Save the photo to the media library.
                    MediaLibrary library = new MediaLibrary();
                    library.SavePictureToCameraRoll(newPhotoName, e.ChosenPhoto);

                    MessageBox.Show("Capture successful. Tap Start, view the photo in the Camera Roll, then in the app bar, tap Open.");
                });   
            }
        }
    }
}
