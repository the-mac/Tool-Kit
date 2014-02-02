/* 
    Copyright (c) 2011 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
using System;
using System.Windows;
using System.Collections.ObjectModel;
using System.IO.IsolatedStorage;
using sdkMVVMCS.Model;

namespace sdkMVVMCS.ViewModelNS
{
    public class ViewModel
    {
        public ObservableCollection<Accomplishment> Accomplishments { get; set; }


        public void GetAccomplishments()
        {
            if (IsolatedStorageSettings.ApplicationSettings.Count > 0)
            {
                GetSavedAccomplishments();
            }
            else
            {
                GetDefaultAccomplishments();
            }
        }


        public void GetDefaultAccomplishments()
        {
            ObservableCollection<Accomplishment> a = new ObservableCollection<Accomplishment>();

            // Items to collect
            a.Add(new Accomplishment() { Name = "Potions", Type = "Item" });
            a.Add(new Accomplishment() { Name = "Coins", Type = "Item" });
            a.Add(new Accomplishment() { Name = "Hearts", Type = "Item" });
            a.Add(new Accomplishment() { Name = "Swords", Type = "Item" });
            a.Add(new Accomplishment() { Name = "Shields", Type = "Item" });

            // Levels to complete
            a.Add(new Accomplishment() { Name = "Level 1", Type = "Level" });
            a.Add(new Accomplishment() { Name = "Level 2", Type = "Level" });
            a.Add(new Accomplishment() { Name = "Level 3", Type = "Level" });

            Accomplishments = a;
            //MessageBox.Show("Got accomplishments from default");
        }


        public void GetSavedAccomplishments()
        {
            ObservableCollection<Accomplishment> a = new ObservableCollection<Accomplishment>();

            foreach (Object o in IsolatedStorageSettings.ApplicationSettings.Values)
            {
                a.Add((Accomplishment)o);
            }

            Accomplishments = a;
            //MessageBox.Show("Got accomplishments from storage");
        }

        public void SaveAccomplishments()
        {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;

            foreach (Accomplishment a in Accomplishments)
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
            MessageBox.Show("Finished saving accomplishments");
        }
    }
}
