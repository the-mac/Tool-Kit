using System;
using System.Windows;
using Microsoft.Phone.Controls;

namespace sdkRawSensorDataCS
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        private void CompassButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/CompassPage.xaml", UriKind.Relative));
        }

        private void AccelerometerButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/AccelerometerPage.xaml", UriKind.Relative));
        }

        private void GyroscopeButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/GyroscopePage.xaml", UriKind.Relative));
        }
    }
}