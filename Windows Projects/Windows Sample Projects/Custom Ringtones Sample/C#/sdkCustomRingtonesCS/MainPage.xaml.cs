/* 
    Copyright (c) 2012 - 2013 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://code.msdn.microsoft.com/wpapps
  
*/
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;

namespace sdkCustomRingtonesCS
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Declare the SaveRingtoneTask object with page scope.
        SaveRingtoneTask saveRingtoneChooser;

        // Path where ringtone files are stored
        string filepath = @"Ringtones/";

        // Used to simulate the XNA Framework Game loop.
        GameTimer timer;

        // Flag that indicates if we need to resume 
        // Zune playback after previewing a ringtone.
        bool resumeMediaPlayerAfterDone = false;


        // Constructor
        public MainPage()
        {
            // Create a GameTimer to pump the XNA Framework.
            timer = new GameTimer();
            timer.UpdateInterval = TimeSpan.FromTicks(333333);
            timer.Update += new EventHandler<GameTimerEventArgs>(timer_Update);

            // Start the timer, which calls FrameworkDispatcher.Update regularly
            timer.Start();

            // Initialize the SaveRingtoneTask and assign the Completed handler.
            saveRingtoneChooser = new SaveRingtoneTask();
            saveRingtoneChooser.Completed += new EventHandler<TaskEventArgs>(saveRingtoneChooser_Completed);

            InitializeComponent();
        }


        #region Event Handlers

        /// <summary>
        /// Pumps the XNA Framework internals.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void timer_Update(object sender, GameTimerEventArgs e)
        {
            FrameworkDispatcher.Update();
        }


        /// <summary>
        /// Handles the preview button Click event.
        /// Toggles between Preview and Pause states.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void previewToggleButton_Click(object sender, EventArgs e)
        {
            // If we're already playing a ringtone, stop playback
            if (MediaElementState.Playing == ringtonePlayer.CurrentState)
            {
                // Stop playback
                ringtonePlayer.Stop();
                return;
            }

            string ringtonePath = GetRingtonePath();

            if (null != ringtonePath)
            {
                // If Zune is playing music, pause 
                // it while we play the ringtone.
                ZunePause();

                ringtonePlayer.Source = new Uri(ringtonePath, UriKind.Relative);
                ringtonePlayer.Play();
            }
        }


        /// <summary>
        /// Updates the UI and resumes Zune playback, if necessary.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void media_CurrentStateChanged(object sender, RoutedEventArgs e)
        {
            if (MediaElementState.Playing == ringtonePlayer.CurrentState)
            {
                // The ringtone is playing, display the Stop button.
                (ApplicationBar.Buttons[0] as ApplicationBarIconButton).IconUri = new Uri("Images/stop.png", UriKind.Relative);
                (ApplicationBar.Buttons[0] as ApplicationBarIconButton).Text = "stop";
            }
            else
            {
                // The ringtone is not playing, display the Play button.
                (ApplicationBar.Buttons[0] as ApplicationBarIconButton).IconUri = new Uri("Images/play.png", UriKind.Relative);
                (ApplicationBar.Buttons[0] as ApplicationBarIconButton).Text = "preview";

                // If Zune was playing music before we 
                // played a ringtone, resume playback.
                ZuneResume();
            }
        }


        /// <summary>
        /// In this example, the SaveRingtoneTask is shown in response to a button click. 
        /// The ringtone file name and display name are obtained from a ListBox control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addButton_Click(object sender, EventArgs e)
        {
            string ringtonePath = GetRingtonePath();

            if (null != ringtonePath)
            {
                // Download the ringtone to local isolated storage
                DownloadToIsoStore(ringtonePath);

                // Set up parameters for the SaveRingtoneTask
                // For more on the "isostore:/" path syntax, see the Silverlight Uri class documentation.
                saveRingtoneChooser.Source = new Uri(@"isostore:/" + ringtonePath);
                saveRingtoneChooser.DisplayName = (string)((ListBoxItem)ringtonesListBox.SelectedItem).Content;
                saveRingtoneChooser.IsShareable = true;

                // If we're playing a ringtone, stop playback
                if (MediaElementState.Playing == ringtonePlayer.CurrentState)
                {
                    // Stop playback
                    ringtonePlayer.Stop();
                }

                // Launch the SaveRingtoneTask chooser
                saveRingtoneChooser.Show();
            }
        }


        /// <summary>
        /// The Completed event handler. No data is returned from this Chooser, but 
        /// the TaskResult field indicates if the task was completed or cancelled.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void saveRingtoneChooser_Completed(object sender, TaskEventArgs e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                statusTextBlock.Text = "Save completed.";
            }
            else if (e.TaskResult == TaskResult.Cancel)
            {
                statusTextBlock.Text = "Save cancelled.";
            }

            // Delete the downloaded ringtone file.
            DeleteFromIsoStore(((SaveRingtoneTask)sender).Source.AbsolutePath);
        }


        /// <summary>
        /// Stops playing the previous ringtone when the user changes the selection in the ListBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ringtonesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((null != ringtonePlayer) && (MediaElementState.Playing == ringtonePlayer.CurrentState))
            {
                ringtonePlayer.Stop();
            }
        }

        #endregion Event Handlers


        #region Ringtone File Handling

        /// <summary>
        /// Constructs the path to the media file based on the 
        /// Name property of the currently selected ListBoxItem.
        /// </summary>
        /// <returns></returns>
        string GetRingtonePath()
        {
            string strMsg = string.Empty;
            string ringtonePath = null;

            // A ListBox contains the available ringtones
            ListBoxItem selection = (ListBoxItem)ringtonesListBox.SelectedItem;

            if (null != selection)
            {
                // The filename is stored in the Name property
                // of each ListBoxItem, minus the file extension
                String filename = selection.Name + ".wma";

                ringtonePath = filepath + filename;
            }
            else
            {
                strMsg = "Nothing selected";
            }

            statusTextBlock.Text = strMsg;
            return ringtonePath;
        }


        /// <summary>
        /// Simulates downloading a ringtone file to application isolated 
        /// storage by just copying it from application data storage.
        /// </summary>
        /// <param name="fileName"></param>
        private void DownloadToIsoStore(string fileName)
        {
            IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication();

            // If the file already exists, no need to "download", just return
            if (isoStore.FileExists(fileName))
            {
                return;
            }

            StreamResourceInfo sr = Application.GetResourceStream(new Uri(fileName, UriKind.Relative));

            using (BinaryReader br = new BinaryReader(sr.Stream))
            {
                // Simulate "downloading" the ringtone file
                byte[] data = br.ReadBytes((int)sr.Stream.Length);

                // Save to local isolated storage
                SaveToIsoStore(fileName, data);
            }

        }


        /// <summary>
        /// Create a file in the application's isolated storage.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="data"></param>
        private void SaveToIsoStore(string fileName, byte[] data)
        {
            string strBaseDir = string.Empty;
            string delimStr = "/";
            char[] delimiter = delimStr.ToCharArray();
            string[] dirsPath = fileName.Split(delimiter);

            // Get the IsoStore
            IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication();

            // We want to re-create the directory structure but not the file
            for (int i = 0; i < dirsPath.Length - 1; i++)
            {
                strBaseDir = System.IO.Path.Combine(strBaseDir, dirsPath[i]);

                if (!isoStore.DirectoryExists(strBaseDir))
                {
                    isoStore.CreateDirectory(strBaseDir);
                }
            }

            // Clean out existing file.
            DeleteFromIsoStore(fileName);

            // Write the file 
            using (BinaryWriter bw = new BinaryWriter(isoStore.CreateFile(fileName)))
            {
                bw.Write(data);
                bw.Close();
            }
        }


        /// <summary>
        /// Deletes a file from isolated storage.
        /// </summary>
        /// <param name="fileName"></param>
        private void DeleteFromIsoStore(string fileName)
        {
            // Get the IsoStore
            IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication();

            // Clean out existing file.
            if (isoStore.FileExists(fileName))
            {
                isoStore.DeleteFile(fileName);
            }
        }

        #endregion Ringtone File Handling


        #region Zune Pause/Resume

        private void ZunePause()
        {
            // Please see the MainPage() constructor above where the GameTimer object is created.
            // This enables the use of the XNA framework MediaPlayer class by pumping the XNA FrameworkDispatcher.

            // Pause the Zune player if it is already playing music.
            if (!MediaPlayer.GameHasControl)
            {
                MediaPlayer.Pause();
                resumeMediaPlayerAfterDone = true;
            }
        }

        private void ZuneResume()
        {
            // If Zune was playing music, resume playback
            if (resumeMediaPlayerAfterDone)
            {
                MediaPlayer.Resume();
            }
        }

        #endregion Zune Pause/Resume
    }
}
