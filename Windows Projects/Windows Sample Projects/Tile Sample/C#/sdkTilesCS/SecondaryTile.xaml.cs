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
using System.Linq;
using System.Windows;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace sdkTilesCS
{
    public partial class SecondaryTile : PhoneApplicationPage
    {
        public SecondaryTile()
        {
            InitializeComponent();
        }


        /// <summary>
        /// Event handler for when this page is navigated to.  Looks to see
        /// if the tile exists and set the check box appropriately.
        /// Also fills in the default value for the Title, based on the
        /// value passed in in the QueryString - either FromMain or FromTile.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // See if the tile is pinned, and if so, make sure the checkbox for it is checked.
            // (User may have deleted it manually.)
            ShellTile TileToFind = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains("DefaultTitle=FromTile"));

            checkBoxDisplaySecondaryTile.IsChecked = (TileToFind != null);

            // To demonstrate the use of the Navigation URI and query parameters, we set the default value
            // of the Title textbox based on where we navigated from.  If we navigated to this page
            // from the MainPage, the DefaultTitle parameter will be "FromMain".  If we navigated here
            // when the secondary Tile was tapped, the parameter will be "FromTile".
            textBoxTitle.Text = this.NavigationContext.QueryString["DefaultTitle"];

        }

        /// <summary>
        /// Event handler for when the checkbox is checked. Create a secondary tile if it doesn't
        /// already exist.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxDisplaySecondaryTile_Checked(object sender, RoutedEventArgs e)
        {
            // Look to see if the tile already exists and if so, don't try to create again.
            ShellTile TileToFind = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains("DefaultTitle=FromTile"));

            // Create the tile if we didn't find it already exists.
            if (TileToFind == null)
            {
                // Create the tile object and set some initial properties for the tile.
                // The Count value of 12 will show the number 12 on the front of the Tile. Valid values are 1-99.
                // A Count value of 0 will indicate that the Count should not be displayed.
                StandardTileData NewTileData = new StandardTileData
                {
                    BackgroundImage = new Uri("Red.jpg", UriKind.Relative),
                    Title = "Secondary Tile",
                    Count = 12,
                    BackTitle = "Back of Tile",
                    BackContent = "Welcome to the back of the Tile",
                    BackBackgroundImage = new Uri("Blue.jpg", UriKind.Relative)
                };

                // Create the tile and pin it to Start. This will cause a navigation to Start and a deactivation of our application.
                ShellTile.Create(new Uri("/SecondaryTile.xaml?DefaultTitle=FromTile", UriKind.Relative), NewTileData);
            }

        }

        /// <summary>
        /// Event handler for when the checkbox is unchecked.  Delete the secondary tile
        /// if it exists.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxDisplaySecondaryTile_Unchecked(object sender, RoutedEventArgs e)
        {
            // Find the tile we want to delete.
            ShellTile TileToFind = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains("DefaultTitle=FromTile"));

            // If tile was found, then delete it.
            if (TileToFind != null)
            {
                TileToFind.Delete();
            }

        }

        /// <summary>
        /// Handle the Title button clicked event by setting the front of Tile title.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSetTitle_Click(object sender, RoutedEventArgs e)
        {
            // Find the tile we want to update.
            ShellTile TileToFind = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains("DefaultTitle=FromTile"));

            // If tile was found, then update the Title
            if (TileToFind != null)
            {
                StandardTileData NewTileData = new StandardTileData
                {
                    Title = textBoxTitle.Text
                };

                TileToFind.Update(NewTileData);
            }

        }

        /// <summary>
        /// Handle the Background Image button clicked event by setting the front of Tile background image.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSetBackgroundImage_Click(object sender, RoutedEventArgs e)
        {
            // Find the tile we want to update.
            ShellTile TileToFind = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains("DefaultTitle=FromTile"));

            // If tile was found, then update the background image.
            if (TileToFind != null)
            {
                StandardTileData NewTileData = new StandardTileData
                {
                    BackgroundImage = new Uri(textBoxBackgroundImage.Text, UriKind.Relative)
                };

                TileToFind.Update(NewTileData);
            }

        }

        /// <summary>
        /// Handle the Count button clicked event by setting the front of Tile count value.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSetCount_Click(object sender, RoutedEventArgs e)
        {
            int newCount = 0;

            // Find the tile we want to update.
            ShellTile TileToFind = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains("DefaultTitle=FromTile"));

            // If tile was found, then update the count.
            if (TileToFind != null)
            {
                // if Count was not entered, then assume a value of 0
                if (textBoxCount.Text == "")
                {
                    newCount = 0;
                }
                // otherwise get the numerical value for Count
                else
                {
                    newCount = int.Parse(textBoxCount.Text);
                }

                StandardTileData NewTileData = new StandardTileData
                {
                    Count = newCount
                };

                TileToFind.Update(NewTileData);
            }

        }

        /// <summary>
        /// Handle the Back Title button clicked event by setting the back of Tile title.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSetBackTitle_Click(object sender, RoutedEventArgs e)
        {
            // Find the tile we want to update.
            ShellTile TileToFind = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains("DefaultTitle=FromTile"));

            // If tile was found, then update the title on the back of the tile
            if (TileToFind != null)
            {
                StandardTileData NewTileData = new StandardTileData
                {
                    BackTitle = textBoxBackTitle.Text
                };

                TileToFind.Update(NewTileData);
            }

        }

        /// <summary>
        /// Handle the Back Background Image button clicked event by setting the back of Tile background image.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSetBackBackgroundImage_Click(object sender, RoutedEventArgs e)
        {
            // Find the tile we want to update.
            ShellTile TileToFind = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains("DefaultTitle=FromTile"));

            // If tile was found, then update the background image on the back of the tile.
            if (TileToFind != null)
            {
                StandardTileData NewTileData = new StandardTileData
                {
                    BackBackgroundImage = new Uri(textBoxBackBackgroundImage.Text, UriKind.Relative)
                };

                TileToFind.Update(NewTileData);
            }

        }

        /// <summary>
        /// Handle the Back Content button clicked event by setting the back of Tile content.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSetBackContent_Click(object sender, RoutedEventArgs e)
        {
            // Find the tile we want to update.
            ShellTile TileToFind = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains("DefaultTitle=FromTile"));

            // If tile was found, then update the content on the back of the tile.
            if (TileToFind != null)
            {
                StandardTileData NewTileData = new StandardTileData
                {
                    BackContent = textBoxBackContent.Text
                };

                TileToFind.Update(NewTileData);
            }

        }


    }
}
