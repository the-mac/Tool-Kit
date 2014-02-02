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
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Phone.Controls;

namespace sdkMulticastCS
{
    /// <summary>
    /// This class handles all UI interaction for the game. 
    /// It also registers for events from the GamePlay class so that it
    /// can update the UI if a game-specific event occurs.
    /// </summary>
    public partial class Game : PhoneApplicationPage
    {
        // Both players in a game choose a rock, paper or scissors.
        // Store the choices in these variables
        private string _myChoice = string.Empty;
        private string _opponentsChoice = string.Empty;

        // The player names
        private string _player = null;
        private string _opponent = null;

        public Game()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            
            // Initialize the game
            NavigationContext.QueryString.TryGetValue("player", out _player);
            NavigationContext.QueryString.TryGetValue("opponent", out _opponent);

            if (App.GamePlay.CurrentOpponent == null)
            {
                NavigationService.Navigate(new Uri("/Players.xaml?player=" + _player, UriKind.RelativeOrAbsolute));
            }

            MyScoreName.Text = _player;
            MyScoreValue.Text = "0";
            OpponentScoreName.Text = _opponent;
            OpponentScoreValue.Text = "0";
            OpponentPiecesTextBlock.Text = _opponent;
            MyPiecesTextBlock.Text = _player;

            // No choices have been made yet
            _myChoice = String.Empty;
            _opponentsChoice = String.Empty;

