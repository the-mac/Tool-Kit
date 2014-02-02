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
using System.ComponentModel;

namespace sdkMVVMCS.Model
{
    public class Accomplishment : INotifyPropertyChanged
    {
        // The name of the accomplishment.
        public string Name { get; set; }

        // The type of the accomplishment, Item or Level.
        public string Type { get; set; }

        // The number of each item that has been collected.
        private int _count;
        public int Count
        {
            get
            {
                return _count;
            }
            set
            {
                _count = value;
                RaisePropertyChanged("Count");
            }
        }

        // Whether a level has been completed or not
        private bool _completed;
        public bool Completed
        {
            get
            {
                return _completed;
            }
            set
            {
                _completed = value;
                RaisePropertyChanged("Completed");
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        // Create a copy of an accomplishment to save.
        // If your object is databound, this copy is not databound.
        public Accomplishment GetCopy()
        {
            Accomplishment copy = (Accomplishment)this.MemberwiseClone();
            return copy;
        }
    }
}
