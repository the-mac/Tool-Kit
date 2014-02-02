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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using sdkBluetoothA2AWP8CS.Resources;
using Windows.Networking.Proximity;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace sdkBluetoothA2AWP8CS
{
    public delegate void ConnectionRequestEventHandler(object sender, ConnectionRequestedEventArgs e);

    class BluetoothHelper
    {
        StreamSocket _socket;
        DataReader _dataReader;
        DataWriter _dataWriter;

        public event ConnectionRequestEventHandler ConnectionRequested;
        public BluetoothHelper()
        {
            PeerFinder.ConnectionRequested += PeerFinder_ConnectionRequested;
        }

        void PeerFinder_ConnectionRequested(object sender, ConnectionRequestedEventArgs args)
        {
            ConnectionRequested(sender, args);
        }

        public void StartAdvertise(string displayName)
        {
            PeerFinder.DisplayName = displayName;
            PeerFinder.Start();
        }

        public void StopAdvertise()
        {
            PeerFinder.Stop();
        }

        public async Task<List<PeerAppInfo>> FindPeers()
        {
            List<PeerAppInfo> peerlist = new List<PeerAppInfo>();

            try
            {
                var peers = await PeerFinder.FindAllPeersAsync();
                foreach (var peer in peers)
                {
                    peerlist.Add(new PeerAppInfo(peer));
                }
            }
            catch (Exception ex)
            {
                if ((uint)ex.HResult == 0x8007048F)
                {
                    MessageBox.Show(AppResources.Err_BluetoothOff);
                }
                else if ((uint)ex.HResult == 0x80070005)
                {
                    // You should remove this check before releasing. 
                    MessageBox.Show(AppResources.Err_MissingCaps);
                }
                else if ((uint)ex.HResult == 0x8000000E)
                {
                    MessageBox.Show(AppResources.Err_NotAdvertising);
                }
                else
                {
                    MessageBox.Show(ex.Message);
                }
            }

            return peerlist;
            
        }

        public async Task<bool> ConnectToPeer(PeerAppInfo peer)
        {
            bool result = false;
            try
            {
                _socket = await PeerFinder.ConnectAsync(peer.PeerInfo);
                result = true;

                // We can preserve battery by not advertising our presence.
                StopAdvertise();
            }
            catch (Exception ex)
            {
                // In this sample, we handle each exception by displaying it and
                // closing any outstanding connection. An exception can occur here if, for example, 
                // the connection was refused, the connection timeout etc.
                MessageBox.Show(ex.Message);
                Disconnect();
            }

            return result;
        }


        public async Task<bool> SendData(string data)
        {
            bool result = false;
            try
            {
                if (_dataWriter == null)
                    _dataWriter = new DataWriter(_socket.OutputStream);

                // Each message is sent in two blocks.
                // The first is the size of the message.
                // The second if the message itself.
                _dataWriter.WriteInt32(data.Length);
                await _dataWriter.StoreAsync();

                _dataWriter.WriteString(data);
                await _dataWriter.StoreAsync();

                result = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return result;
        }

        public async Task<string> GetData()
        {
            string dataReceived = string.Empty;
            try
            {
                if (_dataReader == null)
                    _dataReader = new DataReader(_socket.InputStream);

                // Each message is sent in two blocks.
                // The first is the size of the message.
                // The second if the message itself.
                //var len = await GetMessageSize();
                await _dataReader.LoadAsync(4);
                uint messageLen = (uint)_dataReader.ReadInt32();
                await _dataReader.LoadAsync(messageLen);
                dataReceived = _dataReader.ReadString(messageLen);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return dataReceived;
        }

        public void Disconnect()
        {
            if (_dataReader != null)
            {
                _dataReader.Dispose();
                _dataReader = null;
            }

            if (_dataWriter != null)
            {
                _dataWriter.Dispose();
                _dataWriter = null;
            }

            if (_socket != null)
            {
                _socket.Dispose();
                _socket = null;
            }

        }

        public bool Connected
        {
            get
            {
                return _socket != null;
            }
        }

    }

    /// <summary>
    ///  Class to hold all peer information
    /// </summary>
    public class PeerAppInfo
    {
        internal PeerAppInfo(PeerInformation peerInformation)
        {
            this.PeerInfo = peerInformation;
            this.DisplayName = this.PeerInfo.DisplayName;
        }

        public string DisplayName { get; private set; }
        public PeerInformation PeerInfo { get; private set; }
    }
}
