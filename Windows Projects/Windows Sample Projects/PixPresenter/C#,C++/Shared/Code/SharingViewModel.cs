/* 
    Copyright (c) 2012 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
    To see all Code Samples for Windows Store apps, visit http://code.msdn.microsoft.com/windowsapps.
  
*/
using PixPresenter.PeerConnect;
using PixPresenterPortableLib;
using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace PixPresenter.ViewModels
{
    /// <summary>
    /// Singleton class that contains the logic to control the connection 
    /// between peer apps and to send pictures between them.
    /// </summary>
    public class SharingViewModel : ViewModelBase
    {
        public event EventHandler<PictureReceivedEventArgs> PictureReceived;
        public event EventHandler<ConnectionStatusChangedEventArgs> ConnectionStatusChanged;
        public Action<Action> UIDispatcher { get; set; }
        public static SharingViewModel Instance { get; private set; }

        // Alternate identities so that peer apps can connect from Windows Phone 8 to Windows 8.
        #if NETFX_CORE
            readonly Dictionary<string, string> _alternateIdentities = new Dictionary<string, string>() { { Strings.WP8_AlternateIdentifier, Strings.WP8_AlternateIdentity } };
        #else
            readonly Dictionary<string, string> _alternateIdentities = new Dictionary<string, string>() { { Strings.W8_AlternateIdentifier, Strings.W8_AlternateIdentity } };
        #endif
            PeerConnector _peerConnector; // PeerConnector takes care of connecting to, and communicating with, a peer app.

        static SharingViewModel()
        {
            Instance = new SharingViewModel();
        }

        SharingViewModel()
        {
            IsConnected = false;
            _peerConnector = new PeerConnector(_alternateIdentities);
            _peerConnector.ConnectionStatusChanged += OnConnectionStatusChanged;
            _peerConnector.PictureReceived += OnPictureReceived;
        }

        /// <summary>
        /// Attempt to start a sharing session
        /// </summary>
        /// <remarks>This does not mean we are connected to a peer. It just means we have successfully started the request to share.
        /// The SessionConnectionCompleted event will tell us whether a connection was established.</remarks>
        public void StartSharingSession()
        {
            if (!IsConnected)
            {
                // Create a PeerConnector instance if necessary
                if (_peerConnector == null)
                {
                    _peerConnector = new PeerConnector(_alternateIdentities);
                }

                // Start the connection
                _peerConnector.StartConnect();
            }
        }

        /// <summary>
        /// Uses PeerConnector to stop a sharing session
        /// </summary>
        public void StopSharingSession()
        {
          if (IsConnected && _peerConnector != null)
          {
            _peerConnector.StopConnect();
          }

          IsConnected = false;
        }

        public bool IsConnected { get; private set; }


        /// <summary>
        /// Uses PeerConnector to send an image to a peer.
        /// </summary>
        /// <param name="imageBytes">The encoded image to send, represented as an array of bytes.</param>
        public void SendPictureToPeer(byte[] imageBytes)
        {
            if (!this.IsConnected)
            {
                StartSharingSession();
            }
            else
            {
                if (_peerConnector != null)
                {
                    _peerConnector.SendPictureAsync(imageBytes);
                }
            }
        }

        /// <summary>
        /// Gets and sets the current picture byte array.
        /// </summary>
        public byte[] CurrentSharedPicture
        {
            get
            {
                return _currentSharedPicture;
            }

            protected set
            {
                SetProperty(ref _currentSharedPicture, value);
            }
        }

        void OnPictureReceived(object sender, PictureReceivedEventArgs args)
        {

            SetCurrentPicture(args.Bytes);

            if (PictureReceived != null)
                PictureReceived(this, new PictureReceivedEventArgs());

            Debug.WriteLine("Received {0} bytes", args.Bytes.Length);
        }

        byte[] _currentSharedPicture;

        void SetCurrentPicture(byte[] imageBytes)
        {
            UIDispatcher(() =>
            {
                // Update the CurrentPicture with the bytes we received.
                this.CurrentSharedPicture = imageBytes;
            });
        }

        void OnConnectionStatusChanged(object sender, ConnectionStatusChangedEventArgs e)
        {
            switch (e.Status)
            {
                case ConnectionStatus.Completed:
                    IsConnected = true;
                    break;
                case ConnectionStatus.Canceled:
                    // If I am already connected, the canel just means we accidentally tapped again and I want to stay connected.
                    // If I am not already connected, IsConnected is already false, so no need to do anything.
                    break;
                case ConnectionStatus.Disconnected:
                case ConnectionStatus.Failed:
                case ConnectionStatus.TapNotSupported:
                    IsConnected = false;
                    break;
            }

            if (ConnectionStatusChanged != null)
                ConnectionStatusChanged(this, new ConnectionStatusChangedEventArgs() { Status = e.Status });
        }
    }
}
