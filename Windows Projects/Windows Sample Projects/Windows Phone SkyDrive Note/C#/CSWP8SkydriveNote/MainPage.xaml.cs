/****************************** Module Header ******************************\
* Module Name:    MainPage.xaml.cs
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
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using CSWP8SkydriveNote.Resources;
using System.Windows.Input;
using System.IO.IsolatedStorage;
using System.Xml.Serialization;
using System.IO;

namespace CSWP8SkydriveNote
{
    public partial class MainPage : PhoneApplicationPage
    {
        public static List<Note> CreateTestList()
        {
            List<Note> list = new List<Note>();
            Note note = new Note();

            // Get files from isolated store.
            IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication();
            XmlSerializer serializer = new XmlSerializer(typeof(Note));
            var filesName = store.GetFileNames();
            if (filesName.Length > 0)
            {
                foreach (string fileName in filesName)
                {
                    if (fileName == "__ApplicationSettings") continue;
                    using (var file = store.OpenFile(fileName, FileMode.Open))
                    {
                        note = (Note)serializer.Deserialize(file);
                        list.Add(note);
                    }
                }
            }

            store.Dispose();
            return list;
        }

        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            listBox_NoteList.ItemsSource = CreateTestList();
        }

        private void listBox_NoteList_Hold(object sender, GestureEventArgs e)
        {
            // Get item data.
            FrameworkElement element = (FrameworkElement)e.OriginalSource;
            Note item = (Note)element.DataContext;

            // Get element data.
            ListBox listBox = sender as ListBox;
            var selectedItem = listBox.SelectedItem;

            MessageBox.Show(item.Title);
        }

        private void listBox_NoteList_Tap(object sender, GestureEventArgs e)
        {
            // Get item data
            FrameworkElement element = (FrameworkElement)e.OriginalSource;
            Note item = (Note)element.DataContext;
            if (item == null)
            {
                return;
            }

            string urlWithData = string.Format("/MyNote.xaml?NoteId={0}", item.NoteID);
            NavigationService.Navigate(new Uri(urlWithData, UriKind.Relative));
        }

        private void btn_NewNote_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to MyNote.xaml page
            NavigationService.Navigate(new Uri("/MyNote.xaml", UriKind.Relative));
        }
    }
}