            // Register for event notifications.
            RegisterEvents();
        }

        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);

            // Notify opponent (if one exists) that I have left the game
            if (e.NavigationMode == System.Windows.Navigation.NavigationMode.Back)
            {
                App.GamePlay.LeaveGame();
            }
            else
            {
                App.GamePlay.Leave(false);
            }

            UnregisterEvents();
        }

        private void RegisterEvents()
        {
            App.GamePlay.OpponentPlayed += new OpponentPlayedEventHandler(GamePlay_OpponentPlayed);
            App.GamePlay.NewGameRequested += new NewGameRequestedHandler(GamePlay_NewGameRequested);
            App.GamePlay.LeftGame += new LeftGameHandler(GamePlay_LeftGame);
        }

        private void UnregisterEvents()
        {
            if (App.GamePlay != null)
            {
                App.GamePlay.OpponentPlayed -= new OpponentPlayedEventHandler(GamePlay_OpponentPlayed);
                App.GamePlay.NewGameRequested -= new NewGameRequestedHandler(GamePlay_NewGameRequested);
                App.GamePlay.LeftGame -= new LeftGameHandler(GamePlay_LeftGame);
            }
        }

        void GamePlay_LeftGame(object sender, PlayerEventArgs e)
        {
            // If the opponent leaves the game, back out of the game screen, since we will want
            // to choose another opponent.
            DiagnosticsHelper.SafeShow(String.Format("Player '{0}' has left the game",e.playerInfo.PlayerName));

            // Opponent has left, so clear the opponent name
            _opponent = null;
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();

        }

        void GamePlay_NewGameRequested(object sender, PlayerEventArgs e)
        {
            // Either player can tap "Play Again" which sends a message to setup a new game
            // We encapsualte the setting up of a new game here.
            SetupNewGame();
        }

        void GamePlay_OpponentPlayed(object sender, OpponentPlayedEventArgs e)
        {
            // We received notification that the opponent has chosen a gamepiece (rock, paper, scissors)
            // The event argument contains the piece the opponent chose.
            _opponentsChoice = e.gamepiece;

            // If I have already chosen, then we are ready to see who won
            if (!String.IsNullOrWhiteSpace(_myChoice))
            {
                // Color the opponent's gamepiece
                SelectGamePiece(OpponentGamePieces, _opponentsChoice);
                DetermineWinner();
            }
        }

        /// <summary>
        /// Change the visual representation (style) of the gamepiece that was chosen
        /// </summary>
        /// <param name="stackPanel">The gamepieces are organized into two StackPanels. This parameter contains the 
        /// StackPanel, or gamepiece set, to change. It can be our set, or the opponent's.</param>
        /// <param name="tag">Each gamepiece has the tag property set uniquely to identify each.</param>
        private void SelectGamePiece(StackPanel stackPanel, string tag)
        {
            foreach (Border border in stackPanel.Children.OfType<Border>())
            {
                if (border.Child.GetValue(TagProperty).ToString() == tag)
                {
                    border.BorderBrush = (Brush)Resources["PhoneAccentBrush"];
                    border.Background = (Brush)Resources["PhoneAccentBrush"];
                }
                else
                {
                    border.BorderBrush = new SolidColorBrush(Colors.Transparent);
                    border.Background = new SolidColorBrush(Colors.Transparent);
                }
            }
        }

        /// <summary>
        /// Clear the visual representation of all gamepieces in a set
        /// </summary>
        /// <param name="stackPanel">he gamepieces are organized into two StackPanels. This parameter contains the 
        /// StackPanel, or gamepiece set, to change. It can be our set, or the opponent's.</param>
        private void ClearGamePieces(StackPanel stackPanel)
        {
            foreach (Border border in stackPanel.Children.OfType<Border>())
            {
                border.BorderBrush = new SolidColorBrush(Colors.Transparent);
                border.Background = new SolidColorBrush(Colors.Transparent);
            }
        }

        
        private void GamePiece_Click(object sender, RoutedEventArgs e)
        {
            // Don't allow me to change my mind. Once a gamepiece is selected (clicked) we are locked in for this round.
            MyGamePieces.IsHitTestVisible = false;
            Button button = sender as Button;

            // The tag on the button identifies the piece (that is, rock, paper or scissors)
            _myChoice = button.Tag.ToString();
            SelectGamePiece(MyGamePieces, _myChoice);

            // Tell the opponent I am ready to play, and send my selection
            App.GamePlay.Play(_myChoice);

            // The opponet may have played already. If that is the case, the _opponentsChoice field
            // will contain their choice.
            if (!String.IsNullOrWhiteSpace(_opponentsChoice))
            {
                // Color the opponent's gamepiece
                SelectGamePiece(OpponentGamePieces, _opponentsChoice);

                // Sicne we have both played, find out who won.
                DetermineWinner();
            }

        }

        void DetermineWinner()
        {
            // Both players must have played
            if (_myChoice != String.Empty && _opponentsChoice != String.Empty)
            {
                // Draw if we chose the same gamepiece
                if (_myChoice == _opponentsChoice)
                {
                    // Draw
                    ResultTextBlock.Text = "Draw!";
                }
                else
                {
                    
                    switch (_myChoice)
                    {
                        case "PAPER":
                            // Paper beats Rock
                            if (_opponentsChoice == "ROCK")
                            {
                                // You win
                                ResultTextBlock.Text = "You Win!";
                                MyScoreValue.Text = (Int32.Parse(MyScoreValue.Text) + 1).ToString();
                            }
                            else
                            {
                                // Opponent Wins
                                ResultTextBlock.Text = "Opponent Wins!";
                                OpponentScoreValue.Text = (Int32.Parse(OpponentScoreValue.Text) + 1).ToString();
                            }
                            break;
                        case "ROCK":
                            // Rock beats scissors
                            if (_opponentsChoice == "SCISSORS")
                            {
                                // You win
                                ResultTextBlock.Text = "You Win!";
                                MyScoreValue.Text = (Int32.Parse(MyScoreValue.Text) + 1).ToString();
                            }
                            else
                            {
                                // Opponent Wins
                                ResultTextBlock.Text = "Opponent Wins!";
                                OpponentScoreValue.Text = (Int32.Parse(OpponentScoreValue.Text) + 1).ToString();
                            }
                            break;
                        case "SCISSORS":
                            // Scissors beats paper
                            if (_opponentsChoice == "PAPER")
                            {
                                // You win
                                ResultTextBlock.Text = "You Win!";
                                MyScoreValue.Text = (Int32.Parse(MyScoreValue.Text) + 1).ToString();
                            }
                            else
                            {
                                // Opponent Wins
                                ResultTextBlock.Text = "Opponent Wins!";
                                OpponentScoreValue.Text = (Int32.Parse(OpponentScoreValue.Text) + 1).ToString();
                            }
                            break;
                        default:
                            // ??
                            break;
                    }
                }

                _opponentsChoice = _myChoice = String.Empty;
                PlayAgainButton.Visibility = System.Windows.Visibility.Visible;
                
            }
        }

        private void PlayAgainButton_Click(object sender, RoutedEventArgs e)
        {
            SetupNewGame();
            App.GamePlay.NewGame();

        }

        private void SetupNewGame()
        {
            PlayAgainButton.Visibility = System.Windows.Visibility.Collapsed;
            ClearGamePieces(MyGamePieces);
            ClearGamePieces(OpponentGamePieces);
            ResultTextBlock.Text = string.Empty;
            MyGamePieces.IsHitTestVisible = true;
        }

        private void PhoneApplicationPage_Unloaded(object sender, RoutedEventArgs e)
        {
            UnregisterEvents();
        }

    }
}
