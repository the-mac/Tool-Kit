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
    public partial class ApplicationTile : PhoneApplicationPage
    {
        public ApplicationTile()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Set all the properties of the Application Tile.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSetApplicationTile_Click(object sender, RoutedEventArgs e)
        {
            int newCount = 0;

            // Application Tile is always the first Tile, even if it is not pinned to Start.
            ShellTile TileToFind = ShellTile.ActiveTiles.First();

            // Application should always be found
            if (TileToFind != null)
            {
                // if Count was not entered, then assume a value of 0
                if (textBoxCount.Text == "")
                {
                    // A value of '0' means do not display the Count.
                    newCount = 0;
                }
                // otherwise get the numerical value for Count
                else
                {
                    newCount = int.Parse(textBoxCount.Text);
                }

                // set the properties to update for the Application Tile
                // Empty strings for the text values and URIs will result in the property being cleared.
                StandardTileData NewTileData = new StandardTileData
                {
                    Title = textBoxTitle.Text,
                    BackgroundImage = new Uri(textBoxBackgroundImage.Text, UriKind.Relative),
                    Count = newCount,
                    BackTitle = textBoxBackTitle.Text,
                    BackBackgroundImage = new Uri(textBoxBackBackgroundImage.Text, UriKind.Relative),
                    BackContent = textBoxBackContent.Text
                };

                // Update the Application Tile
                TileToFind.Update(NewTileData);
            }


        }

    }
}
