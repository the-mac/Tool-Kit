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
using Microsoft.Phone.Shell;

namespace WPPushNotification.TestClient
{
    public partial class MainPage : PhoneApplicationPage
    {
        /// <summary>
        /// Contains information about the locations displayed by the application.
        /// </summary>
        public Dictionary<string, LocationInformation> Locations { get; set; }

        // Constructor
        public MainPage()
        {
            InitializeComponent();            
        }

		#region Navigation
		protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
		{			
            Locations = (App.Current as App).Locations;

			base.OnNavigatedTo(e);
		}
		#endregion

		#region Misc logic
		/// <summary>
		/// Creates a Uri leading to the location specified by the location information to be bound to a tile.
		/// </summary>
		/// <param name="locationInformation">The location information for which to generate the Uri.</param>
		/// <returns>Uri for the page displaying information about the provided location.</returns>
		private static Uri MakeTileUri(LocationInformation locationInformation)
		{
			return new Uri(Uri.EscapeUriString(String.Format("/CityPage.xaml?location={0}",
			   locationInformation.Name)), UriKind.Relative);
		}
		#endregion

        #region UI event handlers
        private void UnpinItem_Click(object sender, RoutedEventArgs e)
        {
        }

        private void PinItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 0)
            {
                (sender as ListBox).SelectedIndex = -1;
                NavigationService.Navigate(MakeTileUri(e.AddedItems[0] as LocationInformation));
            }
        }

        private void ChangeMainTile_Click(object sender, RoutedEventArgs e)
        {
        }
        #endregion        
    }
}