/****************************** Module Header ******************************\
* Module Name:    ReadinessAndLevel.cs
* Project:        CSWP8CollectionViewSource
* Copyright (c) Microsoft Corporation
*
* This class is the data model.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\*****************************************************************************/
using System.ComponentModel;

namespace CSWP8CollectionViewSource.Model
{
    public class ReadinessAndLevel : INotifyPropertyChanged
    {
        // The name of the readinessAndLevels.
        public string Name { get; set; }

        // The type of the readinessAndLevels, Item or Level.
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

        // Create a copy of an readinessAndLevels to save.
        // If your object is databound, this copy is not databound.
        public ReadinessAndLevel GetCopy()
        {
            ReadinessAndLevel copy = (ReadinessAndLevel)this.MemberwiseClone();
            return copy;
        }
    }
}
