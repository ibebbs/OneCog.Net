using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using CoreTcpClient = System.Net.Sockets.TcpClient;

namespace OneCog.Net
{
    internal class TcpConnection : IDisposable
    {
        private CoreTcpClient _socket;
        private NetworkStream _stream;
        private Action _disposed;

        public TcpConnection(CoreTcpClient socket, Action disposed)
        {
            _socket = socket;
            _stream = socket.GetStream();

            _disposed = disposed;
        }

        public void Dispose()
        {
            if (_socket != null)
            {
                _socket.Close();
                _socket = null;
            }

            if (_disposed != null)
            {
                _disposed();
                _disposed = null;
            }
        }

        public async Task<int> Read(byte[] bytes, CancellationToken cancellationToken)
        {
            try
            {
                int loaded = await _stream.ReadAsync(bytes, 0, bytes.Length, cancellationToken);

                return loaded;
            }
            catch (TaskCanceledException)
            {
                // Disposing so do nothing
                return -1;
            }
        }

        public async Task Write(byte[] bytes, CancellationToken cancellationToken)
        {
            try
            {
                await _stream.WriteAsync(bytes, 0, bytes.Length, cancellationToken);
            }
            catch (TaskCanceledException)
            {
                // Disposing so do nothing
            }
        }
    }
}
