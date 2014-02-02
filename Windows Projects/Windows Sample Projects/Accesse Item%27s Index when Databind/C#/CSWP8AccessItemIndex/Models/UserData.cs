/****************************** Module Header ******************************\
* Module Name:    UserData.cs
* Project:        CSWP8AccessItemIndex
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
using System.Windows.Media;

namespace CSWP8AccessItemIndex.Models
{
    public class UserData : INotifyPropertyChanged
    {
        // Constructor
        public UserData(int i)
        {
            this.Index = i;
            this.Name = "Name" + (this.Index + 1);
            this.Age = i + 5;
        }

        // id
        private int _index;
        public int Index
        {
            get { return _index; }
            set
            {
                _index = value;
                RaisePropertyChanged("Index");
            }
        }

        // name
        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                RaisePropertyChanged("Name");
            }
        }

        // age
        private int _age;
        public int Age
        {
            get { return _age; }
            set
            {
                _age = value;
                RaisePropertyChanged("Age");
            }
        }

        // Background Brush
        public SolidColorBrush BackgroundBrush
        {
            get
            {
                int index = App.ViewModel.UserDatas.IndexOf(this);
                Color backgroundColor = (index % 2 == 0) ? Colors.Gray : Colors.Blue;
                return new SolidColorBrush(backgroundColor);
            }
            set { }
        }

        // PropertyChangedEventHandler
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }
    }
}
