using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using CoreUdpClient = System.Net.Sockets.UdpClient;

namespace OneCog.Net
{
    internal class UdpConnection : IDisposable
    {
        private CoreUdpClient _socket;
        private Action _disposed;

        public UdpConnection(CoreUdpClient socket, Action disposed)
        {
            _socket = socket;

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
                UdpReceiveResult result = await _socket.ReceiveAsync();

                // TODO: We may lose bytes here if the buffer isn't big enough to hold the entireity
                // of the received message. This should be resolved with an internal buffer.
                int bytesToCopy = Math.Min(result.Buffer.Length, bytes.Length);

                Array.Copy(result.Buffer, bytes, bytesToCopy);

                return bytesToCopy;
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
                await _socket.SendAsync(bytes, bytes.Length);
            }
            catch (TaskCanceledException)
            {
                // Disposing so do nothing
            }
        }
    }
}
