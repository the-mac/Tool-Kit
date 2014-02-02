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
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;

namespace sdkMulticastCS
{
    /// <summary>
    /// This class handles all game communication. Communication for the game is made up of
    /// a number of commands that we have defined in the GameCommand.cs class. These commands 
    /// are the grammar, or set of actions, that we can transmist and receive and interpret.
    /// </summary>
    public class GamePlay : IDisposable
    {
        /// <summary>
        /// All communication takes place using a UdpAnySourceMulticastChannel. 
        /// A UdpAnySourceMulticastChannel is a wrapper we create around the UdpAnySourceMulticastClient.
        /// </summary>
        /// <value>The channel.</value>
        private UdpAnySourceMulticastChannel Channel { get; set; }

        /// <summary>
        /// The IP address of the multicast group. 
        /// </summary>
        /// <remarks>
        /// A multicast group is defined by a multicast group address, which is an IP address 
        /// that must be in the range from 224.0.0.0 to 239.255.255.255. Multicast addresses in 
        /// the range from 224.0.0.0 to 224.0.0.255 inclusive are “well-known” reserved multicast 
        /// addresses. For example, 224.0.0.0 is the Base address, 224.0.0.1 is the multicast group 
        /// address that represents all systems on the same physical network, and 224.0.0.2 represents 
        /// all routers on the same physical network.The Internet Assigned Numbers Authority (IANA) is 
        /// responsible for this list of reserved addresses. For more information on the reserved 
        /// address assignments, please see the IANA website.
        /// http://go.microsoft.com/fwlink/?LinkId=221630
        /// </remarks>
        private const string GROUP_ADDRESS = "224.0.1.11";

        /// <summary>
        /// This defines the port number through which all communication with the multicast group will take place. 
        /// </summary>
        /// <remarks>
        /// The value in this example is arbitrary and you are free to choose your own.
        /// </remarks>
        private const int GROUP_PORT = 54329;
        
        /// <summary>
        /// The name of this player.
        /// </summary>
        private string _playerName;

        public GamePlay()
        {
            this.Channel = new UdpAnySourceMulticastChannel(GROUP_ADDRESS, GROUP_PORT);

            // Register for events on the multicast channel.
            RegisterEvents();

            // Send a message to the multicast group regularly. This is done because
            // we use UDP unicast messages during the game, sending messages directly to 
            // our opponent. This uses the BeginSendTo method on UpdAnySourceMulticastClient
            // and according to the documentation:
            // "The transmission is only allowed if the address specified in the remoteEndPoint
            // parameter has already sent a multicast packet to this receiver"
            // So, if everyone sends a message to the multicast group, we are guaranteed that this 
            // player (receiver) has been sent a multicast packet by the opponent. 
            StartKeepAlive();
        }

        #region Properties
        /// <summary>
        /// The player, against whom, I am currently playing.
        /// </summary>
        private PlayerInfo _currentOpponent;
        public PlayerInfo CurrentOpponent
        {
            get
            {
                return _currentOpponent;
            }
        }

        /// <summary>
        /// Whether we are joined to the multicast group
        /// </summary>
        public bool IsJoined { get; private set; }
        #endregion

        #region Game Actions
        /// <summary>
        /// Join the multicast group.
        /// </summary>
        /// <param name="playerName">The player name I want to join as.</param>
        /// <remarks>The player name is not needed for multicast communication. it is 
        /// used in this example to identify each member of the multicast group with 
        /// a friendly name. </remarks>
        public void Join(string playerName)
        {
            if (IsJoined)
            {
                return;
            }

            // Store my player name
            _playerName = playerName;

            //Open the connection
            this.Channel.Open();
        }

        /// <summary>
        /// Send a message to the given opponent to challenge him to a game.
        /// </summary>
        /// <param name="opponentName">The identifier for the opponent to challenge.</param>
        public void Challenge(string opponentName)
        {
            // Look for this opponent in the list of oppoennts in the group
            PlayerInfo opponent = App.Players.Where(player => player.PlayerName == opponentName).SingleOrDefault();
            if (opponent != null)
            {
                this.Channel.SendTo(opponent.PlayerEndPoint, GameCommands.ChallengeFormat, _playerName);
            }
            else
            {
                DiagnosticsHelper.SafeShow("Opponent is null!");
            }
        }

