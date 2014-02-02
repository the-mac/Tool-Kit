/****************************** Module Header ******************************\
* Module Name:    LevelView.xaml.cs
* Project:        CSWP8CollectionViewSource
* Copyright (c) Microsoft Corporation
*
* This is the view that type is equal to level.
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
using System.Windows.Controls;
using System.Globalization;

namespace CSWP8CollectionViewSource.View
{
    public partial class LevelView : UserControl
    {
        public LevelView()
        {
            InitializeComponent();
        }
    }

    /// <summary>
    /// This is the custom data binding converter
    /// </summary>
    public class BoolOpposite : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool b = (bool)value;
            return !b;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string s = value as string;
            bool b;

            if (bool.TryParse(s, out b))
            {
                return !b;
            }
            return false;
        }
    }

}
