/* 
    Copyright (c) 2012 - 2013 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://code.msdn.microsoft.com/wpapps
  
*/
using Microsoft.Phone.Controls;
using System.Windows;

namespace ControlsCatalog.ButtonAndSelection
{
    public partial class RadioButtonDemo : PhoneApplicationPage
    {
        public RadioButtonDemo()
        {
            InitializeComponent();
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (Radio1 != null && Radio2 != null && Radio3 != null)
            {
                if (Radio1.IsChecked == true)
                {
                    MessageBox.Show("You selected Radio1!");
                    RadioText.Text = "Radio1 selected.";
                }
                if (Radio2.IsChecked == true)
                {
                    MessageBox.Show("You selected Radio2!");
                    RadioText.Text = "Radio2 selected.";
                }
                if (Radio3.IsChecked == true)
                {
                    MessageBox.Show("You selected Radio3!");
                    RadioText.Text = "Radio3 selected.";
                }
            }
        }


    }
}