        /// <summary>
        /// Inform the given opponent that we accept his challenge to play.
        /// </summary>
        /// <param name="opponent">The opponent</param>
        public void AcceptChallenge(PlayerInfo opponent)
        {
            if (opponent != null)
            {
                _currentOpponent = opponent;
                this.Channel.SendTo(_currentOpponent.PlayerEndPoint, GameCommands.AcceptChallengeFormat, _playerName);
            }
        }

        /// <summary>
        /// Reject the challenge from the given opponent.
        /// </summary>
        /// <param name="opponent">The opponent</param>
        public void RejectChallenge(PlayerInfo opponent)
        {
            if (opponent != null)
            {
                this.Channel.SendTo(opponent.PlayerEndPoint, GameCommands.RejectChallengeFormat, _playerName);
            }
        }

        /// <summary>
        /// Leave the multicast group. We will not show up in the list of opponents on any
        /// other client devices.
        /// </summary>
        public void Leave(bool disconnect)
        {
            if (this.Channel != null)
            {
                // Tell everyone we have left
                this.Channel.Send(GameCommands.LeaveFormat, _playerName);

                // Only close the underlying communications channel to the multicast group
                // if disconnect == true.
                if (disconnect)
                {
                    this.Channel.Close();
                }
                
            }

            // Clear the opponent
            _currentOpponent = null;
            this.IsJoined = false;
        }

        /// <summary>
        /// Leave the current game
        /// </summary>
        public void LeaveGame()
        {
            if (this.Channel != null && _currentOpponent != null)
            {
                // Tell the opponent
                this.Channel.SendTo(_currentOpponent.PlayerEndPoint, GameCommands.LeaveGameFormat, _playerName);
                _currentOpponent = null;
            }
        }

        /// <summary>
        /// Tell the opponent what our choice is for this current game
        /// </summary>
        /// <param name="gamepiece">A Rock, Paper or Scissors</param>
        public void Play(string gamepiece)
        {
            if (this.Channel != null)
            {
                // Tell the opponent
                this.Channel.SendTo(_currentOpponent.PlayerEndPoint, GameCommands.PlayFormat, _playerName, gamepiece);
            }
        }

        /// <summary>
        /// Tell the opponent we want to start a new game
        /// </summary>
        public void NewGame()
        {
            if (this.Channel != null && _currentOpponent != null)
            {
                // Tell the opponent
                this.Channel.SendTo(_currentOpponent.PlayerEndPoint, GameCommands.NewGameFormat, _playerName);
            }
        }
        #endregion

        #region Multicast Communication
        /// <summary>
        /// Register for events on the multicast channel.
        /// </summary>
        private void RegisterEvents()
        {
            // Register for events from the multicast channel
            this.Channel.Joined += new EventHandler(Channel_Joined);
            this.Channel.BeforeClose += new EventHandler(Channel_BeforeClose);
            this.Channel.PacketReceived += new EventHandler<UdpPacketReceivedEventArgs>(Channel_PacketReceived);
        }

        /// <summary>
        /// Unregister for events on the multicast channel
        /// </summary>
        private void UnregisterEvents()
        {
            if (this.Channel != null)
            {
                // Register for events from the multicast channel
                this.Channel.Joined -= new EventHandler(Channel_Joined);
                this.Channel.BeforeClose -= new EventHandler(Channel_BeforeClose);
                this.Channel.PacketReceived -= new EventHandler<UdpPacketReceivedEventArgs>(Channel_PacketReceived);
            }
        }
        /// <summary>
        /// Handles the BeforeClose event of the Channel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void Channel_BeforeClose(object sender, EventArgs e)
        {
            this.Channel.Send(String.Format(GameCommands.Leave, _playerName));
        }

        /// <summary>
        /// Handles the Joined event of the Channel.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void Channel_Joined(object sender, EventArgs e)
        {
            this.IsJoined = true;
            this.Channel.Send(GameCommands.JoinFormat, _playerName);
        }

