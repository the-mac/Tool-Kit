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
using Microsoft.Phone.Controls;

namespace sdkMulticastCS
{
    public partial class Players : PhoneApplicationPage
    {
        /// <summary>
        /// The current player.
        /// </summary>
        private string _player = String.Empty;

        public Players()
        {
            InitializeComponent();

            // The players listbox control is bound to the players list.
            this.DataContext = App.Players;

        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (NavigationContext.QueryString.TryGetValue("player", out _player))
            {
                App.GamePlay.Join(_player);
            }

            PlayersListBox.SelectedItem = null;
        }

        private void ChallengButton_Click(object sender, RoutedEventArgs e)
        {
            if (PlayersListBox.SelectedItem == null)
            {
                DiagnosticsHelper.SafeShow("Please select an opponent from the list");
            }
            else
            {
                string opponentName = ((PlayerInfo)PlayersListBox.SelectedItem).PlayerName;
                App.GamePlay.Challenge(opponentName);
                
            }
        }

        private void PlayersListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Note that I cannot select myself in this list.
            if (e.AddedItems.Count > 0 && ((PlayerInfo)e.AddedItems[0]).PlayerName == _player)
            {
                PlayersListBox.SelectedItem = null;
            }
        }
    }
}
