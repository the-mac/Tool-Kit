/****************************** Module Header ******************************\
* Module Name:    ViewModel.cs
* Project:        CSWP8AccessItemIndex
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

using CSWP8AccessItemIndex.Models;
using System.Collections.ObjectModel;

namespace CSWP8AccessItemIndex
{
    public class ViewModel
    {
        public ViewModel()
        {
            // Instantiated.
            this.UserDatas = new ObservableCollection<UserData>();

            // Insert test data.
            UserDatas.Add(new UserData(8));
            UserDatas.Add(new UserData(6));
            UserDatas.Add(new UserData(2));
            UserDatas.Add(new UserData(3));
            UserDatas.Add(new UserData(4));
            UserDatas.Add(new UserData(5));
            UserDatas.Add(new UserData(9));
            UserDatas.Add(new UserData(7));
            UserDatas.Add(new UserData(1));
            UserDatas.Add(new UserData(10));
            UserDatas.Add(new UserData(12));
            UserDatas.Add(new UserData(13));
        }

        // Collection of UserData
        private ObservableCollection<UserData> _userDatas;
        public ObservableCollection<UserData> UserDatas
        {
            get { return _userDatas; }
            set
            {
                _userDatas = value;
            }
        }
    }
}