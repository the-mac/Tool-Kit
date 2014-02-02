using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Notification;
using System.Diagnostics;
using System.Windows.Threading;
using System.Windows.Media.Imaging;
using System.IO;
using System.Xml.Linq;
using System.Collections.ObjectModel;

namespace WPPushNotification.TestClient
{
    public partial class CityPage : PhoneApplicationPage
    {
        // Constructor
        public CityPage()
        {
            InitializeComponent();            
        }


        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {            
            DataContext = (App.Current as App).Locations[NavigationContext.QueryString["location"]];

            // Request the latest data, as the data we have may not be the latest.
            (App.Current as App).RequestLatestData(NavigationContext.QueryString["location"]);            

            base.OnNavigatedTo(e);
        }        
    }
}