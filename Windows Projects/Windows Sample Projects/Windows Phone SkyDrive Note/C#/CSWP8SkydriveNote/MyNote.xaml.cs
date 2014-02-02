/****************************** Module Header ******************************\
* Module Name:    MyNote.xaml.cs
* Project:        CSWP8SkydriveNote
* Copyright (c) Microsoft Corporation
*
* This demo shows how to backup Note to Skydrive.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\*****************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Xml.Serialization;
using Microsoft.Live;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace CSWP8SkydriveNote
{
    public partial class MyNote : PhoneApplicationPage
    {
        Note note;

        // The isDeleteNote field is used to identify if the navigation action
        // is caused by delete, if yes, then, the note will not be saved. 
        bool isDeleteNote = false;
        string fileName;
        string noteId;

        private LiveConnectClient client;
        private LiveConnectSession session;

        public MyNote()
        {
            InitializeComponent();
            isDeleteNote = false;
        }

        /// <summary>
        /// Called when this page becomes the active page. 
        /// </summary>
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ShowNote();
            isDeleteNote = false;
        }

        /// <summary>
        /// Called just before navigating away from this page. 
        /// </summary>
        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        /// <summary>
        /// Called when page is navigating away. 
        /// </summary>
        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            if (isDeleteNote) return;
            SaveNote();
        }

        private void btn_Save_Click(object sender, RoutedEventArgs e)
        {
            if (isDeleteNote) return;
            SaveNote();
        }

        private void CreateNewNote()
        {
            note = new Note();
            note.NoteID = Guid.NewGuid();
            note.Title = txtBox_noteTitle.Text;
            note.Content = txtBox_Content.Text;
            note.CreatedDate = DateTime.Now;
            note.LastEditTime = DateTime.Now;

            noteId = note.NoteID.ToString();

            // serialize to xml and store into file
            IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication();
            using (var file = store.CreateFile(note.NoteID + ".txt"))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Note));
                serializer.Serialize(file, note);
            }
            store.Dispose();
        }

        private void ShowNote()
        {
            // Get the page id
            if (this.NavigationContext.QueryString.Count < 1)
            {
                CreateNewNote();
                return;
            }
            noteId = this.NavigationContext.QueryString["NoteId"].ToString();
            fileName = noteId + ".txt";

            // Deserialized the note base on the id. 
            IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication();
            XmlSerializer serializer = new XmlSerializer(typeof(Note));
            using (var file = store.OpenFile(fileName, FileMode.Open))
            {
                note = (Note)serializer.Deserialize(file);
            }

            txtBox_noteTitle.Text = note.Title;
            txtBox_Content.Text = note.Content;

            store.Dispose();
        }

        private void SaveNote()
        {
            fileName = noteId + ".txt";
            note.NoteID = new Guid(noteId);
            note.Title = txtBox_noteTitle.Text;
            note.Content = txtBox_Content.Text;
            note.CreatedDate = DateTime.Now;
            note.LastEditTime = DateTime.Now;

            // serialize to xml and store into file
            IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication();
            using (var file = store.CreateFile(note.NoteID + ".txt"))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Note));
                serializer.Serialize(file, note);
            }
            store.Dispose();
        }

        private void DeleteNote()
        {
            string idString = noteId;
            fileName = idString + ".txt";
            using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (store.FileExists(fileName))
                {
                    store.DeleteFile(fileName);
                }
            }

            isDeleteNote = true;
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        private void btn_Delete_Click(object sender, RoutedEventArgs e)
        {
            DeleteNote();
        }

        private void btn_Upload_SessionChanged(object sender, Microsoft.Live.Controls.LiveConnectSessionChangedEventArgs e)
        {
            if (isDeleteNote) return;
            SaveNote();

            if (e.Status == LiveConnectSessionStatus.Connected)
            {
                session = e.Session;
                client = new LiveConnectClient(session);
                Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show("Connected");
                });

                client = new LiveConnectClient(session);

                // Get files from isolated store
                IsolatedStorageFile store =
                    IsolatedStorageFile.GetUserStoreForApplication();
                IsolatedStorageFileStream fileStream =
                    store.OpenFile(fileName, FileMode.Open);

                // Upload files to document folder by friendly name.
                client.UploadCompleted += (obj, arg) =>
                {
                    if (arg.Error == null)
                    {
                        IDictionary<string, object> fileInfo = arg.Result;
                        string fileId = fileInfo["id"].ToString();
                        Dispatcher.BeginInvoke(() =>
                        {
                            MessageBox.Show(fileId);
                        });
                    }
                    else
                    {
                        string errorMessage = "Error calling API: " + arg.Error.Message;
                        Dispatcher.BeginInvoke(() =>
                        {
                            MessageBox.Show(errorMessage);
                        });
                    }
                };

                client.UploadAsync("me/skydrive/my_documents", fileName, fileStream, OverwriteOption.Overwrite);
            }

            if (e.Error != null)
            {
                Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show(e.Error.Message);
                });
            }
        }
    }

    public class Note
    {
        public Guid NoteID { get; set; }
        public string Title { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Content { get; set; }
        public DateTime LastEditTime { get; set; }
    }
}