        /// <summary>
        /// Handles the PacketReceived event of the Channel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SilverlightPlayground.UDPMulticast.UdpPacketReceivedEventArgs"/> instance containing the event data.</param>
        void Channel_PacketReceived(object sender, UdpPacketReceivedEventArgs e)
        {
            string message = e.Message.Trim('\0');
            string[] messageParts = message.Split(GameCommands.CommandDelimeter.ToCharArray());

            if (messageParts.Length == 2)
            {
                switch (messageParts[0])
                {
                    case GameCommands.Join:
                        OnPlayerJoined(new PlayerInfo(messageParts[1], e.Source));
                        break;
                    case GameCommands.AcceptChallenge:
                        OnChallengeAccepted(new PlayerInfo(messageParts[1], e.Source));
                        break;
                    case GameCommands.Challenge:
                        OnChallengeReceived(new PlayerInfo(messageParts[1], e.Source));
                        break;
                    case GameCommands.NewGame:
                        OnNewGame(new PlayerInfo(messageParts[1], e.Source));
                        break;
                    case GameCommands.Leave:
                        OnPlayerLeft(new PlayerInfo(messageParts[1], e.Source));
                        break;
                    case GameCommands.LeaveGame:
                        OnLeaveGame(new PlayerInfo(messageParts[1], e.Source));
                        break;
                    case GameCommands.RejectChallenge:
                        OnChallengeRejected(new PlayerInfo(messageParts[1], e.Source));
                        break;
                    case GameCommands.Ready:
                        Debug.WriteLine("Ready received");
                        break;
                    default:
                        break;
                }
                

            }
            else if (messageParts.Length == 3 && messageParts[0] == GameCommands.Play)
            {
                OpponentPlayedEventArgs args = new OpponentPlayedEventArgs();
                args.gamepiece = messageParts[2];
                args.playerInfo = new PlayerInfo(messageParts[1], e.Source);
                if (OpponentPlayed != null)
                {
                    OpponentPlayed(this, args);
                }
            }
            else
            {
                // Ignore messages that don't have the expected number of parts.
            }
        }
        #endregion

        #region Command Handlers - methods to handle each command that we receive
        /// <summary>
        /// Handle a player joining the multicast group.
        /// </summary>
        /// <param name="playerInfo">The player.</param>
        private void OnPlayerJoined(PlayerInfo playerInfo)
        {
            bool add = true;
            int numberAdded = 0;

            foreach (PlayerInfo pi in App.Players)
            {
                if (pi.PlayerName == playerInfo.PlayerName)
                {

                    pi.PlayerEndPoint = playerInfo.PlayerEndPoint;

                    add = false;
                    break;
                }
            }

            if (add)
            {
                numberAdded++;
                App.Players.Add(playerInfo);
            }

            // If any new players have been added, send out our join message again
            // to make sure we are added to their player list.
            if (numberAdded > 0)
            {
                this.Channel.Send(GameCommands.JoinFormat, _playerName);
            }

#if DEBUG
            Debug.WriteLine(" =========   PLAYERS =============");
            foreach (PlayerInfo pi in App.Players)
            {
                Debug.WriteLine(string.Format("{1} [{0}]", pi.PlayerName, pi.PlayerEndPoint));
            }
#endif
        }

        /// <summary>
        /// Handle a player leaving the multicast group.
        /// </summary>
        /// <param name="playerInfo">The player.</param>
        private void OnPlayerLeft(PlayerInfo playerInfo)
        {
            if (playerInfo.PlayerName != _playerName)
            {
                foreach (PlayerInfo pi in App.Players)
                {
                    if (pi.PlayerName == playerInfo.PlayerName)
                    {
                        App.Players.Remove(pi);
                        break;
                    }
                }
            }

            OnLeaveGame(playerInfo);
        }

