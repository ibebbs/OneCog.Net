using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using CoreTcpClient = System.Net.Sockets.TcpClient;

namespace OneCog.Net
{
    public class TcpClient : ITcpClient
    {
        private CoreTcpClient _tcpClient;
        private NetworkStream _networkStream;

        public async Task Connect(string host, uint port)
        {
            _tcpClient = new CoreTcpClient();
            await _tcpClient.ConnectAsync(host, (int)port);
            _networkStream = _tcpClient.GetStream();
        }

        public Task Read(byte[] bytes)
        {
            if (bytes == null) throw new ArgumentNullException("bytes");

            return _networkStream.ReadAsync(bytes, 0, bytes.Length);
        }

        public Task Write(byte[] bytes)
        {
            if (bytes == null) throw new ArgumentNullException("bytes");

            return _networkStream.WriteAsync(bytes, 0, bytes.Length);
        }

        public void Dispose()
        {
            if (_networkStream != null)
            {
                _networkStream.Dispose();
                _networkStream = null;
            }

            if (_tcpClient != null)
            {
                _tcpClient.Close();
                _tcpClient = null;
            }
        }
    }
}
