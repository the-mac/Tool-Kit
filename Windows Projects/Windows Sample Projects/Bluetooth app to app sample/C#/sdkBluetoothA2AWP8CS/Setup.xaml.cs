/* 
    Copyright (c) 2012 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using sdkBluetoothA2AWP8CS.Resources;
using Windows.Networking.Proximity;
using Windows.Networking.Sockets;

namespace sdkBluetoothA2AWP8CS
{
    public partial class Setup : PhoneApplicationPage
    {

        ObservableCollection<PeerAppInfo> _peerApps;  // a local copy of peer app information
        StreamSocket _socket; // socket object used to communicate with a peer

        // Constructor
        public Setup()
        {
            InitializeComponent();
            Loaded += MainPage_Loaded;
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            // Bluetooth is not available in the emulator. 
            if (Microsoft.Devices.Environment.DeviceType == Microsoft.Devices.DeviceType.Emulator)
            {
                MessageBox.Show(AppResources.Msg_EmulatorMode);
            }

            _peerApps = new ObservableCollection<PeerAppInfo>();
            PairedDevicesList.ItemsSource = _peerApps;
            PeerFinder.ConnectionRequested += PeerFinder_ConnectionRequested;
        }

        void PeerFinder_ConnectionRequested(object sender, ConnectionRequestedEventArgs args)
        {
            try
            {
                if (ShouldConnect(args.PeerInformation))
                {
                    // Go ahead and connect
                    ConnectToPeer(args.PeerInformation);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private bool ShouldConnect(PeerInformation peerInformation)
        {
            return true;
            MessageBoxResult result = MessageBoxResult.None;
            this.Dispatcher.BeginInvoke(() =>
                {
                     result = MessageBox.Show("Do you want to connect with this peer?", "Incoming Peer Request", MessageBoxButton.OKCancel);
                });

            return (result == MessageBoxResult.OK);
        }

        async void ConnectToPeer(PeerInformation peer)
        {
            try
            {
                StreamSocket socket = await PeerFinder.ConnectAsync(peer);
               
                App.Socket = socket;
                

                //imageBytes = await dataHelper.ReceiveDataAsync();

                //this.Dispatcher.BeginInvoke(() =>
                //{
                //    MessageBox.Show(string.Format("Received {0} bytes", imageBytes.Length));
                //});
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
           
        }

        private void FindPeers_Tap(object sender, GestureEventArgs e)
        {
            RefreshPeerAppList();
        }

        /// <summary>
        /// Asynchronous call to re-populate the ListBox of peers.
        /// </summary>
        private async void RefreshPeerAppList()
        {
            // By clearing the backing data, we are effectively clearing the ListBox
            _peerApps.Clear();

            try
            {
                PeerFinder.DisplayName = txtUserId.Text;

                // PeerFinder.Start() is used to advertise our presence so that peers can find us. 
                // It must always be called before FindAllPeersAsync.
                PeerFinder.Start();

                var peers = await PeerFinder.FindAllPeersAsync();

                if (peers.Count == 0)
                {
                    MessageBox.Show(AppResources.Msg_NoPeers);

                }
                else
                {
                    MessageBox.Show(String.Format("Found {0} peers", peers.Count));

                    // Found paired devices.
                    foreach (var peer in peers)
                    {
                        MessageBox.Show(peer.DisplayName);
                        _peerApps.Add(new PeerAppInfo(peer));
                    }

                }
            }
            catch (Exception ex)
            {
                if ((uint)ex.HResult == 0x8007048F)
                {
                    MessageBox.Show(AppResources.Msg_BluetoothOff);
                }
                else if ((uint)ex.HResult == 0x80070005)
                {
                    // You should remove this check before releasing. 
                    MessageBox.Show(AppResources.Msg_MissingCaps);
                }
                else
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void ConnectToSelected_Tap_1(object sender, GestureEventArgs e)
        {
            // Because I enable the ConnectToSelected button only if the user has
            // a device selected, I don't need to check here whether that is the case.

            // Connect to the device
            PeerAppInfo pdi = PairedDevicesList.SelectedItem as PeerAppInfo;
            PeerInformation peer = pdi.PeerInfo;

            // Asynchronous call to connect to the device
            ConnectToPeer(peer);
        }

        //private async void ConnectToDevice(PeerInformation peer)
        //{
        //    if (_socket != null)
        //    {
        //        // Disposing the socket with close it and release all resources associated with the socket
        //        _socket.Dispose();
        //    }

        //    try
        //    {
        //        _socket = new StreamSocket();
        //        //string serviceName = (String.IsNullOrWhiteSpace(peer.ServiceName)) ? tbServiceName.Text : peer.ServiceName;

        //        // Note: If either parameter is null or empty, the call will throw an exception
        //        await _socket.ConnectAsync(peer.HostName, serviceName);

        //        // If the connection was successful, the RemoteAddress field will be populated
        //        MessageBox.Show(String.Format(AppResources.Msg_ConnectedTo, _socket.Information.RemoteAddress.DisplayName));
        //    }
        //    catch (Exception ex)
        //    {
        //        // In a real app, you would want to take action dependent on the type of 
        //        // exception that occurred.
        //        MessageBox.Show(ex.Message);
        //    }
        //}

        private void PairedDevicesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Check whether the user has selected a device
            if (PairedDevicesList.SelectedItem == null)
            {
                // No - hide these fields
                ConnectToSelected.IsEnabled = false;
                //ServiceNameInput.Visibility = Visibility.Collapsed;
            }
            else
            {
                // Yes - enable the connect button
                ConnectToSelected.IsEnabled = true;

                // Show the service name field, if the ServiceName associated with this device is currently empty
                //PeerAppInfo pdi = PairedDevicesList.SelectedItem as PeerAppInfo;
                //ServiceNameInput.Visibility = (String.IsNullOrWhiteSpace(pdi.ServiceName)) ? Visibility.Visible : Visibility.Collapsed;
            }
        }
    }

    /// <summary>
    ///  Class to hold all paired device information
    /// </summary>
    public class PeerAppInfo
    {
        internal PeerAppInfo(PeerInformation peerInformation)
        {
            this.PeerInfo = peerInformation;
            this.DisplayName = this.PeerInfo.DisplayName;
            //this.HostName = this.PeerInfo.HostName.DisplayName;
            //this.ServiceName = this.PeerInfo.ServiceName;
        }

        public string DisplayName { get; private set; }
        //public string HostName { get; private set; }
        //public string ServiceName { get; private set; }
        public PeerInformation PeerInfo { get; private set; }
    }
}