        /// <summary>
        /// Handle a challenge from a player.
        /// </summary>
        /// <param name="playerInfo">The player.</param>
        private void OnChallengeReceived(PlayerInfo playerInfo)
        {
            if (playerInfo.PlayerName == _playerName)
                return;

            if (_currentOpponent != null)
            {
                // Automatically reject incoming challenge if I am already in a game
                RejectChallenge(playerInfo);
            }
            else
            {
                MessageBoxResult result = DiagnosticsHelper.SafeShow(String.Format("'{0}' is challenging you." + Environment.NewLine + "Ok to accept, Cancel to reject", playerInfo.PlayerName), "Incoming Challenge", MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
                {
                    AcceptChallenge(playerInfo);
                    string uriString = String.Format("/Game.xaml?player={0}&opponent={1}", _playerName, playerInfo.PlayerName);
                    (Application.Current as App).RootFrame.Navigate(new Uri(uriString, UriKind.RelativeOrAbsolute));
                }
                else
                {
                    RejectChallenge(playerInfo);
                }
            }
        }

        /// <summary>
        /// Handle a player accepting our challenge.
        /// </summary>
        /// <param name="playerInfo">The player.</param>
        private void OnChallengeAccepted(PlayerInfo playerInfo)
        {
            _currentOpponent = playerInfo;
            DiagnosticsHelper.SafeShow(String.Format("Player '{0}' Accepted Your Challenge", playerInfo.PlayerName));
            string uriString = String.Format("/Game.xaml?player={0}&opponent={1}", _playerName,playerInfo.PlayerName);
            (Application.Current as App).RootFrame.Navigate(new Uri(uriString, UriKind.RelativeOrAbsolute));
        }

        /// <summary>
        /// Handle a player rejecting our challenge.
        /// </summary>
        /// <param name="playerInfo">The player.</param>
        private void OnChallengeRejected(PlayerInfo playerInfo)
        {
            _currentOpponent = null;
            DiagnosticsHelper.SafeShow(String.Format("Player '{0}' Rejected Your Challenge", playerInfo.PlayerName));
        }

        /// <summary>
        /// Handle an opponent requesting a new game.
        /// </summary>
        /// <param name="playerInfo">The player.</param>
        private void OnNewGame(PlayerInfo playerInfo)
        {
            // Is the current opponent requesting a new game?
            if (_currentOpponent != null && playerInfo.PlayerName == _currentOpponent.PlayerName)
            {
                PlayerEventArgs args = new PlayerEventArgs();
                args.playerInfo = playerInfo;
                if (NewGameRequested != null)
                {
                    NewGameRequested(this, args);
                }
            }
        }

        /// <summary>
        /// Handle a player leaving a game.
        /// </summary>
        /// <param name="playerInfo">The player.</param>
        private void OnLeaveGame(PlayerInfo playerInfo)
        {
            // Kill game if I am playing against this opponent
            if (_currentOpponent != null && playerInfo.PlayerName == _currentOpponent.PlayerName)
            {
                PlayerEventArgs args = new PlayerEventArgs();
                args.playerInfo = playerInfo;
                if (LeftGame != null)
                {
                    LeftGame(this, args);
                }
            }
        }

        public event OpponentPlayedEventHandler OpponentPlayed;
        public event NewGameRequestedHandler NewGameRequested;
        public event LeftGameHandler LeftGame;
        #endregion

        #region Keep-Alive
        DispatcherTimer _dt;
        private void StartKeepAlive()
        {
            if (_dt == null)
            {
                _dt = new DispatcherTimer();
                _dt.Interval = new TimeSpan(0, 0, 1);
                _dt.Tick +=
                            delegate(object s, EventArgs args)
                            {
                                if (this.Channel != null && IsJoined)
                                {
                                    this.Channel.Send(GameCommands.ReadyFormat, _playerName);
                                }
                            };
            }
            _dt.Start();

        }


        private void StopkeepAlive()
        {
            if (_dt != null)
                _dt.Stop();
        }
        #endregion

        #region IDisposable Implementation
        public void Dispose()
        {
            UnregisterEvents();
            StopkeepAlive();
        }
        #endregion

    }

    // A delegate type for hooking up change notifications.
    public delegate void OpponentPlayedEventHandler(object sender, OpponentPlayedEventArgs e);
    public delegate void NewGameRequestedHandler(object sender, PlayerEventArgs e);
    public delegate void LeftGameHandler(object sender, PlayerEventArgs e);

    public class PlayerEventArgs : EventArgs
    {
        public PlayerInfo playerInfo { get; set; }
    }

    /// <summary>
    /// When we receive a message that the opponent has played, we pass
    /// on the playerInfo and the gamepiece they played.
    /// </summary>
    public class OpponentPlayedEventArgs : EventArgs
    {
        public PlayerInfo playerInfo { get; set; }
        public string gamepiece { get; set; }
    }
}
