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
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace sdkAppBarCS
{
    public partial class PlainPage : PhoneApplicationPage
    {
        public PlainPage()
        {
            InitializeComponent();

            //Set the initial values for the Application Bar properties by checking the radio buttons.
            ForeNormal.IsChecked = true;
            BackNormal.IsChecked = true; 
            One.IsChecked = true;
            DefaultSize.IsChecked = true;
            Visible.IsChecked = true;
            Enabled.IsChecked = true;
        }

        private void ForeColorChanged(object sender, RoutedEventArgs e)
        {
            String option = ((RadioButton)sender).Name;
            switch (option)
            {
                case "ForeNormal":
                    ApplicationBar.ForegroundColor = (Color)Resources["PhoneForegroundColor"];
                    break;

                case "ForeAccent":
                    ApplicationBar.ForegroundColor = (Color)Resources["PhoneAccentColor"];
                    break;
            }
        }

        private void BackColorChanged(object sender, RoutedEventArgs e)
        {
            String option = ((RadioButton)sender).Name;
            switch (option)
            {
                case "BackNormal":
                    ApplicationBar.BackgroundColor = new Color() {A=0, R=0, G=0, B=0};
                    break;

                case "BackAccent":
                    ApplicationBar.BackgroundColor= (Color)Resources["PhoneAccentColor"];
                    break;
            }
        }
        
        private void OpacityChanged(object sender, RoutedEventArgs e)
        {
            String option = ((RadioButton)sender).Name;
            switch (option)
            {
                case "One":
                    ApplicationBar.Opacity = 1.0;
                    break;

                case "Half":
                    ApplicationBar.Opacity = 0.5;
                    break;

                case "Zero":
                    ApplicationBar.Opacity = 0.0;
                    break;
            }
        }

        private void ModeChanged(object sender, RoutedEventArgs e)
        {
            String option = ((RadioButton)sender).Name;
            switch (option)
            {
                case "DefaultSize":
                    ApplicationBar.Mode = ApplicationBarMode.Default;
                    break;

                case "Mini":
                    ApplicationBar.Mode = ApplicationBarMode.Minimized;
                    break;
            }
        }

        private void VisibilityChanged(object sender, RoutedEventArgs e)
        {
            String option = ((RadioButton)sender).Name;
            switch (option)
            {
                case "Visible":
                    ApplicationBar.IsVisible = true;
                    break;

                case "Hidden":
                    ApplicationBar.IsVisible = false;
                    break;
            }
        }

        private void MenuEnabledChanged(object sender, RoutedEventArgs e)
        {
            String option = ((RadioButton)sender).Name;
            switch (option)
            {
                case "Enabled":
                    ApplicationBar.IsMenuEnabled = true;
                    break;

                case "Disabled":
                    ApplicationBar.IsMenuEnabled = false;
                    break;
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Button 1 works!  Do something useful in your application.");
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Button 2 works!  Do something useful in your application.");
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Button 3 works!  Do something useful in your application.");
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Button 4 works!  Do something useful in your application.");
        }

        private void MenuItem1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("The default Application Bar size is " + ApplicationBar.DefaultSize + " pixels.");
        }

        private void MenuItem2_Click(object sender, EventArgs e)
        {
            MessageBox.Show("The mini Application Bar size is " + ApplicationBar.MiniSize + " pixels.");
        }

    }//page class
}//namespace
