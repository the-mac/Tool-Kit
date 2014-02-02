/****************************** Module Header ******************************\
* Module Name:    DataView1.xaml.cs
* Project:        CSWP8AccessItemIndex
* Copyright (c) Microsoft Corporation
*
* This view will demo how to use custom Converter to alternate rows' background.
* In the Converter we will use a custom method to get the index of the data item.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\*****************************************************************************/

using System.Windows;
using System.Windows.Controls;

namespace CSWP8AccessItemIndex.Views
{
    public partial class DataView1 : UserControl
    {
        public DataView1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Tap the data item, pop-up the index of the item.
        /// </summary>
        /// <param name="sender">Item of Item ItemsControl</param>
        /// <param name="e">GestureEventArgs</param>
        private void tbName_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            int intIndex = -1;                      // index
            TextBlock element = sender as TextBlock;// current element

            intIndex = Utilities.GetIndexOfUIElement(element);

            MessageBox.Show("The index of the Tap item is:" + intIndex.ToString());
        }
    }
}
