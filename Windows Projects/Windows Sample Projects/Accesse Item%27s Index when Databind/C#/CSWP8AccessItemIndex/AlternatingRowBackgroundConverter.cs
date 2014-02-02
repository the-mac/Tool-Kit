/****************************** Module Header ******************************\
* Module Name:    AlternatingRowBackgroundConverter.cs
* Project:        CSWP8AccessItemIndex
* Copyright (c) Microsoft Corporation
*
* This is a custom Converter. We will use it to alternate the row's background.
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
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
namespace CSWP8AccessItemIndex
{
    public class AlternatingRowBackgroundConverter : IValueConverter
    {
        public Brush NormalBrush { get; set; }    // Normal Brush
        public Brush AlternateBrush { get; set; } // Alternate Brush

        /// <summary>
        /// Deal with the background brush of current UIElement element.
        /// </summary>
        /// <param name="value">object</param>
        /// <param name="targetType">Type</param>
        /// <param name="parameter">parameter</param>
        /// <param name="culture">CultureInfo</param>
        /// <returns></returns>
        public object Convert(
            object value, Type targetType, object parameter, CultureInfo culture)
        {
            Panel element = (Panel)value;
            element.Loaded += Element_Loaded;

            return NormalBrush;
        }

        public object ConvertBack(
            object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the index of UIElement element and in accordance with the index to
        /// perform background color alternating.
        /// </summary>
        /// <param name="sender">UIElement</param>
        /// <param name="e"></param>
        private void Element_Loaded(object sender, RoutedEventArgs e)
        {
            int intIndex = -1;               // index
            Panel element = sender as Panel; // Current UIElement          

            intIndex = Utilities.GetIndexOfUIElement(element);

            if (intIndex != (-1))
            {
                if (intIndex % 2 != 0)
                    element.Background = AlternateBrush;
            }
        }
    }
}
