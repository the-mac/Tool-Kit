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
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace sdkBluetoothA2AWP8CS
{
    internal class DataHelper
    {
        private StreamSocket _socket;
        DataReader _reader;
        DataWriter _writer;

        internal DataHelper(StreamSocket socket)
        {
            _socket = socket;
        }

        internal StreamSocket Socket
        {
            set
            {
                _socket = value;
            }
        }

        internal async void SendData(byte[] buffer)
        {
            if (_writer == null)
                _writer = new DataWriter(_socket.OutputStream);

            _writer.WriteInt32(buffer.Length);
            await _writer.StoreAsync();
            _writer.WriteBytes(buffer);
            await _writer.StoreAsync();
        }

        internal async Task<byte[]>  ReceiveDataAsync()
        {


            var len = await GetImageSize();

            var data = await GetImageBytes(len);

            return data;
        }

        private async Task<uint> GetImageSize()
        {
            
            if (_reader == null)
                _reader = new DataReader(_socket.InputStream);

            await _reader.LoadAsync(4);
            return (uint)_reader.ReadInt32();

        }

        private async Task<byte[]> GetImageBytes(uint length)
        {
            byte[] imageBytes = new byte[length];
            await _reader.LoadAsync(length);
            _reader.ReadBytes(imageBytes);
            return imageBytes;

        }

    }
}
