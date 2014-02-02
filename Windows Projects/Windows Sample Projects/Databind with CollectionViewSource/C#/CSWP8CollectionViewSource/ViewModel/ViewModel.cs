/****************************** Module Header ******************************\
* Module Name:    ViewModel.cs
* Project:        CSWP8CollectionViewSource
* Copyright (c) Microsoft Corporation
*
* This is the ViewModel.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\*****************************************************************************/

using CSWP8CollectionViewSource.Model;
using System;
using System.Collections.ObjectModel;
using System.IO.IsolatedStorage;
using System.Windows;

namespace CSWP8CollectionViewSource.ViewModelNamespace
{
    public class ViewModel
    {
        // Collection of ReadinessAndLevel.
        public ObservableCollection<ReadinessAndLevel> readinessAndLevels { get; set; }

        /// <summary>
        /// Get ReadinessAndLevels.
        /// </summary>
        public void GetReadinessAndLevels()
        {
            if (IsolatedStorageSettings.ApplicationSettings.Count > 0)
            {
                GetSavedReadinessAndLevels();
            }
            else
            {
                GetDefaultReadinessAndLevels();
            }
        }

        /// <summary>
        /// Initialization data.
        /// </summary>
        public void GetDefaultReadinessAndLevels()
        {
            ObservableCollection<ReadinessAndLevel> a = new ObservableCollection<ReadinessAndLevel>();

            // Items to collect
            a.Add(new ReadinessAndLevel() { Name = "ASPNET", Type = "Item" });
            a.Add(new ReadinessAndLevel() { Name = "SharePoint", Type = "Item" });
            a.Add(new ReadinessAndLevel() { Name = "Azure", Type = "Item" });
            a.Add(new ReadinessAndLevel() { Name = "Windows Phone", Type = "Item" });

            // Levels to complete
            a.Add(new ReadinessAndLevel() { Name = "Level 100", Type = "Level" });
            a.Add(new ReadinessAndLevel() { Name = "Level 200", Type = "Level" });
            a.Add(new ReadinessAndLevel() { Name = "Level 300", Type = "Level" });

            readinessAndLevels = a;
            //MessageBox.Show("Got ReadinessAndLevels from default");
        }

        /// <summary>
        /// IsolatedStorage data.
        /// </summary>
        public void GetSavedReadinessAndLevels()
        {
            ObservableCollection<ReadinessAndLevel> a = new ObservableCollection<ReadinessAndLevel>();

            foreach (Object o in IsolatedStorageSettings.ApplicationSettings.Values)
            {
                a.Add((ReadinessAndLevel)o);
            }

            readinessAndLevels = a;
            //MessageBox.Show("Got ReadinessAndLevels from storage");
        }
       
        /// <summary>
        /// Save data to IsolatedStorage.
        /// </summary>
        public void SaveReadinessAndLevels()
        {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;

            foreach (ReadinessAndLevel a in readinessAndLevels)
            {
                if (settings.Contains(a.Name))
                {
                    settings[a.Name] = a;
                }
                else
                {
                    settings.Add(a.Name, a.GetCopy());
                }
            }

            settings.Save();
            MessageBox.Show("Finished saving ReadinessAndLevels");
        }
    }
}
