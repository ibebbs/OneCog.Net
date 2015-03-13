using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace OneCog.Net
{
    public class TcpClient : ITcpClient
    {
        private StreamSocket _socket;
        private DataReader _dataReader;
        private DataWriter _dataWriter;

        public async Task Connect(string host, uint port)
        {
            _socket = new StreamSocket();
            await _socket.ConnectAsync(new HostName(host), port.ToString());
            _dataReader = new DataReader(_socket.InputStream);
            _dataWriter = new DataWriter(_socket.OutputStream);
        }

        public async Task Read(byte[] bytes)
        {
            if (bytes == null) throw new ArgumentNullException("bytes");
            await _dataReader.LoadAsync((uint) bytes.Length);
            _dataReader.ReadBytes(bytes);
        }

        public async Task Write(byte[] bytes)
        {
            if (bytes == null) throw new ArgumentNullException("bytes");
            _dataWriter.WriteBytes(bytes);
            await _dataWriter.StoreAsync();
        }

        public void Dispose()
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
    }
}
